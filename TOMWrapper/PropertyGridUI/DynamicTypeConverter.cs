using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper.DynamicProperties
{
    public class DynamicTypeConverter: TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return base.GetProperties(context, value, attributes);
        }
    }
}
