using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class SingleColumnRelationship
    {
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", FromColumn.DaxObjectFullName, this.CrossFilteringBehavior == Microsoft.AnalysisServices.Tabular.CrossFilteringBehavior.OneDirection ? "-->" : "<-->",
                ToColumn.DaxObjectFullName);
        }

        public override string Name
        {
            get
            {
                return this.ToString();
            }

            set
            {
                // don't allow changing names of relationships.
            }
        }
    }
}
