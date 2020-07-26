using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Enumerations;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Models;
using EasyTwoJuetengBackend.Persistence.AuditTrailRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.AuditTrailRepositories
{
    public class AuditTrailRepo : IAuditTrailRepo
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly IMapper _mapper;
        private readonly IActionContextAccessor _accessor;

        public AuditTrailRepo(EasyTwoJuetengContext context, IMapper mapper, IActionContextAccessor accessor)
        {
            this._context = context;
            this._mapper = mapper;
            this._accessor = accessor;
        }
        public ClaimsPrincipal User { get; set; }
        public SystemModule Module { get; set; }

        private string GetIpAddress()
        {
            return _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        public async Task SaveSuccessTrail(AuditTrailSuccessSaveDto model)
        {
            var auditTrail = _mapper.Map<AuditTrail>(model);
            auditTrail.IsSuccessful = true;
            auditTrail.Module = Module.ToString();
            auditTrail.DateTimeOccurred = DateTime.Now;
            auditTrail.ErrorMessage = "No Error";
            auditTrail.UserRole = AuthenticatedUserDetails.Role;
            auditTrail.UsernameInCharge = AuthenticatedUserDetails.UserInCharge;
            auditTrail.UserId = AuthenticatedUserDetails.Id;
            auditTrail.IpAddress = GetIpAddress();
            _context.Add(auditTrail);
            await _context.SaveChangesAsync();
        }

        public void SaveSuccessTrails(List<AuditTrailSuccessSaveDto> models)
        {
            if (models is null)
            {
                throw new ArgumentNullException(nameof(models));
            }

            var auditTrails = new List<AuditTrail>();
            foreach(var model in models)
            {
                var auditTrail = _mapper.Map<AuditTrail>(model);
                auditTrail.IsSuccessful = true;
                auditTrail.Module = Module.ToString();
                auditTrail.DateTimeOccurred = DateTime.Now;
                auditTrail.ErrorMessage = "No Error";
                auditTrail.UserRole = AuthenticatedUserDetails.Role;
                auditTrail.UsernameInCharge = AuthenticatedUserDetails.UserInCharge;
                auditTrail.UserId = AuthenticatedUserDetails.Id;
                auditTrail.IpAddress = GetIpAddress();
                auditTrails.Add(auditTrail);
            }
            _context.AddRange(auditTrails);

        }
        public async Task SaveFailedTrail(AuditTrailFailedSaveDto model)
        {
            var auditTrail = _mapper.Map<AuditTrail>(model);
            auditTrail.IsSuccessful = false;
            auditTrail.DateTimeOccurred = DateTime.Now;
            auditTrail.Module = Module.ToString();
            auditTrail.UsernameInCharge = AuthenticatedUserDetails.UserInCharge;
            auditTrail.UserRole = AuthenticatedUserDetails.Role;
            auditTrail.UserId = AuthenticatedUserDetails.Id;
            auditTrail.IpAddress = GetIpAddress();
            _context.Add(auditTrail);
            await _context.SaveChangesAsync();
        }
        public async Task SaveSuccessAuthTrail(AuditTrailAuthSuccessSaveDto model)
        {
            var auditTrail = _mapper.Map<AuditTrail>(model);
            auditTrail.IsSuccessful = true;
            auditTrail.DateTimeOccurred = DateTime.Now;
            auditTrail.Module = Module.ToString();
            auditTrail.IpAddress = GetIpAddress();
            _context.Add(auditTrail);
            await _context.SaveChangesAsync();
        }
        public CurrentUserReadDto AuthenticatedUserDetails =>
             new CurrentUserReadDto()
            {
                Id = Convert.ToInt32(AES.Decrypt(User.FindFirst("UserId").Value)),
                Role = User.FindFirst(ClaimTypes.Role).Value,
                UserInCharge = AES.Decrypt(User.FindFirst("LoggedInUser").Value)
            };


    }
}
