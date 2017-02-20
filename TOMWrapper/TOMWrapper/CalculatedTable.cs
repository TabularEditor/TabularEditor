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
    public class CalculatedTable: Table, IExpressionObject
    {
        [Browsable(false)]
        public Dictionary<IDaxObject, List<Dependency>> Dependencies { get; private set; } = new Dictionary<IDaxObject, List<Dependency>>();

        protected override void Init()
        {
            base.Init();

            if (Partitions.Count == 0) {
                var p = new Partition(this);
                p.MetadataObject.Source = new TOM.CalculatedPartitionSource();
            }

            Partitions[0].PropertyChanged += CalculatedTable_PropertyChanged;
        }

        private void CalculatedTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Expression") OnPropertyChanged("Expression", null, this.Expression);
        }

        public CalculatedTable(Model parent) : base(parent)
        {
        }
        public CalculatedTable(TabularModelHandler handler, TOM.Table tableMetadataObject) : base(handler, tableMetadataObject)
        {
            
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == "Expression")
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

        /// <summary>
        /// Call this method after the model is saved to a DB, to check for changed columns (in case of expression changes)
        /// </summary>
        public void ReinitColumns()
        {
            Columns.CollectionChanged -= Children_CollectionChanged;
            Columns = new ColumnCollection(Handler, this.GetObjectPath() + ".Columns", MetadataObject.Columns, this);
            Columns.CollectionChanged += Children_CollectionChanged;
            Handler.UpdateObject(this);
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
