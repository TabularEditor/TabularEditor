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
    internal class ClonableObjectCollectionEditor<T> : RefreshGridCollectionEditor where T: class, IClonableObject
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
            var orgObject = propInfo.GetValue(lb.SelectedItem) as T;

            MethodInfo methodInfo = m_collectionForm.GetType().GetMethod("AddItems", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(m_collectionForm, new object[] { Enumerable.Repeat(orgObject.Clone(), 1).ToList() } );

        }

        protected override object CreateInstance(Type itemType)
        {
            if(itemType == typeof(T))
            {
                return typeof(T).GetMethod("CreateNew", new Type[] { typeof(string) }).Invoke(null, new object[] { null });
            }
            return base.CreateInstance(itemType);
        }

        public ClonableObjectCollectionEditor(Type type): base(type)
        {

        }
    }
}
