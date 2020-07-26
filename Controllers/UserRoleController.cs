using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTwoJuetengBackend.DataContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyTwoJuetengBackend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly EasyTwoJuetengContext _context;

        public UserRoleController(EasyTwoJuetengContext context)
        {
            _context = context;
        }


        /// <summary>
        /// GetAll UserRoles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRoles = await _context.UserRoles
                .AsNoTracking()
                .ToListAsync();
            if (userRoles.Count == 0)
                return NoContent();
            return Ok(userRoles);
        }
    }
}
