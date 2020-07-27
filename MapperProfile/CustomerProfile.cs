using AutoMapper;
using EasyTwoJuetengBackend.Dtos.CustomerDto;
using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.MapperProfile
{
    public class CustomerProfile:Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerReadDto>()
                .AfterMap((src, dest) => dest.FullName = $"{src.FirstName} {src.MiddleName} {src.LastName}");
            CreateMap<CustomerSaveDto, Customer>();
            CreateMap<Customer, CustomerSaveDto>();

        }
    }
}
