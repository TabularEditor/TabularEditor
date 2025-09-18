using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.ComponentModel.Design;

namespace TabularEditor.TOMWrapper
{

    /// <summary>
    /// Calculation Group Tables are special tables that only contain a single column
    /// </summary>
    public class CalculationGroupTable : Table
    {
        /// <summary>
        /// Adds a calculation item with the given name and expression to the calculation group table.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        [IntelliSense("Adds a calculation item with the given name and expression to the calculation group table.")]
        public CalculationItem AddCalculationItem(string name = null, string expression = null)
        {
            var item = CalculationItem.CreateNew(CalculationGroup, string.IsNullOrEmpty(name) ? "New Calculation" : name);
            if (expression != null) item.Expression = expression;
            if (CalculationItems.Any(i => i.Ordinal != -1))
                item.Ordinal = CalculationItems.Max(i => i.Ordinal) + 1;
            return item;
        }

        internal string CalculationItemErrors { get; private set; } = null;

        internal void AddError(CalculationItem calculationItem)
        {
            if(ErrorMessage == null) ErrorMessage = $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";
            else ErrorMessage += $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";

            if (CalculationItemErrors == null) CalculationItemErrors = $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";
            else CalculationItemErrors += $"Error on \"{calculationItem.Name}\": {calculationItem.ErrorMessage}";

            Handler._errors.Add(calculationItem);
        }

        private void AddCalcGroupError(string error)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = string.Empty;
            else
                ErrorMessage += "\n";
            ErrorMessage += error;
        }

        internal override void ClearError()
        {
            CalculationItemErrors = null;
            base.ClearError();

            if (Handler.CompatibilityLevel >= DefaultExpressionRequiredCompatibilityLevel)
            {
                if(MetadataObject.CalculationGroup?.NoSelectionExpression != null)
                {
                    if (!string.IsNullOrEmpty(MetadataObject.CalculationGroup.NoSelectionExpression.ErrorMessage)) AddCalcGroupError("No Selection Expression error: " + MetadataObject.CalculationGroup.NoSelectionExpression.ErrorMessage);
                    if (!string.IsNullOrEmpty(MetadataObject.CalculationGroup.NoSelectionExpression.FormatStringDefinition?.ErrorMessage)) AddCalcGroupError("No Selection Format String Expression error: " + MetadataObject.CalculationGroup.NoSelectionExpression.FormatStringDefinition.ErrorMessage);
                }
                if (MetadataObject.CalculationGroup?.MultipleOrEmptySelectionExpression != null)
                {
                    if (!string.IsNullOrEmpty(MetadataObject.CalculationGroup.MultipleOrEmptySelectionExpression.ErrorMessage)) AddCalcGroupError("Multiple Selection Expression error: " + MetadataObject.CalculationGroup.MultipleOrEmptySelectionExpression.ErrorMessage);
                    if (!string.IsNullOrEmpty(MetadataObject.CalculationGroup.MultipleOrEmptySelectionExpression.FormatStringDefinition?.ErrorMessage)) AddCalcGroupError("Multiple Selection Format String Expression error: " + MetadataObject.CalculationGroup.MultipleOrEmptySelectionExpression.FormatStringDefinition.ErrorMessage);
                }
            }
        }

        internal bool DisableReordering = false;
        private bool _reordering = false;
        /// <summary>
        /// Set to true, when multiple calculation items are going to be re-ordered as one action.
        /// </summary>
        [Browsable(false)]
        public bool Reordering
        {
            get
            {
                return _reordering;
            }
            internal set
            {
                if (value)
                {
                    if (_reordering) throw new InvalidOperationException("Re-ordering is already in progress.");
                    originalOrder = CalculationItems.OrderBy(l => l.Ordinal).ToList();
                }
                else
                {
                    if (!_reordering) throw new InvalidOperationException("No re-ordering is currently happening.");
                    CompactLevelOrdinals();
                    var newOrder = CalculationItems.OrderBy(l => l.Ordinal).ToList();
                    Handler.UndoManager.Add(new UndoCalculationItemsOrderAction(this, originalOrder, newOrder));
                    Handler.Tree.OnStructureChanged(this);
                }
                _reordering = value;
            }
        }

        private List<CalculationItem> originalOrder;

        public void CompactLevelOrdinals()
        {
            var ordinal = 0;
            foreach (var l in CalculationItems.OrderBy(l => l.Ordinal))
            {
                l.MetadataObject.Ordinal = ordinal;
                ordinal++;
            }
        }

        public void FixItemOrder(CalculationItem item, int newOrdinal)
        {
            if (_reordering) return;

            var before = CalculationItems.OrderBy(l => l.Ordinal).ToList();

            var ordinal = 0;
            foreach (var l in CalculationItems.OrderBy(l => (l == item ? newOrdinal : l.Ordinal) * 2 - (l == item ? 1 : 0)))
            {
                l.MetadataObject.Ordinal = newOrdinal == -1 ? -1 : ordinal;
                ordinal++;
            }

            var after = CalculationItems.OrderBy(l => l.Ordinal).ToList();

            Handler.UndoManager.Add(new UndoCalculationItemsOrderAction(this, before, newOrdinal == -1 ? null : after));

            _reordering = false;
            Handler.Tree.OnStructureChanged(this);
        }

        public void SetLevelOrder(IList<CalculationItem> order)
        {
            if (order == null)
            {
                foreach (var item in CalculationItems) item.MetadataObject.Ordinal = -1;
            }
            else
            {
                if (order.Count != CalculationItems.Count) throw new ArgumentException("Cannot order a hierarchy by a list that does not contain exactly the same levels as the hierarchy iteself.");

                for (var i = 0; i < CalculationItems.Count; i++)
                {
                    if (!CalculationItems.Contains(order[i])) throw new ArgumentException("Cannot order a hierarchy by levels in another hierarchy.");
                    order[i].MetadataObject.Ordinal = i;
                }
            }

            Handler.Tree.OnStructureChanged(this);
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

            // Generate a new LineageTag if an object with the provided lineage tag already exists:
            if (!string.IsNullOrEmpty(metadataObject.LineageTag))
            {
                if (parent.Handler.CompatibilityLevel < 1540) metadataObject.LineageTag = null;
                else if (parent.MetadataObject.Tables.FindByLineageTag(metadataObject.LineageTag) != metadataObject)
                {
                    metadataObject.LineageTag = Guid.NewGuid().ToString();
                }
            }

            var obj = new CalculationGroupTable(metadataObject);
            parent.Tables.Add(obj);

            obj.Init();
            //obj.NameField = new CalculationGroupAttribute(obj.DataColumns.First());

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
            if (parent.Model.Database.CompatibilityLevel >= 1540) metadataObject.LineageTag = Guid.NewGuid().ToString();
            var nameCol = new TOM.DataColumn { Name = "Name", DataType = TOM.DataType.String, SourceColumn = "Name" };
            var ordinalCol = new TOM.DataColumn { Name = "Ordinal", DataType = TOM.DataType.Int64, IsHidden = true, SourceColumn = "Ordinal" };
            metadataObject.Columns.Add(nameCol);
            metadataObject.Columns.Add(ordinalCol);
            nameCol.SortByColumn = ordinalCol;
            metadataObject.Partitions.Add(new TOM.Partition { Source = new TOM.CalculationGroupSource(), Mode = TOM.ModeType.Import });
            metadataObject.Name = parent.Tables.GetNewName(string.IsNullOrWhiteSpace(name) ? "New Calculation Group" : name);

            return CreateFromMetadata(parent, metadataObject);
        }

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

        [Category("Basic"),DisplayName("Calculation Group Precedence"),Description("When multiple Calculation Groups are used as a filter condition, this property determines the order of evaluation.")]
        public int CalculationGroupPrecedence { get { return CalculationGroup.Precedence; } set { CalculationGroup.Precedence = value; } }

        [Category("Basic"),DisplayName("Calculation Group Description"),Description("The description of the Calculation Group object.")]
        public string CalculationGroupDescription { get { return CalculationGroup.Description; } set { CalculationGroup.Description = value; } }

        [Category("Metadata"),DisplayName("Calculation Group Annotations"),Description("Annotations on the Calculation Group object."),NoMultiselect,Editor(typeof(AnnotationCollectionEditor), typeof(UITypeEditor))]
        public AnnotationCollection CalculationGroupAnnotations { get { return CalculationGroup.Annotations; } }

        /// <summary>
        /// The expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are aplied.
        /// </summary>
        [Category("Options"),DisplayName("Multiple or Empty Selection Expression"),Description("The expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are applied."),Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MultipleOrEmptySelectionExpression
        {
            get => CalculationGroup.MultipleOrEmptySelectionExpression;
            set => CalculationGroup.MultipleOrEmptySelectionExpression = value;
        }

        /// <summary>
        /// The format string expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are aplied.
        /// </summary>
        [Category("Options"), DisplayName("Multiple or Empty Selection Format String Expression"), Description("The format string expression defined on this object will be applied to the selected measure in DAX queries, when multiple calculation items are applied."), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MultipleOrEmptySelectionFormatStringExpression
        {
            get => CalculationGroup.MultipleOrEmptySelectionFormatStringExpression;
            set => CalculationGroup.MultipleOrEmptySelectionFormatStringExpression = value;
        }

        /// <summary>
        /// The description of the CalculationGroupExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
        /// </summary>
        [Category("Options"), DisplayName("Multiple or Empty Selection Expression Description"), Description("The description of the CalculationGroupExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.")]
        public string MultipleOrEmptySelectionDescription
        {
            get => CalculationGroup.MultipleOrEmptySelectionDescription;
            set => CalculationGroup.MultipleOrEmptySelectionDescription = value;
        }
        /// <summary>
        /// The expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items are applied.
        /// </summary>
        [Category("Options"), DisplayName("No-selection Expression"), Description("The expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items are applied."), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string NoSelectionExpression
        {
            get => CalculationGroup.NoSelectionExpression;
            set => CalculationGroup.NoSelectionExpression = value;
        }

        /// <summary>
        /// The format string expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items are applied.
        /// </summary>
        [Category("Options"), DisplayName("No-selection Format String Expression"), Description("The format string expression defined on this object will be applied to the selected measure in DAX queries, when no calculation items are applied."), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string NoSelectionFormatStringExpression
        {
            get => CalculationGroup.NoSelectionFormatStringExpression;
            set => CalculationGroup.NoSelectionFormatStringExpression = value;
        }

        /// <summary>
        /// The description of the CalculationGroupExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.
        /// </summary>
        [Category("Options"), DisplayName("No-selection Expression Description"), Description("The description of the CalculationGroupExpression, visible to developers at design time and to administrators in management tools, such as SQL Server Management Studio.")]
        public string NoSelectionExpressionDescription
        {
            get => CalculationGroup.NoSelectionExpressionDescription;
            set => CalculationGroup.NoSelectionExpressionDescription = value;
        }

        protected override void Init()
        {
            if (MetadataObject.CalculationGroup != null)
            {
                CalculationGroup = CalculationGroup.CreateFromMetadata(this, MetadataObject.CalculationGroup);
                CalculationGroup.PropertyChanging += (s, e) => RaisePropertyChanging(e.PropertyName);
                CalculationGroup.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
            }
            base.Init();
        }

        internal override void Reinit()
        {
            base.Reinit();
            Handler.WrapperLookup.Remove(CalculationGroup.MetadataObject);
            CalculationGroup.MetadataObject = MetadataObject.CalculationGroup;
            Handler.WrapperLookup.Add(CalculationGroup.MetadataObject, CalculationGroup);

            CalculationGroup.Reinit();
        }

        public override ObjectType ObjectType => ObjectType.CalculationGroupTable;

        public CalculationItemCollection CalculationItems => CalculationGroup.CalculationItems;

        private protected override bool IsBrowsable(string propertyName)
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
                case Properties.DEFAULTDETAILROWSEXPRESSION:
                    return true;
                case nameof(MultipleOrEmptySelectionExpression):
                case nameof(MultipleOrEmptySelectionFormatStringExpression):
                case nameof(MultipleOrEmptySelectionDescription):
                case nameof(NoSelectionExpression):
                case nameof(NoSelectionFormatStringExpression):
                case nameof(NoSelectionExpressionDescription):
                    return Handler.CompatibilityLevel >= DefaultExpressionRequiredCompatibilityLevel;
                default:
                    return false;
            }
        }

        public const int DefaultExpressionRequiredCompatibilityLevel = 1605;
    }

    internal static partial class Properties
    {
        public const string CALCULATIONGROUPDESCRIPTION = "CalculationGroupDescription";
        public const string CALCULATIONGROUPANNOTATIONS = "CalculationGroupAnnotations";
        public const string CALCULATIONGROUPPRECEDENCE = "CalculationGroupPrecedence";
    }
}
