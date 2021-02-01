using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI.Converters
{
    public class ConnectionStringConverter: TypeConverter
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
            if(destinationType == typeof(string))
            {
                try
                {
                    var csb = new DbConnectionStringBuilder();
                    csb.ConnectionString = value.ToString();
                    var modded = false;
                    if (csb.ContainsKey("password")) { csb["password"] = "********"; modded = true; }
                    if (csb.ContainsKey("pwd")) { csb["pwd"] = "********"; modded = true; }
                    if (csb.ContainsKey("secret")) { csb["secret"] = "********"; modded = true; }
                    return modded ? csb.ToString() : value;
                }
                catch
                {
                    return value;
                }
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }
    }
}
