using EasyTwoJuetengBackend.Dtos.WorkLocationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.EmployeeDto
{
    public class EmployeeReadDto
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int WorkLocationId { get; set; }
        public WorkLocationReadDto WorkLocation { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
