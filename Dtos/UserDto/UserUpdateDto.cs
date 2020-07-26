using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.UserDto
{
    public class UserUpdateDto
    {
        public string UserName { get; set; }
        public int UserRoleId { get; set; }
    }
}
