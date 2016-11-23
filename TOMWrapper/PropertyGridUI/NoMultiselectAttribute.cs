using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI
{
    public class NoMultiselectAttribute: Attribute
    {
        public static NoMultiselectAttribute Default = new NoMultiselectAttribute();
    }
}
