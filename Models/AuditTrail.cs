using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string Activity { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string UsernameInCharge { get; set; }
        public string UserRole { get; set; }
        public DateTime DateTimeOccurred { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
    }
}
