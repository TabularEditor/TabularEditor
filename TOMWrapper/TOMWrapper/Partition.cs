using System;
using System.Linq;
using System.Collections.Generic;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Diagnostics;
using System.ComponentModel;
using TabularEditor.UndoFramework;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace TabularEditor.TOMWrapper
{
    public partial class Partition: IDynamicPropertyObject
    {
        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.Partition();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        public Partition(): this(TabularModelHandler.Singleton, new TOM.Partition() { Source = new TOM.QueryPartitionSource() })
        {
            if (Model.DataSources.Count == 0) throw new Exception("Unable to create partitions on a model with no data sources.");
            DataSource = Model.DataSources.FirstOrDefault();
        }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
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

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Expression
        {
            get
            {
                return (MetadataObject.Source as TOM.CalculatedPartitionSource)?.Expression;
            }
            set
            {
                if (MetadataObject.Source is TOM.CalculatedPartitionSource)
                {
                    var oldValue = Expression;
                    if (oldValue == value) return;
                    bool undoable = true;
                    bool cancel = false;
                    OnPropertyChanging("Expression", value, ref undoable, ref cancel);
                    if (cancel) return;
                    (MetadataObject.Source as TOM.CalculatedPartitionSource).Expression = value;
                    if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "Expression", oldValue, value));
                    OnPropertyChanged("Expression", oldValue, value);
                }
            }
        }

        [TypeConverter(typeof(DataSourceConverter))]
        public DataSource DataSource
        {
            get
            {
                if (MetadataObject.Source is TOM.QueryPartitionSource)
                {
                    var ds = (MetadataObject.Source as TOM.QueryPartitionSource)?.DataSource;
                    return ds == null ? null : Handler.WrapperLookup[ds] as DataSource;
                }
                else return null;
            }
            set
            {
                if (MetadataObject.Source is TOM.QueryPartitionSource)
                {
                    if (value == null) return;
                    var oldValue = DataSource;
                    if (oldValue == value) return;
                    bool undoable = true;
                    bool cancel = false;
                    OnPropertyChanging("DataSource", value, ref undoable, ref cancel);
                    if (cancel) return;
                    (MetadataObject.Source as TOM.QueryPartitionSource).DataSource = value?.MetadataObject;
                    if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "DataSource", oldValue, value));
                    OnPropertyChanged("DataSource", oldValue, value);
                }
            }
        }

        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "DataSource":
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
                case "DataSource":
                case "Query":
                    if (MetadataObject.SourceType == TOM.PartitionSourceType.Query) return true;
                    return false;
                default:
                    return false;
            }
        }
    }
}
