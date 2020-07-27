using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Dtos.CustomerDto;
using EasyTwoJuetengBackend.Enumerations;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using EasyTwoJuetengBackend.Persistence.AuditTrailRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyTwoJuetengBackend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Both")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICrudPattern<Customer> _crudPattern;
        private readonly IMapper _mapper;
        private readonly IValidator<Customer, CustomerSaveDto> _validator;
        private readonly IAuditTrailRepo _auditTrailRepo;

        public CustomerController(ICrudPattern<Customer> crudPattern,
            IMapper mapper,
            IValidator<Customer,CustomerSaveDto> validator,
            IAuditTrailRepo auditTrailRepo)
        {
            _crudPattern = crudPattern;
            _mapper = mapper;
            _validator = validator;
            _auditTrailRepo = auditTrailRepo;
            _auditTrailRepo.Module = SystemModule.Customers;
        }

        /// <summary>
        /// Creates a Customer for Easy Two Jueteng
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerSaveDto resourceSave)
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

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.ADD,
                Activity = activity
            });
            return StatusCode(201);
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
