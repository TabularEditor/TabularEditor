using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class NamedExpression: IExpressionObject
    {
        public bool NeedsValidation { get { return false; } private set { } }
    }
}
