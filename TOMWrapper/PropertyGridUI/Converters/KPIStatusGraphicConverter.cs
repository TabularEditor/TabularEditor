using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    internal class KPIStatusGraphicConverter: TypeConverter
    {
        public static string[] StatusValues = {
            "Cylinder",
            "Faces",
            "Five Bars Colored",
            "Five Boxes Colored",
            "Gauge",
            "Gauge - Ascending",
            "Gauge - Descending",
            "Reversed Gauge",
            "Reversed status arrow",
            "Road Signs",
            "Shapes",
            "Smiley",
            "Smiley Face",
            "Standard Arrow",
            "Status Arrow",
            "Thermometer",
            "Three Triangles",
            "Three Circles Colored",
            "Three Flags Colored",
            "Three Stars Colored",
            "Three Symbols Uncircled Colored",
            "Traffic Light",
            "Traffic Light - Single",
            "Variance Arrow",
            "Status Arrow - Ascending",
            "Status Arrow - Descending"
        };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(StatusValues);
        }
    }
}
