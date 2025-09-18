using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class DataCoverageDefinition: IExpressionObject, ITabularTableObject
    {
        [Browsable(false)]
        public Table Table => Partition.Table;

        bool ITabularNamedObject.CanEditName() { return false; }

        [Browsable(false)]
        public int MetadataIndex
        {
            get
            {
                return 0;
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == nameof(Expression))
            {
                FormulaFixup.BuildDependencyTree(Partition);
            }
        }

        public static DataCoverageDefinition CreateNew()
        {
            var tomDataCoverageDefinition = new TOM.DataCoverageDefinition();
            var obj = new DataCoverageDefinition(tomDataCoverageDefinition);
            obj.Init();
            return obj;
        }

        public static DataCoverageDefinition CreateFromMetadata(Partition parent, TOM.DataCoverageDefinition metadataObject)
        {
            var obj = new DataCoverageDefinition(metadataObject);
            parent.MetadataObject.DataCoverageDefinition = metadataObject;

            obj.Init();

            return obj;
        }

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "DataCoverageDefinition";
            }

            set
            {

            }
        }

        [Browsable(false)]
        public bool NeedsValidation => IsExpressionModified;

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
            Partition.RemoveDataCoverageDefinition();
        }

        private protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case Properties.PARTITION: return false;
            }
            return true;
        }

        private protected override bool IsEditable(string propertyName)
        {
            if (propertyName == Properties.NAME) return false;
            return true;
        }
    }
}
