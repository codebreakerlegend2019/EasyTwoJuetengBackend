using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string CellphoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
