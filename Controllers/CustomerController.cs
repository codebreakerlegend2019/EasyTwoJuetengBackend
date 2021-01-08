using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BrunoZell.ModelBinding;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Dtos.CustomerDto;
using EasyTwoJuetengBackend.Enumerations;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using EasyTwoJuetengBackend.Persistence.AuditTrailRepositories;
using EasyTwoJuetengBackend.Persistence.BlobStorageRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace EasyTwoJuetengBackend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Both")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        #region fields
        private readonly ICrudPattern<Customer> _crudPattern;
        private readonly IMapper _mapper;
        private readonly IValidator<Customer, CustomerSaveDto> _validator;
        private readonly IAuditTrailRepo _auditTrailRepo;
        private readonly IConfiguration _configuration;
        private readonly IBlobStorageRepo _blobStorageRepo;
        private readonly CloudStorageAccount _cloudStorage;
        #endregion
        public CustomerController(ICrudPattern<Customer> crudPattern,
            IMapper mapper,
            IValidator<Customer,CustomerSaveDto> validator,
            IAuditTrailRepo auditTrailRepo,
            IConfiguration configuration,
            IBlobStorageRepo blobStorageRepo)
        {
            _crudPattern = crudPattern;
            _mapper = mapper;
            _validator = validator;
            _auditTrailRepo = auditTrailRepo;
            _configuration = configuration;
            this._blobStorageRepo = blobStorageRepo;
            _auditTrailRepo.Module = SystemModule.Customers;
            var azureBlobConnectionString = _configuration.GetSection("AzureBlobStorage:EasyTwoBlobStorage").Value;
            _cloudStorage = CloudStorageAccount.Parse(azureBlobConnectionString);
        }

        /// <summary>
        /// Creates a Customer for Easy Two Jueteng
        /// </summary>
        /// <param name="profilePictureFile">The id of the value you wish to get.</param>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([ModelBinder(BinderType = typeof(JsonModelBinder))][FromBody] CustomerSaveDto resourceSave, IFormFile profilePictureFile)
        {
            _auditTrailRepo.User = User;
            var activity = Validator.GenerateActivity(resourceSave, TransactionType.ADD);
            var validator = await _validator.Validate(resourceSave);
            if (validator.HasError)
            {
                await _auditTrailRepo.SaveFailedTrail(new AuditTrailFailedSaveDto()
                {
                    Action = TransactionType.ADD,
                    Activity = activity,
                    ErrorMessage = validator.Message
                });
                return BadRequest(validator.Message);
            }
            var customer = _mapper.Map<Customer>(resourceSave);

            _crudPattern.Create(customer);
            var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePictureFile.FileName)}";
            customer.ProfilePictureFileName = newFileName;

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");

            await _blobStorageRepo.UploadFile(profilePictureFile, newFileName);


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.ADD,
                Activity = activity
            });
            return StatusCode(201);
        }

        /// <summary>
        /// GetAll Customers 
        /// <param name="fileName">The id of the value you wish to get.</param>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("profilepicture/image/{fileName}")]
        public async Task<IActionResult> GetProfileImage([FromRoute] string fileName)
        {
            var profileImage = await _blobStorageRepo.DownloadFile(fileName);
            if (profileImage == null)
                return NotFound();
            return profileImage;
        }

        /// <summary>
        /// GetAll Customers 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _crudPattern.GetAll();
            if (customers.Count == 0)
                return NoContent();
            var result = _mapper.Map<List<CustomerReadDto>>(customers);
            return Ok(result);
        }

        /// <summary>
        /// GetById Customer
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var customer = await _crudPattern.Get(id);
            if (customer == null)
                return NotFound();
            var result = _mapper.Map<CustomerReadDto>(customer);
            return Ok(result);
        }

        /// <summary>
        /// Update Customer 
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CustomerSaveDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var customer = await _crudPattern.Get(id);
            if (customer == null)
                return NotFound();
            var activity = Validator.GetDifferences(customer, _mapper.Map<Customer>(resourceSave));
            var validator = await _validator.Validate(customer, resourceSave);
            if (validator.HasError)
            {

                await _auditTrailRepo.SaveFailedTrail(new AuditTrailFailedSaveDto()
                {
                    Action = TransactionType.EDIT,
                    Activity = activity,
                    ErrorMessage = validator.Message,
                });

                return BadRequest(validator.Message);
            }

            _crudPattern.Update(resourceSave, customer);

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.EDIT,
                Activity = activity
            });
            return Ok(customer);
        }


        /// <summary>
        /// Delete Customer
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            _auditTrailRepo.User = User;
            var employee = await _crudPattern.Get(id);
            if (employee == null)
                return NotFound();
            var activity = Validator.GenerateActivity(employee, TransactionType.DELETE);
            _crudPattern.Delete(employee);

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");

            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.DELETE,
                Activity = activity,
            });

            return Ok(activity);
        }
    }
}
