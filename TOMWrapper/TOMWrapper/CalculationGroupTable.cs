using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
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

        internal string CalculationItemErrors { get; private set; } = null;

        internal void AddError(CalculationItem calculationItem)
        {
            if(ErrorMessage == null) ErrorMessage = $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";
            else ErrorMessage += $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";

            if (CalculationItemErrors == null) CalculationItemErrors = $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";
            else CalculationItemErrors += $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";
        }

        internal override void ClearError()
        {
            CalculationItemErrors = null;
            base.ClearError();
        }

        internal override void PropagateChildErrors()
        {
            foreach (var calcItem in CalculationItems) if (!string.IsNullOrEmpty(calcItem.ErrorMessage)) AddError(calcItem);
            base.PropagateChildErrors();
        }

        internal new static CalculationGroupTable CreateFromMetadata(Model parent, TOM.Table metadataObject)
        {
            if (metadataObject.GetSourceType() != TOM.PartitionSourceType.CalculationGroup)
                throw new ArgumentException("Provided metadataObject is not a Calculation Group Table.");
            var obj = new CalculationGroupTable(metadataObject);
            parent.Tables.Add(obj);

            obj.Init();
            obj.Field = new CalculationGroupAttribute(obj.DataColumns.First());

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

        [Browsable(false)]
        public CalculationGroupAttribute Field { get; private set; }

        CalculationGroupTable(TOM.Table table) : base(table)
        {

        }

        internal override ITabularObjectCollection GetCollectionForChild(TabularObject child)
        {
            if(child is CalculationItem) return CalculationGroup.CalculationItems;
            if (child is CalculationGroup) return null;
            return base.GetCollectionForChild(child);
        }
        
        public CalculationGroup CalculationGroup { get; private set; }

        [Category("Calculation Group"),DisplayName("Precedence"),Description("When multiple Calculation Groups are used as a filter condition, this property determines the order of evaluation.")]
        public int CalculationGroupPrecedence { get { return CalculationGroup.Precedence; } set { CalculationGroup.Precedence = value; } }

        [Category("Calculation Group"),DisplayName("Description"),Description("The description of the Calculation Group object.")]
        public string CalculationGroupDescription { get { return CalculationGroup.Description; } set { CalculationGroup.Description = value; } }

        [Category("Calculation Group"),DisplayName("Annotations"),Description("Annotations on the Calculation Group object."),NoMultiselect,Editor(typeof(AnnotationCollectionEditor), typeof(UITypeEditor))]
        public AnnotationCollection CalculationGroupAnnotations { get { return CalculationGroup.Annotations; } }

        protected override void Init()
        {
            if (MetadataObject.CalculationGroup != null)
                CalculationGroup = CalculationGroup.CreateFromMetadata(this, MetadataObject.CalculationGroup);
            base.Init();
        }

        internal override void Reinit()
        {
            base.Reinit();
            CalculationGroup.RenewMetadataObject();
            CalculationGroup.Reinit();
            Field = new CalculationGroupAttribute(DataColumns.First());
        }

        public override ObjectType ObjectType => ObjectType.CalculationGroup;

        public CalculationItemCollection CalculationItems => CalculationGroup.CalculationItems;

        public override IEnumerable<ITabularNamedObject> GetChildren()
        {
            yield return Field;
            foreach (var item in CalculatedColumns) yield return item;
            foreach (var item in Measures) yield return item;
            foreach (var item in Hierarchies) yield return item;
        }

        public override IEnumerable<IFolderObject> GetChildrenByFolders()
        {
            return base.GetChildrenByFolders();
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.PRECEDENCE:
                case Properties.CALCULATIONGROUPDESCRIPTION:
                case Properties.CALCULATIONGROUPANNOTATIONS:
                case Properties.CALCULATIONGROUPPRECEDENCE:
                case Properties.NAME:
                case Properties.ISHIDDEN:
                case Properties.DESCRIPTION:
                case Properties.OBJECTTYPENAME:
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

    internal partial class Properties
    {
        public const string CALCULATIONGROUPDESCRIPTION = "CalculationGroupDescription";
        public const string CALCULATIONGROUPANNOTATIONS = "CalculationGroupAnnotations";
        public const string CALCULATIONGROUPPRECEDENCE = "CalculationGroupPrecedence";
    }
}
