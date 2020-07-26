using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Models
{
    public class WorkLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barangay { get; set; }
        public int CityId { get; set; }
        public virtual City City { get; set; }
    }
}
