using AutoMapper;
using EasyTwoJuetengBackend.Dtos.UserDto;
using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.MapperProfile
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserUpdateDto>();
            CreateMap<User, UserSaveDto>();
            CreateMap<User, UserReadDto>()
                .ForMember(x => x.Role, opt => opt.MapFrom(x => x.UserRole.Name));

        }
    }
}
