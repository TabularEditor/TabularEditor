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

        public void Init(TabularModelHandler handler)
        {
            model = handler.Model;
            form = new VisualizeRelationshipsForm();
        }

        public void RegisterActions(Action<string, Action> registerCallback)
        {
            registerCallback("Visualize Relationships...", ShowForm);
        }

        public void ShowForm()
        {
            form.ShowGraph(model);
        }
    }
}
