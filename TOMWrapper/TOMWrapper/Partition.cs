using System;
using System.Linq;
using System.Collections.Generic;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Diagnostics;
using System.ComponentModel;
using TabularEditor.UndoFramework;

namespace TabularEditor.TOMWrapper
{
    public partial class Partition: IDynamicPropertyObject
    {
        public string Query
        {
            set
            {
                if (MetadataObject.Source is TOM.QueryPartitionSource)
                {
                    var oldValue = Query;
                    if (oldValue == value) return;
                    bool undoable = true;
                    bool cancel = false;
                    OnPropertyChanging("Query", value, ref undoable, ref cancel);
                    if (cancel) return;
                    (MetadataObject.Source as TOM.QueryPartitionSource).Query = value;
                    if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "Query", oldValue, value));
                    OnPropertyChanged("Query", oldValue, value);
                }
            }
            get
            {
                return (MetadataObject.Source as TOM.QueryPartitionSource)?.Query;
            }
        }

        public string Source
        {
            get
            {
                return (MetadataObject.Source as TOM.QueryPartitionSource)?.DataSource.Name;
            }
        }

        public string Expression
        {
            get
            {
                return (MetadataObject.Source as TOM.CalculatedPartitionSource)?.Expression;
            }
        }

        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Query":
                    return SourceType == TOM.PartitionSourceType.Query;
                case "Expression":
                    return SourceType == TOM.PartitionSourceType.Calculated;
                case "Mode":
                case "Description":
                case "Name":
                case "RefreshedTime":
                    return true;
                default:
                    return false;
            }
        }

        [Category("Metadata"),DisplayName("Last Processed")]
        public DateTime RefreshedTime
        {
            get { return MetadataObject.RefreshedTime; }
        }

        public override string Name
        {
            set
            {
                base.Name = value;
            }
        }

        public bool Editable(string propertyName)
        {
            switch(propertyName)
            {
                case "Name":
                case "Description":
                    return true;
                case "Query":
                    if (MetadataObject.SourceType == TOM.PartitionSourceType.Query) return true;
                    return false;
                default:
                    return false;
            }
        }
    }
}
