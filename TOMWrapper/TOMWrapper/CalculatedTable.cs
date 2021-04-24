using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper.PowerBI;

namespace TabularEditor.TOMWrapper
{
    public class CalculatedTable: Table, IExpressionObject
    {
        [IntelliSense("Adds a new Calculated Table column to the table."), Tests.GenerateTest()]
        public CalculatedTableColumn AddCalculatedTableColumn(string name = null, string sourceColumn = null, string displayFolder = null, DataType dataType = DataType.String)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(CalculatedTableColumn)))
                throw new PowerBIGovernanceException("Adding columns to a table in a Power BI model is not supported.");

            Handler.BeginUpdate("add Calculated Table column");
            var column = CalculatedTableColumn.CreateNew(this, name);
            column.DataType = dataType;
            if (!string.IsNullOrEmpty(sourceColumn)) column.SourceColumn = sourceColumn;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }

        internal override void ClearError()
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(MetadataObject.Partitions[0].ErrorMessage))
                errors.Add("Expression: " + MetadataObject.Partitions[0].ErrorMessage);
            if (Handler.CompatibilityLevel >= 1400 && !string.IsNullOrEmpty(MetadataObject.DefaultDetailRowsDefinition?.ErrorMessage))
                errors.Add("Detail rows: " + MetadataObject.DefaultDetailRowsDefinition.ErrorMessage);

            ErrorMessage = errors.Count == 0 ? null : string.Join("\r\n", errors);
        }
        protected override void Init()
        {
            base.Init();

            Partitions[0].PropertyChanged += Partition_PropertyChanged;
        }

        internal override bool IsBrowsable(string propertyName)
        {
            // Calculated Table should not expose all properties that the ancestor Table class has
            // For example, we don't want users to edit the Partitions collection.
            switch(propertyName)
            {
                case "Partitions":
                case "SourceType":
                    return false;
                default:
                    return base.IsBrowsable(propertyName);
            }
        }

        private void Partition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Changes to the Expression property of a CalculatedTables partitions should be re-raised as a
            // property change event on the CalculatedTable itself:
            if (e.PropertyName == Properties.EXPRESSION) OnPropertyChanged(Properties.EXPRESSION, null, this.Expression);
        }

        /// <summary>
        /// Creates a new Calculated Table and adds it to the specified Model.
        /// Also creates the underlying metadataobject and adds it to the TOM tree.
        /// </summary>		
        public static CalculatedTable CreateNew(Model parent, string name = null, string expression = null)
        {
            var metadataObject = new TOM.Table();
            if (parent.Model.Database.CompatibilityLevel >= 1540) metadataObject.LineageTag = Guid.NewGuid().ToString();
            metadataObject.Name = parent.Tables.GetNewName(string.IsNullOrWhiteSpace(name) ? "New Calculated Table" : name);
            var tomPartition = new TOM.Partition();
            tomPartition.Mode = TOM.ModeType.Import;
            tomPartition.Name = metadataObject.Name;
            tomPartition.Source = new TOM.CalculatedPartitionSource();
            metadataObject.Partitions.Add(tomPartition);
            var obj = new CalculatedTable(metadataObject);
            parent.Tables.Add(obj);
            obj.Init();

            if (!string.IsNullOrWhiteSpace(expression)) obj.Expression = expression;

            return obj;
        }
        
        /// <summary>
        /// Creates a new Calculated Table and adds it to the current Model.
        /// Also creates the underlying metadataobject and adds it to the TOM tree.
        /// </summary>
        public static CalculatedTable CreateNew()
        {
            return CreateNew(TabularModelHandler.Singleton.Model);
        }

        internal new static CalculatedTable CreateFromMetadata(Model parent, TOM.Table metadataObject)
        {
            if (metadataObject.GetSourceType() != TOM.PartitionSourceType.Calculated) throw new ArgumentException("Provided metadataObject is not a Calculated Table.");

            // Generate a new LineageTag if an object with the provided lineage tag already exists:
            if (!string.IsNullOrEmpty(metadataObject.LineageTag))
            {
                if (parent.Handler.CompatibilityLevel < 1540) metadataObject.LineageTag = null;
                else if (parent.MetadataObject.Tables.FindByLineageTag(metadataObject.LineageTag) != metadataObject)
                {
                    metadataObject.LineageTag = Guid.NewGuid().ToString();
                }
            }
            var obj = new CalculatedTable(metadataObject);
            parent.Tables.Add(obj);

            obj.Init();

            return obj;
        }

        protected CalculatedTable(TOM.Table tableMetadataObject) : base(tableMetadataObject)
        {
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == Properties.EXPRESSION)
            {
                FormulaFixup.BuildDependencyTree(this);
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        internal override void PropagateChildErrors()
        {
            base.PropagateChildErrors();
            if (Partitions.Count > 0 && !string.IsNullOrEmpty(Partitions[0].ErrorMessage)) ErrorMessage = Partitions[0].ErrorMessage;
        }

        [Browsable(true), DisplayName("Expression")]
        [Category("Options"), IntelliSense("The Expression of this Calculated Table. Read only.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Expression
        {
            get
            {
                return Partitions[0].Expression;
            }
            set
            {
                var oldValue = Partitions[0].Expression;
                if (value == oldValue) return;
                Partitions[0].Expression = value;
                OnPropertyChanged(Properties.EXPRESSION, oldValue, value);
            }
        }

        [Browsable(false)]
        public override bool NeedsValidation => IsExpressionModified;
    }
}
