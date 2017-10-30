using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class KPI: ITabularNamedObject, IDaxDependantObject
    {


        [Browsable(false)]
        public int MetadataIndex
        {
            get
            {
                return 0;
            }
        }

        public static KPI CreateFromMetadata(Measure parent, TOM.KPI metadataObject)
        {
            var obj = new KPI(metadataObject);
            parent.MetadataObject.KPI = metadataObject;

            obj.Init();

            return obj;
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

        private DependsOnList _dependsOn = null;

        [Browsable(false)]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
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
