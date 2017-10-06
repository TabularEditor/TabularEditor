using Aga.Controls.Tree;
using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Actions
{
    public abstract class ClipboardAction: Crad.Windows.Forms.Actions.Action
    {
        private UIElements UI { get { return UIController.Current.Elements; } }

        /// <summary>
        /// Returns the currently selected nodes of the main form's TreeView, provided
        /// the main form's TreeView control currently has focus.
        /// </summary>
        public ReadOnlyCollection<TreeNodeAdv> SelectedNodes
        {
            get
            {
                if (Form.ActiveForm == UI.FormMain &&
                    UIHelper.GetFocusedControl(UI.FormMain) == UI.TreeView &&
                    UI.TreeView.SelectedNodes.Count > 0)
                    return UI.TreeView.SelectedNodes;
                return null;
            }
        }

        public ITextBox ActiveTextBox
        {
            get
            {
                var x = UIHelper.GetFocusedControl(ActionList.ContainerControl);
                if (x is TextBoxBase) return new TextboxWrapper(x as TextBoxBase);
                if (x is FastColoredTextBoxNS.FastColoredTextBox) return new TextboxWrapper(x as FastColoredTextBoxNS.FastColoredTextBox);
                return null;
            }
        }
    }
}
