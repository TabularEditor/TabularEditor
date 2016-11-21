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

        public CalculatedTable(Model parent) : base(parent)
        {

        }
        public CalculatedTable(TabularModelHandler handler, TOM.Table tableMetadataObject) : base(handler, tableMetadataObject)
        {

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
