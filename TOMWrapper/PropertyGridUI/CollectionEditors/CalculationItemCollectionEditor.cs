using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.PropertyGridUI
{
    internal class CalculationItemCollectionEditor: ClonableObjectCollectionEditor<CalculationItem>
    {
        public CalculationItemCollectionEditor(Type type) : base(type)
        {

        }

        CalculationGroup calculationGroup;

        protected override object[] GetItems(object editValue)
        {
            calculationGroup = (editValue as CalculationItemCollection).CalculationGroup;
            return base.GetItems(editValue);
        }

        protected override object CreateInstance(Type itemType)
        {
            return CalculationItem.CreateNew(calculationGroup);
        }
    }
}
