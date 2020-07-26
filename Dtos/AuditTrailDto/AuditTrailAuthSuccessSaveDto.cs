using EasyTwoJuetengBackend.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.AuditTrailDto
{
    public class AuditTrailAuthSuccessSaveDto
    {
        public TransactionType Action { get; set; }
        public string Activity { get; set; }
        public string UserInCharge { get; set; }
        public int UserId { get; set; }
        public string UserRole { get; set; }
    }
}
