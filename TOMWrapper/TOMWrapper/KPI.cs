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

        public bool CanDelete()
        {
            return true;
        }

        public bool CanDelete(out string message)
        {
            message = null;
            return true;
        }

        public void Delete()
        {
            Measure.RemoveKPI();
        }

        protected override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.MEASURE: return false;
            }
            return true;
        }

        protected override bool IsEditable(string propertyName)
        {
            if (propertyName == Properties.NAME) return false;
            return true;
        }
    }
}
