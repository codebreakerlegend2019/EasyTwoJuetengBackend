using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Models
{
    public class GameSchedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan DrawTime { get; set; }
        public TimeSpan EntryStart { get; set; }
        public TimeSpan EntryEnd { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public bool IsActive { get; set; }
    }
}
