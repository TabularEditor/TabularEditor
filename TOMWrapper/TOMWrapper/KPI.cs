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
    public partial class KPI: ITabularNamedObject, IDaxDependantObject, IExpressionObject, ITabularTableObject
    {
        [Browsable(false)]
        public Table Table => Measure.Table;

        [DisplayName("Parent Measure"), Category("Metadata")]
        public string MeasureName => Measure?.DaxObjectName;

        bool ITabularNamedObject.CanEditName() { return false; }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch(propertyName)
            {
                case Properties.STATUSEXPRESSION:
                case Properties.TARGETEXPRESSION:
                case Properties.TRENDEXPRESSION:
                    FormulaFixup.BuildDependencyTree(this);
                    break;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        [Browsable(false)]
        public int MetadataIndex
        {
            get
            {
                return 0;
            }
        }

        public static KPI CreateNew()
        {
            var tomKpi = new TOM.KPI();
            var obj = new KPI(tomKpi);
            obj.Init();
            return obj;
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

        [Browsable(false)]
        public bool NeedsValidation => IsExpressionModified;

        [Browsable(false)]
        public string Expression
        {
            get
            {
                return StatusExpression;
            }

            set
            {
                StatusExpression = value;
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

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.MEASURE: return false;
            }
            return true;
        }

        internal override bool IsEditable(string propertyName)
        {
            if (propertyName == Properties.NAME) return false;
            return true;
        }
    }
}
