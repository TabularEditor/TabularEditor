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
    public class CultureCollectionEditor : RefreshGridCollectionEditor
    {
        public CultureCollectionEditor(Type type) : base(type)
        {
            // TODO: Add "Clone" button
            // - Hook up event handler on the listview, to make sure an item is selected before the Clone button becomes enabled
            // - Hook up event handler on the Clone button, to actually perform the clone.

            //var panel = form.Controls.Find("addRemoveTableLayoutPanel", true).FirstOrDefault() as TableLayoutPanel;
            //panel.ColumnCount = 3;
            //panel.Controls.Add(new Button() { Text = "Clone" });
        }

        protected override object SetItems(object editValue, object[] value)
        {
            return base.SetItems(editValue, value.Cast<Culture>().Where(c => !c.Unassigned).ToArray());
        }
    }
}
