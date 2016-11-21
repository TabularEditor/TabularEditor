using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.DynamicProperties
{
    public class HidePropertyAttribute: Attribute
    {
        public string PropertyName { get; private set; }

        public HidePropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
