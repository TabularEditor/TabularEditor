using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    public class FormatStringConverter: TypeConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] {
                "0",
                "0.00",
                "#,##0.00",
                "0 %; -0 %; 0 %",
                "#,##0.00;(#,##0.00)",
                "\"$\" #,0.00;-\"$\" #,0.00;\"$\" #,0.00",
                "0.00E+000"
            });
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
