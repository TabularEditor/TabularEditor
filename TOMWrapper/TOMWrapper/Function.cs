using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class Function
    {
        [Browsable(false)]
        public bool IsVisible => !IsHidden;
        [Browsable(false)]
        public bool NeedsValidation { get; set; }
    }
}
