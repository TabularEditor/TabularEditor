using Aga.Controls.Tree;
using Crad.Windows.Forms.Actions;
using System;
using System.Linq;
using TabularEditor.TOMWrapper;
using TabularEditor.TreeViewAdvExtension;
using System.Windows.Forms;

namespace TabularEditor.UI
{
    [StandardAction]
    public class UIDeleteAction: UIModelAction
    {
        protected override void OnExecute(EventArgs e)
        {
            var ctr = ActionList.ActiveControl;

            if (ctr is TreeViewAdv)
            {
                if (Selected.Count == 0) return;
                if (MessageBox.Show(string.Format("Are you sure you want to delete {0}?", Selected.Name),
                    "Confirm delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel) return;

                Handler.BeginUpdate("delete " + Selected.Name);
                Selected.Delete();
                Handler.EndUpdate();
            }
            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            var ctr = ActionList.ActiveControl;

            if (ctr is TextBoxBase)
            {
                Text = "Delete";
                Enabled = true;
                Visible = true;
            } else if (ctr is TreeViewAdv)
            {
                var tree = ctr as TreeViewAdv;
                // Update the delete text depending on which type of object(s) is selected:
                if (tree.SelectedNode == null || tree.SelectedNode.Tag is Model)
                {
                    Text = "Delete";
                    Enabled = false;
                    Visible = false;
                }
                else
                {
                    Text = "Delete " + UIController.Current.Selection.Name + "...";
                    Enabled = true;
                    Visible = true;
                }
            }
            else
            {
                Text = "Delete";
                Visible = true;
            }
        }
    }
}