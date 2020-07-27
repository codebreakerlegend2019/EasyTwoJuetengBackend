using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using EasyTwoJuetengBackend.Dtos.AuthDto;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Persistence.AuthRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using EasyTwoJuetengBackend.Persistence.AuditTrailRepositories;
using EasyTwoJuetengBackend.Enumerations;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EasyTwoJuetengBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _repo;
        private readonly IConfiguration _configuration;
        private readonly IAuditTrailRepo _auditTrailRepo;

        public AuthController(IAuthRepo repo,
            IConfiguration configuration,
            IAuditTrailRepo auditTrailRepo)
        {
            _repo = repo;
            _configuration = configuration;
            _auditTrailRepo = auditTrailRepo;
            _auditTrailRepo.Module = SystemModule.Auth;
        }

        /// <summary>
        /// Use to Login to WebApp/Agent App To Access Data
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login/token")]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsSaveDto resourceSave)
        {
            var activity = $"Logged In {resourceSave.Username}, Password: {AES.Encrypt(resourceSave.Password)}";
            if (resourceSave.Username == null || resourceSave.Password == null)
                return BadRequest("No Username or Password Provided!!!");


            var user = await _repo.LoginChecker(resourceSave);
            if (!user.IsSuccess)
                return Unauthorized();

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                            new Claim("LoggedInUser", AES.Encrypt($"{user.UserName}")),
                            new Claim(ClaimTypes.Role, user.Role),
                            new Claim("UserId", AES.Encrypt(user.UserId.ToString())),
                            new Claim("FullName", AES.Encrypt(user.UserFullName)),
                        }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                Issuer = _configuration.GetSection("TokenAuthentication:Issuer").Value,
                Audience = _configuration.GetSection("TokenAuthentication:Audience").Value
            };
            var tokenBytes = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenBytes);

            var result = new LoginResultReadDto()
            {
                Nickname = user.Nickname,
                LogginedUser = $"{user.UserName}",
                Token = token,
                FullName = user.UserFullName,
                Role = user.Role
            };

            await _auditTrailRepo.SaveSuccessAuthTrail(new AuditTrailAuthSuccessSaveDto()
            {
                Action = TransactionType.LOGINTOSYSTEM,
                Activity = $"{activity} Role: {user.Role}",
                UserInCharge = $"{user.UserName}",
                UserId = user.UserId,
                UserRole = user.Role
            });

            return Ok(result);

        }
        /// <summary>
        /// Use to Change Password for Both Admin / Agent Role
        /// </summary>
        /// <param name="resourceSave">The id of the value you wish to get.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Both")]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordSaveDto resourceSave)
        {
            _auditTrailRepo.User = User;
            var changepassword = await _repo.ChangePassword(_auditTrailRepo.AuthenticatedUserDetails.Id,resourceSave);
            if(changepassword.HasError)
            {
                await _auditTrailRepo.SaveFailedTrail(new AuditTrailFailedSaveDto()
                {
                    Action = TransactionType.CHANGEPASSWORD,
                    Activity = $"Change Password User:{_auditTrailRepo.AuthenticatedUserDetails.UserInCharge}",
                    ErrorMessage = changepassword.Message
                });
                return BadRequest(changepassword.Message);
            }
            return Ok("Change Password Done!");
        }
    }
}
