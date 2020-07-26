using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Dtos.ValidatorDto
{
    public class ErrorValidator
    {
        public ErrorValidator(string error)
        {
            Message = error;
            HasError = true;
        }
        public ErrorValidator()
        {
            Message = "";
            HasError = false;
        }
        public string Message { get; set; }
        public bool HasError { get; set; }
    }
}
