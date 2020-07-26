using AutoMapper;
using EasyTwoJuetengBackend.Dtos.CityDto;
using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.MapperProfile
{
    public class CityProfile:Profile
    {
        public CityProfile()
        {
            CreateMap<City, CitySaveDto>();
            CreateMap<CitySaveDto, City>();
        }
    }
}
