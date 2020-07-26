using AutoMapper;
using EasyTwoJuetengBackend.Dtos.WorkLocationDto;
using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.MapperProfile
{
    public class WorkLocationProfile:Profile
    {
        public WorkLocationProfile()
        {
            CreateMap<WorkLocation, WorkLocationSaveDto>();
            CreateMap<WorkLocationSaveDto, WorkLocation>();
            CreateMap<WorkLocation, WorkLocationReadDto>()
                .ForMember(x => x.City, opt => opt.MapFrom(x => x.City.Name));
        }
    }
}
