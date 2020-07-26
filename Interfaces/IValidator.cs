using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Interfaces
{
    public interface IValidator<TEntity, TResource>
    {
        Task<ErrorValidator> Validate(TResource resourceSave);
        Task<ErrorValidator> Validate(TEntity entity, TResource resourceSave);
    }
}
