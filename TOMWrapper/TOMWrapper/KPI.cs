using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public partial class KPI : IDynamicPropertyObject
    {
        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Measure": return false;
            }
            return true;
        }

        public bool Editable(string propertyName)
        {
            return true;
        }
    }
}
