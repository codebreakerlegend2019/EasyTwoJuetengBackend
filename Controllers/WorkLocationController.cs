using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Dtos.WorkLocationDto;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkLocationController : ControllerBase
    {
        private readonly ICrudPattern<WorkLocation> _crudPattern;
        private readonly IMapper _mapper;
        private readonly IValidator<WorkLocation, WorkLocationSaveDto> _validator;
        private readonly IAuditTrailRepo _auditTrailRepo;

        public WorkLocationController(ICrudPattern<WorkLocation> crudPattern,
            IMapper mapper,
            IValidator<WorkLocation,WorkLocationSaveDto> validator,
            IAuditTrailRepo auditTrailRepo)
        {
            _crudPattern = crudPattern;
            _mapper = mapper;
            _validator = validator;
            _auditTrailRepo = auditTrailRepo;
            _auditTrailRepo.Module = SystemModule.WorkLocations;
        }

        /// <summary>
         /// Creates a Work Location for Employee
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkLocationSaveDto resourceSave)
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
            var workLocation = _mapper.Map<WorkLocation>(resourceSave);

            _crudPattern.Create(workLocation);

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
        /// GetAll Work Locations 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workLocations = await _crudPattern.GetAll();
            if (workLocations.Count == 0)
                return NoContent();
            var result = _mapper.Map<List<WorkLocationReadDto>>(workLocations);
            return Ok(result);
        }

        /// <summary>
        /// GetById Work Location
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var workLocation = await _crudPattern.Get(id);
            if (workLocation == null)
                return NotFound();
            var result = _mapper.Map<WorkLocationReadDto>(workLocation);
            return Ok(result);
        }

        /// <summary>
        /// Update City 
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] WorkLocationSaveDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var workLocation = await _crudPattern.Get(id);
            if (workLocation == null)
                return NotFound();
            var activity = Validator.GetDifferences(workLocation, _mapper.Map<WorkLocation>(resourceSave));
            var validator = await _validator.Validate(workLocation, resourceSave);
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

            _crudPattern.Update(resourceSave, workLocation);

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.EDIT,
                Activity = activity
            });
            return Ok(workLocation);
        }
    }

}
