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
            ////tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        public Partition(): this(new TOM.Partition() { Source = new TOM.QueryPartitionSource() })
        {
            if (Model.DataSources.Count == 0) throw new Exception("Unable to create partitions on a model with no data sources.");
            DataSource = Model.DataSources.FirstOrDefault();
        }

        public string Source
        {
            get
            {
                return (MetadataObject.Source as TOM.QueryPartitionSource)?.DataSource.Name;
            }
        }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Query { get { return Expression; } set { Expression = value; } }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Expression
        {
            get
            {
                switch(MetadataObject.SourceType)
                {
                    case TOM.PartitionSourceType.Calculated:
                        return (MetadataObject.Source as TOM.CalculatedPartitionSource)?.Expression;
                    case TOM.PartitionSourceType.Query:
                        return (MetadataObject.Source as TOM.QueryPartitionSource)?.Query;
#if CL1400
                    case TOM.PartitionSourceType.M:
                        return (MetadataObject.Source as TOM.MPartitionSource)?.Expression;
#endif
                }
                throw new NotSupportedException();
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

                    switch (MetadataObject.SourceType)
                    {
                        case TOM.PartitionSourceType.Calculated:
                            (MetadataObject.Source as TOM.CalculatedPartitionSource).Expression = value; break;
                        case TOM.PartitionSourceType.Query:
                            (MetadataObject.Source as TOM.QueryPartitionSource).Query = value; break;
#if CL1400
                        case TOM.PartitionSourceType.M:
                            (MetadataObject.Source as TOM.MPartitionSource).Expression = value; break;
#endif
                    }
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
#if CL1400
                    return SourceType == TOM.PartitionSourceType.Calculated || SourceType == TOM.PartitionSourceType.M;
#else
                    return SourceType == TOM.PartitionSourceType.Calculated;
#endif
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
            get
            {
                return base.Name;
            }
        }

        public bool Editable(string propertyName)
        {
            switch(propertyName)
            {
                case "Name":
                case "Description":
                case "DataSource":
                case "Query":
                case "Expression":
                    return true;
                default:
                    return false;
            }
        }
    }

#if CL1400
    public class MPartition: Partition
    {
        public MPartition() : base(new TOM.Partition() { Source = new TOM.MPartitionSource() })
        {

        }
    }
#endif
}
