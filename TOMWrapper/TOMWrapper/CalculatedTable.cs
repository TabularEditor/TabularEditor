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

namespace TabularEditor.TOMWrapper
{
    public class CalculatedTable: Table, IExpressionObject
    {
        [IntelliSense("Adds a new Calculated Table column to the table."), Tests.GenerateTest()]
        public CalculatedTableColumn AddCalculatedTableColumn(string name = null, string sourceColumn = null, string displayFolder = null, DataType dataType = DataType.String)
        {
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(CalculatedTableColumn))) return null;

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

            if (Partitions.Count == 0) {
                // Make sure the calculated table contains at least one partition (but don't add that to the undo stack):
                Handler.UndoManager.Enabled = false;
                Partition.CreateCalculatedTablePartition(this);
                Handler.UndoManager.Enabled = true;
            }

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
            metadataObject.Name = parent.Tables.GetNewName(string.IsNullOrWhiteSpace(name) ? "New Calculated Table" : name);

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
                NeedsValidation = true;
                FormulaFixup.BuildDependencyTree(this);
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        internal override void CheckChildrenErrors()
        {
            base.CheckChildrenErrors();
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
                NeedsValidation = true;
                OnPropertyChanged("Expression", oldValue, value);
            }
        }

        internal override bool Editable(string propertyName)
        {
            return base.Editable(propertyName);
        }

        [Browsable(false)]
        public override bool NeedsValidation { get; set; } = false;
    }
}
