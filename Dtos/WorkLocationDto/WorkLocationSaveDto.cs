using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.WorkLocationDto
{
    public class WorkLocationSaveDto
    {
        public string Name { get; set; }
        public string Barangay { get; set; }
        public int CityId { get; set; }
    }
}
