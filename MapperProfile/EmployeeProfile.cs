using AutoMapper;
using EasyTwoJuetengBackend.Dtos.EmployeeDto;
using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.MapperProfile
{
    public class EmployeeProfile:Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeReadDto>();
            CreateMap<Employee, EmployeeSaveDto>();
            CreateMap<EmployeeSaveDto, Employee>();
        }
    }
}
