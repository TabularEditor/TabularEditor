using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    internal class DataTypeEnumConverter: EnumConverter
    {
        static public Dictionary<TOMWrapper.DataType, string> DataTypeStrings = new Dictionary<TOMWrapper.DataType, string>()
        {
            { TOMWrapper.DataType.Int64, "Integer / Whole Number (int64)" },
            { TOMWrapper.DataType.Decimal, "Currency / Fixed Decimal Number (decimal)" },
            { TOMWrapper.DataType.Double, "Floating Point / Decimal Number (double)" },
            { TOMWrapper.DataType.String, "String / Text" },
            { TOMWrapper.DataType.Boolean, "Boolean / (true/false)" },
        };

        public DataTypeEnumConverter() : base(typeof(TOMWrapper.DataType)) { }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var enumValue = (TOMWrapper.DataType)value;
            if (DataTypeStrings.ContainsKey(enumValue)) return DataTypeStrings[enumValue];
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var strValue = (string)value;
            if (DataTypeStrings.ContainsValue(strValue)) return DataTypeStrings.FirstOrDefault(kvp => kvp.Value == strValue).Key;
            return base.ConvertFrom(context, culture, value);
        }
    }
}
