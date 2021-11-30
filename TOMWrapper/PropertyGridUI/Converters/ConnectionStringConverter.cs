using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.PropertyGridUI.Converters
{
    public class ConnectionStringConverter : TypeConverter
    {


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                // Mask password if present:
                if (value != null && TryGetPassword(value.ToString(), out string key, out string password))
                {
                    var csb = new DbConnectionStringBuilder();
                    csb.ConnectionString = value.ToString();
                    csb[key] = "********";
                    return csb.ToString();
                }
                return value;
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        internal static bool TryGetPassword(string connectionString, out string key, out string password)
        {
            try
            {
                var csb = new DbConnectionStringBuilder();
                csb.ConnectionString = connectionString;
                return csb.TryGet(new[] { "password", "pwd", "secret", "key" }, out key, out password);
            }
            catch
            {
                key = null;
                password = null;
                return false;
            }

        }
    }
}
