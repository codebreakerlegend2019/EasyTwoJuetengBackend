using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.WorkLocationDto
{
    public class WorkLocationReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
    }
}
