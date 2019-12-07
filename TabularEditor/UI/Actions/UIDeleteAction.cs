using Aga.Controls.Tree;
using Crad.Windows.Forms.Actions;
using System;
using System.Linq;
using TabularEditor.TOMWrapper;
using TabularEditor.TreeViewAdvExtension;
using System.Windows.Forms;

namespace TabularEditor.UI
{
    // TODO: Do we have duplicated Delete logic? The ModelActionManager also defines an action for deleting objects.
    // Maybe this entire class can be removed.

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
                if (Selected.Count == 1 && Selected.Context.HasX(Context.DataObjects))
                {
                    var d = (Selected.FirstOrDefault() as IDaxObject);
                    if (d != null && d.ReferencedBy.Count > 0)
                    {
                        var dependent = d.ReferencedBy.First();
                        refs = "\n\nThis object is directly referenced in the DAX expression on " + (dependent as IDaxObject)?.DaxObjectFullName ?? dependent.GetName();
                        if (d.ReferencedBy.Count > 1) refs += string.Format(" and {0} other object{1}.", d.ReferencedBy.Count - 1, d.ReferencedBy.Count == 2 ? "" : "s");
                    } else
                    {
                        refs = "\n\nThis object does not appear to be referenced in DAX expressions of other objects.";
                    }
                }

                if(Selected.Context.HasX(Context.DataSource))
                {
                    if(Selected.DataSources.Any(ds => ds.UsedByPartitions.Any()))
                    {
                        MessageBox.Show(string.Format("The selected data source{0} are used by one or more tables. Delete the tables before deleting the data source.", Selected.Count > 1 ? "s" : ""),
                            "Object in use", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
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
                    Enabled = UIController.Current.Selection.Any(obj => obj.CanDelete());
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