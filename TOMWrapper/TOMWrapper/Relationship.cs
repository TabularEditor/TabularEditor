using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class Relationship
    {
        [Browsable(false)]
        public IEnumerable<Variation> UsedInVariations { get { return Model.AllColumns.SelectMany(c => c.Variations).Where(v => v.Relationship == this); } }

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            // Make sure the relationship is no longer used in any Variations:
            if (Handler.CompatibilityLevel >= 1400)
                UsedInVariations.ToList().ForEach(v => v.Delete());

            base.DeleteLinkedObjects(isChildOfDeleted);
        }
    }
}
