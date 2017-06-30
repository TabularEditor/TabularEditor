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
        CollectionForm m_collectionForm;
        Button cloneButton;
        ListBox lb;

        protected override CollectionForm CreateCollectionForm()
        {
            m_collectionForm = base.CreateCollectionForm();

            // Add "Clone" button:
            var panel = m_collectionForm.Controls.Find("addRemoveTableLayoutPanel", true).FirstOrDefault() as TableLayoutPanel;
            panel.ColumnCount = 3;
            cloneButton = new Button() { Text = "Clone" };
            panel.Controls.Add(cloneButton);

            // Handle Clone button click:
            cloneButton.Click += CloneButton_Click;

            // Handle listbox selection changed (to enable/disable clone button):
            lb = m_collectionForm.Controls.Find("listbox", true).FirstOrDefault() as ListBox;
            lb.SelectedIndexChanged += (s, e) => { cloneButton.Enabled = lb.SelectedIndex >= 0; };

            return m_collectionForm;
        }

        private void CloneButton_Click(object sender, EventArgs e)
        {
            if (lb.SelectedItem == null) return;
            PropertyInfo propInfo = lb.SelectedItem.GetType().GetProperty("Value");
            var orgCulture = propInfo.GetValue(lb.SelectedItem) as Culture;

            MethodInfo methodInfo = m_collectionForm.GetType().GetMethod("AddItems", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(m_collectionForm, new object[] { Enumerable.Repeat(orgCulture.Clone(), 1).ToList() } );

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Delete unassigned translations:
            TabularModelHandler.Singleton.Model.Cultures.Where(c => c.Unassigned).ToList().ForEach(c => c.Delete());
            base.OnFormClosed(e);
        }

        public CultureCollectionEditor(Type type) : base(type)
        {
        }
    }
}
