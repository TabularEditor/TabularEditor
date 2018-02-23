using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Variation
    {
        public new bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case "TranslatedNames":
                case "TranslatedDescriptions":
                    return false;
                default:
                    return true;
            }
        }

        public new bool IsEditable(string propertyName)
        {
            return true;
        }

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            DefaultColumn = null;
            DefaultHierarchy = null;
            Relationship = null;
            base.DeleteLinkedObjects(isChildOfDeleted);
        }
    }
}
