using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.UserDto
{
    public class UserSaveDto
    {
        public string UserName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int UserRoleId { get; set; }
        public int EmployeeId { get; set; }
    }
}
