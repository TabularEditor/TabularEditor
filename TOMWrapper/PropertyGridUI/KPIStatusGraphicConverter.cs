using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    public class KPIStatusGraphicConverter: TypeConverter
    {
        public static readonly string[] StatusValues = {
            "Cylinder",
            "Faces",
            "Gauge",
            "Reversed Gauge",
            "Road Signs",
            "Shapes",
            "Thermometer",
            "Traffic Light",
            "Variance arrow"
        };

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
