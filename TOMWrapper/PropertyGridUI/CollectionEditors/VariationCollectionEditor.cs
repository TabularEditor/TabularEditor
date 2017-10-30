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
    internal class VariationCollectionEditor: ClonableObjectCollectionEditor<Partition>
    {
        public VariationCollectionEditor(Type type) : base(type)
        {

        }

        Column column;

        protected override object[] GetItems(object editValue)
        {
            column = (editValue as VariationCollection).Column;
            return base.GetItems(editValue);
        }

        protected override object CreateInstance(Type itemType)
        {
            if(itemType == typeof(Variation)) {
                return Variation.CreateNew(column);
            }
            return base.CreateInstance(itemType);
        }
    }
}
