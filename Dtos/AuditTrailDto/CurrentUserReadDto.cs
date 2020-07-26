using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.AuditTrailDto
{
    public class CurrentUserReadDto
    {
        public int Id { get; set; }
        public string UserInCharge { get; set; }
        public string Role { get; set; }
    }
}
