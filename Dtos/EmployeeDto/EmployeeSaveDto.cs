using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.EmployeeDto
{
    public class EmployeeSaveDto
    {
        [Required]
        public string NickName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int WorkLocationId { get; set; }
    }
}
