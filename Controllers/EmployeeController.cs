using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Dtos.EmployeeDto;
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
    public class EmployeeController : ControllerBase
    {
        private readonly ICrudPattern<Employee> _crudPattern;
        private readonly IMapper _mapper;
        private readonly IValidator<Employee, EmployeeSaveDto> _validator;
        private readonly IAuditTrailRepo _auditTrailRepo;

        public EmployeeController(ICrudPattern<Employee> crudPattern,
            IMapper mapper,
            IValidator<Employee, EmployeeSaveDto> validator,
            IAuditTrailRepo auditTrailRepo)
        {
            _crudPattern = crudPattern;
            _mapper = mapper;
            _validator = validator;
            _auditTrailRepo = auditTrailRepo;
            _auditTrailRepo.Module = SystemModule.Employees;
        }

        /// <summary>
        /// Creates a Employee for Easy Two Jueteng
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeSaveDto resourceSave)
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
            var employee = _mapper.Map<Employee>(resourceSave);

            _crudPattern.Create(employee);

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
        /// GetAll Employees 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _crudPattern.GetAll();
            if (employees.Count == 0)
                return NoContent();
            var result = _mapper.Map<List<EmployeeReadDto>>(employees);
            return Ok(result);
        }

        /// <summary>
        /// GetById Employee
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var employee = await _crudPattern.Get(id);
            if (employee == null)
                return NotFound();
            var result = _mapper.Map<EmployeeReadDto>(employee);
            return Ok(result);
        }

        /// <summary>
        /// Update Employee 
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] EmployeeSaveDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var employee = await _crudPattern.Get(id);
            if (employee == null)
                return NotFound();
            var activity = Validator.GetDifferences(employee, _mapper.Map<Employee>(resourceSave));
            var validator = await _validator.Validate(employee, resourceSave);
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

            _crudPattern.Update(resourceSave, employee);

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.EDIT,
                Activity = activity
            });
            return Ok(employee);
        }


        /// <summary>
        /// Delete Employee
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
