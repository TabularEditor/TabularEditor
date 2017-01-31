using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public class CalculatedTable: Table, IExpressionObject
    {
        public Dictionary<IDaxObject, List<Dependency>> Dependencies { get; private set; } = new Dictionary<IDaxObject, List<Dependency>>();


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
            if (!string.IsNullOrEmpty(Partitions[0].ErrorMessage)) ErrorMessage = Partitions[0].ErrorMessage;
        }

        [DisplayName("Expression")]
        [Category("Options"), IntelliSense("The Expression of this Calculated Table. Read only.")]
        public string Expression
        {
            get
            {
                return Partitions[0].Expression;
            }
            set
            {
                throw new Exception("Expressions for Calculated Tables cannot be set through Tabular Editor.");
            }
        }

        public override bool Editable(string propertyName)
        {
            return propertyName == "Expression" ? false : base.Editable(propertyName);
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
