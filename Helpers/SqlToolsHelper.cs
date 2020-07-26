using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Helpers
{
    public static class SqlToolsHelper
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static DataTable ToDataTable<T>(T item)
        {
            var dataTable = new DataTable(typeof(T).Name);

            var props = (typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .Where(x=>!x.GetMethod.IsVirtual)
                .ToArray();
            foreach (PropertyInfo prop in props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }
            var values = new object[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);
            return dataTable;
        }
    }
}
