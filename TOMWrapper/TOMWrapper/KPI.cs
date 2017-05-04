using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public partial class KPI
    {
        protected override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Measure": return false;
            }
            return true;
        }

        protected override bool IsEditable(string propertyName)
        {
            return true;
        }
    }
}
