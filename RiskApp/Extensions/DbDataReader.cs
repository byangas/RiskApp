using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Extensions
{
    public static class DbDataReaderExtension
    {
        public static DateTime GetUniversalTime(this DbDataReader reader, string fieldName)
        {
            return GetValue<DateTime>(reader, fieldName).ToUniversalTime();
        }


        public static T GetValue<T>(this DbDataReader reader, string fieldName)
        {
            object value = reader[fieldName];
            if (value == DBNull.Value)
            {
                return default;
            }

            //todo: refactor to "GetEnumValue" so that we don't have overhead of typeof call
            Type type = typeof(T);
            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, (string)value);
            }
            try
            {
                return (T)value;
            }
            catch (InvalidCastException ex)
            {
                int ordinal = reader.GetOrdinal(fieldName);
                return reader.GetFieldValue<T>(ordinal);
            }
   

        }

    }
}
