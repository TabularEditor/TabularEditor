using System;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public class PolicyRangePartition : Partition
    {
        public override Partition Clone(string newName = null, Table newParent = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(Partition)))
            {
                throw new InvalidOperationException(string.Format(Messages.CannotCreatePowerBIObject, typeof(Partition).GetTypeName()));
            }

            Handler.BeginUpdate("Clone Partition");

            // Create a clone of the underlying metadataobject:
            var tom = MetadataObject.Clone() as TOM.Partition;


            // Assign a new, unique name:
            tom.Name = Parent.Partitions.GetNewName(string.IsNullOrEmpty(newName) ? tom.Name + " copy" : newName);

            // Create the TOM Wrapper object, representing the metadataobject
            PolicyRangePartition obj = CreateFromMetadata(newParent ?? Parent, tom);

            Handler.EndUpdate();

            return obj;
        }

        protected override void Init()
        {
            if (MetadataObject.Source == null && !(Parent is CalculatedTable))
            {
                if (Model.DataSources.Count == 0) StructuredDataSource.CreateNew(Model);
                MetadataObject.Source = new TOM.PolicyRangePartitionSource();
            }
            base.Init();
        }

        protected PolicyRangePartition(TOM.Partition metadataObject) : base(metadataObject)
        {
        }

        public new static PolicyRangePartition CreateNew(Table parent, string name = null)
        {
            if (!parent.Handler.PowerBIGovernance.AllowCreate(typeof(PolicyRangePartition)))
            {
                throw new InvalidOperationException(string.Format(Messages.CannotCreatePowerBIObject, typeof(PolicyRangePartition).GetTypeName()));
            }

            var metadataObject = new TOM.Partition();
            metadataObject.Name = parent.Partitions.GetNewName(string.IsNullOrWhiteSpace(name) ? "New " + typeof(PolicyRangePartition).GetTypeName() : name);
            metadataObject.Source = new TOM.PolicyRangePartitionSource();

            var obj = new PolicyRangePartition(metadataObject);

            parent.Partitions.Add(obj);

            obj.Init();

            return obj;

        }

        internal new static PolicyRangePartition CreateFromMetadata(Table parent, TOM.Partition metadataObject)
        {
            var obj = new PolicyRangePartition(metadataObject);
            parent.Partitions.Add(obj);

            obj.Init();

            return obj;
        }

        private TOM.PolicyRangePartitionSource TomSource => MetadataObject.Source as TOM.PolicyRangePartitionSource;


        [Category("Refresh Policy"), Description("Gets or sets the range start of the refresh policy for this partition"), IntelliSense("Gets or sets the range start of the refresh policy for this partition")]
        public DateTime Start
        {
            get => TomSource.Start;
            set => SetValue(Start, value, v => TomSource.Start = v);
        }

        [Category("Refresh Policy"), Description("Gets or sets the range end of the refresh policy for this partition"), IntelliSense("Gets or sets the range end of the refresh policy for this partition")]
        public DateTime End
        {
            get => TomSource.End;
            set => SetValue(End, value, v => TomSource.End = v);
        }

        [Category("Refresh Policy"), Description("Gets or sets the granularity of the refresh policy for this partition"), IntelliSense("Gets or sets the granularity of the refresh policy for this partition")]
        public RefreshGranularityType Granularity
        {
            get => (RefreshGranularityType)TomSource.Granularity;
            set => SetValue(Granularity, value, v => TomSource.Granularity = (TOM.RefreshGranularityType)v);
        }

        [Category("Refresh Policy"), Description("Gets the refresh bookmark of the refresh policy for this partition"), IntelliSense("Gets the refresh bookmark of the refresh policy for this partition")]
        public string RefreshBookmark
        {
            get => TomSource.RefreshBookmark;
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(DataSource):
                case nameof(End):
                case nameof(Start):
                case nameof(Granularity):
                case nameof(RefreshBookmark):
                    return true;
                case nameof(Expression): return false;
            }
            return base.IsBrowsable(propertyName);
        }

        internal override bool IsEditable(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(DataSource):
                case nameof(End):
                case nameof(Start):
                case nameof(Granularity):
                case nameof(RefreshBookmark):
                    return true;
                case nameof(Expression): return false;
            }
            return base.IsEditable(propertyName);
        }
    }
}