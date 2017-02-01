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

                string refs = "";
                if (Selected.Count == 1)
                {
                    var d = (Selected.FirstOrDefault() as IDaxObject);
                    if (d != null && d.Dependants.Count > 0)
                    {
                        refs = "\n\nThis object is referenced by " + d.Dependants.First().DaxObjectFullName;
                        if (d.Dependants.Count > 1) refs += string.Format(" and {0} other object{1}.", d.Dependants.Count - 1, d.Dependants.Count == 2 ? "" : "s");
                    } else
                    {
                        refs = "\n\nThis object does not appear to be referenced by other objects.";
                    }
                }

                if (MessageBox.Show(string.Format("Are you sure you want to delete {0}?{1}", Selected.Name, refs),
                    "Confirm deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

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