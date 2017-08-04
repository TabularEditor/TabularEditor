using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
#if CL1400
    public partial class Variation
    {
        public bool IsBrowsable(string propertyName)
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

        public bool IsEditable(string propertyName)
        {
            return true;
        }
    }
#endif
}
