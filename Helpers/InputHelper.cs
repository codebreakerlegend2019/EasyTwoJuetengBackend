using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Helpers
{
    public static class InputHelper
    {
        public static void Trimmer<T>(T entity) where T: class
        {
            var props = entity.GetType().GetProperties();
            foreach (var propOld in props)
            {
                var propertyName = propOld.Name;
                var value = entity.GetType().GetProperty(propertyName).GetValue(entity, null);

                if (propertyName == "LazyLoader")
                    continue;
                if (propOld.PropertyType == typeof(string) && value!=null)
                    value.ToString().Trim();
            }
        }

    }
    
}
