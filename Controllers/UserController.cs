using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Dtos.UserDto;
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
    public class UserController : ControllerBase
    {
        private readonly ICrudPattern<User> _crudPattern;
        private readonly IMapper _mapper;
        private readonly IValidator<User, UserSaveDto> _validatorSaving;
        private readonly IValidator<User, UserUpdateDto> _validatorUpdating;
        private readonly IAuditTrailRepo _auditTrailRepo;

        public UserController(ICrudPattern<User> crudPattern,
            IMapper mapper,
            IValidator<User,UserSaveDto> validatorSaving,
            IValidator<User,UserUpdateDto> validatorUpdating,
            IAuditTrailRepo auditTrailRepo)
        {
            _crudPattern = crudPattern;
            _mapper = mapper;
            _validatorSaving = validatorSaving;
            _validatorUpdating = validatorUpdating;
            _auditTrailRepo = auditTrailRepo;
            _auditTrailRepo.Module = SystemModule.User;
        }
        /// <summary>
        /// Creates a User to Access the System
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserSaveDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var activity = Validator.GenerateActivity(resourceSave, TransactionType.ADD);
            var validator = await _validatorSaving.Validate(resourceSave);
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
            var user = _mapper.Map<User>(resourceSave);

            _crudPattern.Create(user);

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
        /// GetAll Users 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _crudPattern.GetAll();
            if (users.Count == 0)
                return NoContent();
            var result = _mapper.Map<List<UserReadDto>>(users);
            return Ok(result);
        }

        /// <summary>
        /// GetById User
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _crudPattern.Get(id);
            if (user == null)
                return NotFound();
            var result = _mapper.Map<UserReadDto>(user);
            return Ok(result);
        }

        /// <summary>
        /// Update User 
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UserUpdateDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var user = await _crudPattern.Get(id);
            if (user == null)
                return NotFound();
            var activity = Validator.GetDifferences(user, _mapper.Map<User>(resourceSave));
            var validator = await _validatorUpdating.Validate(user, resourceSave);
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

            _mapper.Map(resourceSave, user);

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.EDIT,
                Activity = activity
            });
            return Ok(user);
        }


        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            _auditTrailRepo.User = User;
            var user = await _crudPattern.Get(id);
            if (user == null)
                return NotFound();
            var activity = Validator.GenerateActivity(user, TransactionType.DELETE);
            _crudPattern.Delete(user);

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
