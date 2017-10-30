using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    internal class CultureConverter : TypeConverter
    {
        public static Dictionary<string, CultureInfo> Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToDictionary(c => c.Name, c => c);

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Cultures.Values.OrderBy(c => c.DisplayName).Select(c => c.Name + " - " + c.DisplayName).ToList());
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (value as string).Split(' ').First();
            }
            else
                throw new InvalidOperationException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is string)
            {
                var cn = (value as string).Split(' ').First();
                CultureInfo c;
                if (Cultures.TryGetValue(cn, out c))
                {
                    return c.Name + " - " + c.DisplayName;
                }
                return "Unknown culture";
            }
            else
                throw new InvalidOperationException();
        }
    }
}
