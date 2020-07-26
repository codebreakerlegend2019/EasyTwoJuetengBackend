using AutoMapper;
using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.MapperProfile
{
    public class AuditTrailProfile:Profile
    {
        public AuditTrailProfile()
        {
            CreateMap<AuditTrailFailedSaveDto, AuditTrail>();
            CreateMap<AuditTrailAuthSuccessSaveDto, AuditTrail>();
            CreateMap<AuditTrailSuccessSaveDto, AuditTrail>();
        }
    }
}
