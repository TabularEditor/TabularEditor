using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public partial class KPI: ITabularNamedObject
    {
        [Browsable(false)]
        public int MetadataIndex
        {
            get
            {
                return 0;
            }
        }

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "KPI";
            }

            set
            {
                
            }
        }

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
            if (propertyName == "Name") return false;
            return true;
        }
    }
}
