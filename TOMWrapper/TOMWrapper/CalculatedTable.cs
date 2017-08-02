using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public class CalculatedTable: Table, IDAXExpressionObject
    {
        [Browsable(false)]
        public Dictionary<IDaxObject, List<Dependency>> Dependencies { get; } = new Dictionary<IDaxObject, List<Dependency>>();

        protected override void Init()
        {
            base.Init();

            if (Partitions.Count == 0) {
                // Make sure the calculated table contains at least one partition:
                var p = Partition.CreateNew(this, Name);
                p.MetadataObject.Source = new TOM.CalculatedPartitionSource();
            }

            Partitions[0].PropertyChanged += Partition_PropertyChanged;
        }

        protected override bool IsBrowsable(string propertyName)
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
            metadataObject.Name = parent.Tables.GetNewName(string.IsNullOrWhiteSpace(name) ? "New Table" : name);

            var obj = new CalculatedTable(metadataObject);
            parent.Tables.Add(obj);
            obj.Init();

            if (!string.IsNullOrWhiteSpace(expression)) obj.Expression = expression;

            return obj;
        }

        public static CalculatedTable CreateFromMetadata(TOM.Table metadataObject)
        {
            var obj = new CalculatedTable(metadataObject);
            obj.Init();
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

        public CalculatedTable(TOM.Table tableMetadataObject) : base(tableMetadataObject)
        {
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == Properties.EXPRESSION)
            {
                NeedsValidation = true;
                Handler.BuildDependencyTree(this);
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        public override void CheckChildrenErrors()
        {
            base.CheckChildrenErrors();
            if (Partitions.Count > 0 && !string.IsNullOrEmpty(Partitions[0].ErrorMessage)) ErrorMessage = Partitions[0].ErrorMessage;
        }

        [DisplayName("Expression")]
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

        public override bool Editable(string propertyName)
        {
            return base.Editable(propertyName);
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;

        public override string ObjectTypeName
        {
            get
            {
                return "Calculated Table";
            }
        }
    }
}
