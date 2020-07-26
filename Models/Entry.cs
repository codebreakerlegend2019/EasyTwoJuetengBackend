using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public int GameModeId { get; set; }
        public virtual GameMode GameMode { get; set; }
        public int GameScheduleId { get; set; }
        public virtual GameSchedule GameSchedule { get; set; }
        public DateTime DateTimeEncoded { get; set; }
        public DateTime DateTimeLogged { get; set; }
    }
}
