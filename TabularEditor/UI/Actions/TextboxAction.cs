using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Actions
{
    public abstract class TextBoxAction: Crad.Windows.Forms.Actions.Action
    {
        public ITextBox ActiveTextBox
        {
            get
            {
                var x = ActionList.ContainerControl as Control;
                while((x as ContainerControl)?.ActiveControl != null)
                {
                    x = (x as ContainerControl).ActiveControl;
                }
                if (x is TextBoxBase) return new TextboxWrapper(x as TextBoxBase);
                if (x is FastColoredTextBoxNS.FastColoredTextBox) return new TextboxWrapper(x as FastColoredTextBoxNS.FastColoredTextBox);
                return null;
            }
        }
    }
}
