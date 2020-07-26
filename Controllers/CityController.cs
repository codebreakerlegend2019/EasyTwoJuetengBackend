using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Dtos.CityDto;
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
    public class CityController : ControllerBase
    {
        private readonly ICrudPattern<City> _crudPattern;
        private readonly IMapper _mapper;
        private readonly IValidator<City, CitySaveDto> _validator;
        private readonly IAuditTrailRepo _auditTrailRepo;

        public CityController(ICrudPattern<City> crudPattern,
            IMapper mapper,
            IValidator<City,CitySaveDto> validator,
            IAuditTrailRepo auditTrailRepo)
        {
            _crudPattern = crudPattern;
            _mapper = mapper;
            _validator = validator;
            _auditTrailRepo = auditTrailRepo;
            _auditTrailRepo.Module = SystemModule.Cities;
        }
        /// <summary>
        /// Creates a City for Work Location
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CitySaveDto resourceSave)
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
            var city = _mapper.Map<City>(resourceSave);

            _crudPattern.Create(city);

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
        /// GetAll Cities 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _crudPattern.GetAll();
            if (cities.Count == 0)
                return NoContent();
            return Ok(cities);
        }

        /// <summary>
        /// GetById City
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var city = await _crudPattern.Get(id);
            if (city == null)
                return NotFound();
            return Ok(city);
        }

        /// <summary>
        /// Update City 
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CitySaveDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var city = await _crudPattern.Get(id);
            if (city == null)
                return NotFound();
            var activity = Validator.GetDifferences(city, _mapper.Map<City>(resourceSave));
            var validator = await _validator.Validate(city, resourceSave);
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

            _crudPattern.Update(resourceSave, city);

            if (!await _crudPattern.SaveChanges())
                return BadRequest("Nothing has been Saved!");


            await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
            {
                Action = TransactionType.EDIT,
                Activity = activity
            });
            return Ok(city);
        }


        /// <summary>
        /// Delete City
        /// </summary>
        /// <param name="id">The id of the value you wish to get.</param>
        /// <returns></returns>
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    _auditTrailRepo.User = User;
        //    var city = await _crudPattern.Get(id);
        //    if (city == null)
        //        return NotFound();
        //    var activity = $"Delete City with Name: {city.Name}, Id: {city.Id}";

        //    _crudPattern.Delete(city);

        //    if (!await _crudPattern.SaveChanges())
        //        return BadRequest("Nothing has been Saved!");


        //    await _auditTrailRepo.SaveSuccessTrail(new AuditTrailSuccessSaveDto()
        //    {
        //        Action = TransactionType.DELETE,
        //        Activity = activity,
        //    });

        //    return Ok(activity);
        //}

    }
}
