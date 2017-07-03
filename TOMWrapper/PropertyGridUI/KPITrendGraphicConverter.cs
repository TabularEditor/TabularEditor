using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    public class KPITrendGraphicConverter: TypeConverter
    {
        public static readonly string[] TrendValues = {
            "Faces",
            "Reversed status arrow",
            "Standard Arrow",
            "Status Arrow"
        };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(TrendValues);
        }
    }
}
