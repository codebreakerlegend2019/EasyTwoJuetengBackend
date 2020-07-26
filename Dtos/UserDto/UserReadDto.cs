using EasyTwoJuetengBackend.Dtos.EmployeeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.UserDto
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string Role { get; set; }
        public EmployeeReadDto Employee { get; set; }
    }
}
