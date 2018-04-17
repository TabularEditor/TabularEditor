using System;
using System.Linq;
using System.Collections.Generic;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Diagnostics;
using System.ComponentModel;
using TabularEditor.TOMWrapper.Undo;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace TabularEditor.TOMWrapper
{
    public partial class Partition: IExpressionObject
    {
        public bool NeedsValidation { get { return false; } set { } }
        internal static void CreateCalculatedTablePartition(CalculatedTable calcTable)
        {
            var tomPartition = new TOM.Partition();
            tomPartition.Name = calcTable.Name;
            var partition = new Partition(tomPartition);

            calcTable.Partitions.Add(partition);
            partition.Init();
            partition.MetadataObject.Source = new TOM.CalculatedPartitionSource();
        }

        protected override void Init()
        {
            if (MetadataObject.Source == null && !(Parent is CalculatedTable))
            {
                if (Model.DataSources.Count(ds => ds is ProviderDataSource) == 0) Model.AddDataSource();
                MetadataObject.Source = new TOM.QueryPartitionSource() { DataSource = Model.DataSources.First(ds => ds is ProviderDataSource).MetadataObject };
            }
            base.Init();
        }

        [Category("Basic"),Description("The query which is executed on the Data Source to populate this partition with data.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), IntelliSense("Gets or sets the query which is executed on the Data Source to populate the partition with data.")]
        public string Query { get { return Expression; } set { Expression = value; } }

        [Category("Expression"), Description("The expression which is used to populate this partition with data.")]
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
                    case TOM.PartitionSourceType.M:
                        return (MetadataObject.Source as TOM.MPartitionSource)?.Expression;
                }
                throw new NotSupportedException();
            }
            set
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
                    case TOM.PartitionSourceType.M:
                        (MetadataObject.Source as TOM.MPartitionSource).Expression = value; break;
                    default:
                        throw new NotSupportedException();
                }

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "Expression", oldValue, value));
                OnPropertyChanged("Expression", oldValue, value);
            }
        }

        [Category("Other"),DisplayName("Data Source"),Description("The Data Source used by this partition."),TypeConverter(typeof(DataSourceConverter))]
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

        protected override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case "DataSource":
                case "Query":
                    return SourceType == PartitionSourceType.Query;
                case "Expression":
                    return SourceType == PartitionSourceType.Calculated || SourceType == PartitionSourceType.M;
                case "Mode":
                case "DataView":
                case "Description":
                case "Name":
                case "RefreshedTime":
                case "ObjectTypeName":
                case "State":
                case "SourceType":
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

        protected override bool AllowDelete(out string message)
        {
            if(Table.Partitions.Count == 1)
            {
                message = Messages.TableMustHaveAtLeastOnePartition;
                return false;
            }
            return base.AllowDelete(out message);
        }

        protected override bool IsEditable(string propertyName)
        {
            switch(propertyName)
            {
                case "Name":
                case "Description":
                case "DataSource":
                case "Query":
                case "Expression":
                case "Mode":
                case "DataView":
                    return true;
                default:
                    return false;
            }
        }
    }

    public class MPartition: Partition
    {

        protected override void Init()
        {
            if (MetadataObject.Source == null && !(Parent is CalculatedTable))
            {
                if (Model.DataSources.Count == 0) StructuredDataSource.CreateNew(Model);
                MetadataObject.Source = new TOM.MPartitionSource();
            }
            base.Init();
        }

        protected MPartition(TOM.Partition metadataObject) : base(metadataObject)
        {
            metadataObject.Source = new TOM.MPartitionSource();
        }

        public new static MPartition CreateNew(Table parent, string name = null)
        {
            if (TabularModelHandler.Singleton.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(MPartition)))
            {
                throw new InvalidOperationException(string.Format(Messages.CannotCreatePowerBIObject, typeof(MPartition).GetTypeName()));
            }

            var metadataObject = new TOM.Partition();
            metadataObject.Name = parent.Partitions.GetNewName(string.IsNullOrWhiteSpace(name) ? "New " + typeof(MPartition).GetTypeName() : name);

            var obj = new MPartition(metadataObject);

            parent.Partitions.Add(obj);

            obj.Init();

            return obj;

        }

        internal new static MPartition CreateFromMetadata(Table parent, TOM.Partition metadataObject)
        {
            var obj = new MPartition(metadataObject);
            parent.Partitions.Add(obj);

            obj.Init();

            return obj;
        }
    }
}
