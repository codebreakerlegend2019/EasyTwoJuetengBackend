using EasyTwoJuetengBackend.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.AuditTrailDto
{
    public class AuditTrailSuccessSaveDto
    { 
        public TransactionType Action { get; set; }
        public string Activity { get; set; }
    }
}
