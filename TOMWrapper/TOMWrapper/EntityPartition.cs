using System;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public class EntityPartition : Partition
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
            EntityPartition obj = CreateFromMetadata(newParent ?? Parent, tom);

            Handler.EndUpdate();

            return obj;
        }

        protected override void Init()
        {
            if (MetadataObject.Source == null && !(Parent is CalculatedTable))
            {
                if (Model.DataSources.Count == 0) StructuredDataSource.CreateNew(Model);
                MetadataObject.Source = new TOM.EntityPartitionSource();
            }
            base.Init();
        }

        protected EntityPartition(TOM.Partition metadataObject) : base(metadataObject)
        {
        }

        public new static EntityPartition CreateNew(Table parent, string name = null)
        {
            if (!parent.Handler.PowerBIGovernance.AllowCreate(typeof(EntityPartition)))
            {
                throw new InvalidOperationException(string.Format(Messages.CannotCreatePowerBIObject, typeof(EntityPartition).GetTypeName()));
            }

            var metadataObject = new TOM.Partition();
            metadataObject.Name = parent.Partitions.GetNewName(string.IsNullOrWhiteSpace(name) ? "New " + typeof(EntityPartition).GetTypeName() : name);
            metadataObject.Source = new TOM.EntityPartitionSource();

            var obj = new EntityPartition(metadataObject);

            parent.Partitions.Add(obj);

            obj.Init();

            return obj;

        }

        internal new static EntityPartition CreateFromMetadata(Table parent, TOM.Partition metadataObject)
        {
            var obj = new EntityPartition(metadataObject);
            parent.Partitions.Add(obj);

            obj.Init();

            return obj;
        }


        [Category("Options"), DisplayName("Expression Source"), Description("The Expression Source used by this partition."), TypeConverter(typeof(NamedExpressionConverter)), IntelliSense("The Expression Source used by this partition.")]
        public NamedExpression ExpressionSource
        {
            get
            {
                var tomExpressionSource = (MetadataObject.Source as TOM.EntityPartitionSource).ExpressionSource;
                if (tomExpressionSource != null) return Handler.WrapperLookup[tomExpressionSource] as NamedExpression;
                else return null;
            }
            set
            {
                SetValue(ExpressionSource, value, (v) => (MetadataObject.Source as TOM.EntityPartitionSource).ExpressionSource = v?.MetadataObject);
            }
        }


        [Category("Options"), DisplayName("Entity Name"), Description("Gets or sets the Name of the underlying referenced object used to query and populate current partition."), IntelliSense("Gets or sets the Name of the underlying referenced object used to query and populate current partition.")]
        public string EntityName
        {
            get => (MetadataObject.Source as TOM.EntityPartitionSource).EntityName;
            set => SetValue(EntityName, value, v => (MetadataObject.Source as TOM.EntityPartitionSource).EntityName = v);
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(DataSource):
                case nameof(EntityName):
                case nameof(ExpressionSource):
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
                case nameof(EntityName):
                case nameof(ExpressionSource):
                    return true;
                case nameof(Expression): return false;
            }
            return base.IsEditable(propertyName);
        }
    }
}