using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

    }
}
