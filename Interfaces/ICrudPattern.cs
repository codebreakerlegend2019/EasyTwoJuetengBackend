using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Interfaces
{
    public interface ICrudPattern<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> Get(int id);
        void Create(T model);

        void Delete(T model);
        Task<bool> SaveChanges();

        void Update(object newUpdate, T modelToBeUpdated);
    }
}
