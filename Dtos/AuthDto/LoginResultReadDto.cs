using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.AuthDto
{
    public class LoginResultReadDto
    {

        public string Token { get; set; }
        public string LogginedUser { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
