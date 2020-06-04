using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class Set
    {
        bool IHideableObject.IsVisible => Table.IsVisible && !this.IsHidden;

        bool IExpressionObject.NeedsValidation => IsExpressionModified;
    }
}
