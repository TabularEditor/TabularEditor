using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    /// <summary>
    /// This Collection Editor for Cultures takes care of removing unassigned cultures
    /// when the Collection Editor is closed.
    /// </summary>
    internal class CultureCollectionEditor : ClonableObjectCollectionEditor<Culture>
    {
        public CultureCollectionEditor(Type type) : base(type) { }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Delete unassigned translations:
            TabularModelHandler.Singleton.Model.Cultures.Where(c => c.Unassigned).ToList().ForEach(c => c.Delete());
            base.OnFormClosed(e);
        }
    }
}
