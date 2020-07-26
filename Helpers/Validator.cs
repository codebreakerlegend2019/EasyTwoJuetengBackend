using EasyTwoJuetengBackend.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Helpers
{

    public static class Validator
    {
        public static int ToServiceFormNumber(string serviceFormNumber)
        {
            var sfNum = serviceFormNumber.Split('-').ToList();
            if (sfNum.Count > 2)
            {
                var year = sfNum[1];
                var number = sfNum[2];
                if (year != string.Empty && number != string.Empty)
                    return Convert.ToInt32($"{year}{number}");
            }
            return 0;
        }
        public static string GetDifferences<T>(T Old, T New)
        {
            var activity = "Edit ";
            var propsOld = Old.GetType().GetProperties();
            var propsNew = New.GetType().GetProperties();
            var lastItem = propsOld.Last();
            foreach (var propOld in propsOld)
            {
                var propertyName = propOld.Name;

                if (propertyName == "LazyLoader")
                    continue;

                var valueOld = Old.GetType().GetProperty(propertyName).GetValue(Old, null);
                var valueNew = New.GetType().GetProperty(propertyName).GetValue(New, null);
                var changeText = $"Changed {propertyName}: From {valueOld} to {valueNew}";
                var propNew = propsNew.FirstOrDefault(x => x.Name == propOld.Name);



                if (propOld.PropertyType == typeof(string))
                {
                    if (valueOld.ToString().ToLower() != valueNew.ToString().ToLower())
                        activity += changeText;
                }
                if (propOld.PropertyType == typeof(int))
                {
                    if (Convert.ToInt32(valueOld) != Convert.ToInt32(valueNew))
                        activity += changeText;
                }
                if (propOld.PropertyType == typeof(bool))
                {
                    if (Convert.ToBoolean(valueOld) != Convert.ToBoolean(valueNew))
                        activity += changeText;
                }
                if (propOld.PropertyType == typeof(decimal))
                {
                    if (Convert.ToDecimal(valueOld) != Convert.ToDecimal(valueNew))
                        activity += changeText;
                }
                if (propOld.PropertyType == typeof(DateTime))
                {
                    if (Convert.ToDateTime(valueOld) != Convert.ToDateTime(valueNew))
                        activity += changeText;
                }
            }
            var hehe = activity.Split("Changed").Skip(1).ToList();
            var result = string.Join(",", hehe);
            return $"Edited: {result}";
        }

        public static string GenerateActivity<T>(T Model, TransactionType transactionType)
        {
            var activity = new List<string>();

            var props = Model.GetType().GetProperties();
            foreach (var prop in props)
            {
                var propertyName = prop.Name;
                var value = Model.GetType().GetProperty(propertyName).GetValue(Model, null);

                activity.Add($"{propertyName}:{value}");
            }

            var result = $"{transactionType.ToString().ToUpperInvariant()}: {string.Join(",", activity)}";
            return result;
        }
    }
}
