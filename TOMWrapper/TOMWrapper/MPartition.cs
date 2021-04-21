using System;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace TabularEditor.TOMWrapper
{
    public class MPartition: Partition
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
            MPartition obj = CreateFromMetadata(newParent ?? Parent, tom);

            Handler.EndUpdate();

            return obj;
        }

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
        }

        public new static MPartition CreateNew(Table parent, string name = null)
        {
            if (!parent.Handler.PowerBIGovernance.AllowCreate(typeof(MPartition)))
            {
                throw new InvalidOperationException(string.Format(Messages.CannotCreatePowerBIObject, typeof(MPartition).GetTypeName()));
            }

            var metadataObject = new TOM.Partition();
            metadataObject.Name = parent.Partitions.GetNewName(string.IsNullOrWhiteSpace(name) ? "New " + typeof(MPartition).GetTypeName() : name);
            metadataObject.Source = new TOM.MPartitionSource();

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

        [Category("Basic"), DisplayName("M Expression"), Description("The Power Query (M) Expression used to populate the partition with data.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MExpression { get => Expression; set => Expression = value; }

        [Category("Options"), Description("Gets or sets the M attributes.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Attributes
        {
            get => (MetadataObject.Source as TOM.MPartitionSource)?.Attributes;
            set
            {
                if (!(MetadataObject.Source is TOM.MPartitionSource mPartitionSource)) return;
                SetValue(Attributes, value, (v) => mPartitionSource.Attributes = v);
            }
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(MExpression): return true;
                case nameof(Attributes): return true;
                case nameof(Expression): return false;
            }
            return base.IsBrowsable(propertyName);
        }

        internal override bool IsEditable(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(Attributes): return true;
                case nameof(MExpression): return true;
            }
            return base.IsEditable(propertyName);
        }
    }
}
