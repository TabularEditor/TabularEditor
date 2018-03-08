using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace VisualizeRelationships
{
    public class VisualizeRelationshipsPlugin : ITabularEditorPlugin
    {
        Model model;
        VisualizeRelationshipsForm form;
        TabularModelHandler Handler;

        public void Init(TabularModelHandler handler)
        {
            if (form != null && form.Visible) form.Close();

            if (Handler != null)
            {
                Handler.ObjectDeleted -= Handler_ObjectDeleted;
                Handler.Tree.UpdateComplete -= Tree_UpdateComplete;
            }
            Handler = handler;
            Handler.ObjectDeleted += Handler_ObjectDeleted;
            Handler.Tree.UpdateComplete += Tree_UpdateComplete;

            model = Handler.Model;
            form = new VisualizeRelationshipsForm();
            form.Model = model;
        }

        private void Tree_UpdateComplete(object sender, EventArgs e)
        {
            if (form != null && form.Visible) form.Refresh();
        }

        private void Handler_ObjectDeleted(object sender, ObjectDeletedEventArgs e)
        {
            if(e.TabularObject is Table)
            {
                foreach (var d in form.AllDiagrams) d.Remove(e.TabularObject as Table);
            }
            else if (e.TabularObject is Relationship)
            {
                foreach (var d in form.AllDiagrams) d.Remove(e.TabularObject as Relationship);
            }
        }

        public void RegisterActions(Action<string, Action> registerCallback)
        {
            registerCallback("Visualize Relationships...", ShowForm);
        }

        public void ShowForm()
        {
            if(form != null && Handler != null) form.Show();
        }
    }
}
