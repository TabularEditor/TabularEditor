using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI
{
    public static class UIHelper
    {
        public static Control GetFocusedControl(Control parent)
        {
            var control = parent as Control;
            var container = control as IContainerControl;
            while (container != null && container.ActiveControl != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }
            return control;
        }
    }
}
