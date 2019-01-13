using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Calculation Group Tables are special tables that only contain a single column
    /// </summary>
    public class CalculationGroupTable : Table
    {
        public CalculationItem AddCalculationItem(string name = null, string expression = null)
        {
            var item = CalculationItem.CreateNew(CalculationGroup, string.IsNullOrEmpty(name) ? "New Calculation" : name);
            if (expression != null) item.Expression = expression;
            return item;
        }

        internal new static CalculationGroupTable CreateFromMetadata(Model parent, TOM.Table metadataObject)
        {
            if (metadataObject.GetSourceType() != TOM.PartitionSourceType.CalculationGroup)
                throw new ArgumentException("Provided metadataObject is not a Calculation Group Table.");
            var obj = new CalculationGroupTable(metadataObject);
            parent.Tables.Add(obj);

            obj.Init();
            obj.Field = obj.Columns[0] as DataColumn;

            return obj;
        }

        /// <summary>
        /// Creates a new Calculated Table and adds it to the current Model.
        /// Also creates the underlying metadataobject and adds it to the TOM tree.
        /// </summary>
        public static CalculationGroupTable CreateNew()
        {
            return CreateNew(TabularModelHandler.Singleton.Model);
        }

        public new static CalculationGroupTable CreateNew(Model parent, string name = null)
        {
            parent.DiscourageImplicitMeasures = true;
            var metadataObject = new TOM.Table() { CalculationGroup = new TOM.CalculationGroup() };
            metadataObject.Columns.Add(new TOM.DataColumn { Name = "Attribute", DataType = TOM.DataType.String });
            metadataObject.Partitions.Add(new TOM.Partition { Source = new TOM.CalculationGroupSource() });
            metadataObject.Name = parent.Tables.GetNewName(string.IsNullOrWhiteSpace(name) ? "New Calculation Group" : name);

            return CreateFromMetadata(parent, metadataObject);
        }

        DataColumn Field { get; set; }

        CalculationGroupTable(TOM.Table table) : base(table)
        {

        }

        internal override ITabularObjectCollection GetCollectionForChild(TabularObject child)
        {
            if(child is CalculationItem) return CalculationGroup.CalculationItems;
            if (child is CalculationGroup) return null;
            return base.GetCollectionForChild(child);
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CalculationGroup CalculationGroup { get; private set; }

        protected override void Init()
        {
            if (MetadataObject.CalculationGroup != null)
                CalculationGroup = CalculationGroup.CreateFromMetadata(this, MetadataObject.CalculationGroup);
            base.Init();
        }

        public override ObjectType ObjectType => ObjectType.CalculationGroup;

        public CalculationItemCollection CalculationItems => CalculationGroup.CalculationItems;

        public override IEnumerable<ITabularNamedObject> GetChildren()
        {
            yield return Field;
            foreach (var item in CalculationItems) yield return item;
        }
        public override IEnumerable<IFolderObject> GetChildrenByFolders()
        {
            throw new NotSupportedException();
        }

        protected override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.CALCULATIONGROUP:
                case Properties.NAME:
                case Properties.ISHIDDEN:
                case Properties.DESCRIPTION:
                case Properties.OBJECTTYPE:
                case Properties.EXTENDEDPROPERTIES:
                case Properties.ANNOTATIONS:
                case Properties.TRANSLATEDNAMES:
                case Properties.TRANSLATEDDESCRIPTIONS:
                case Properties.INPERSPECTIVE:
                    return true;
                default:
                    return false;
            }
        }
    }

    public partial class CalculationGroup
    {
        internal static CalculationGroup CreateFromMetadata(Table parent, TOM.CalculationGroup metadataObject)
        {
            var obj = new CalculationGroup(metadataObject);
            parent.MetadataObject.CalculationGroup = metadataObject;

            obj.Init();

            return obj;
        }
        
        public override string ToString()
        {
            return "Calculation Group";
        }
    }
}
