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
    /// A CollectionEditor that automatically refreshes the parent PropertyGrid when the CollectionEditor form is closed.
    /// This ensures that no expanded items (using the DictionaryProperty) still show old (deleted) members after the
    /// form is closed.
    /// </summary>
    public class RefreshGridCollectionEditor : CollectionEditor
    {
        TabularModelHandler handler;
        bool canceled = false;

        public RefreshGridCollectionEditor(Type type) : base(type)
        {
        }

        protected override CollectionForm CreateCollectionForm()
        {
            handler = (Context.Instance as Model)?.Handler ?? (Context.Instance as ITabularObject)?.Model?.Handler;

            CollectionForm form = base.CreateCollectionForm();

            form.Load += Form_Load;
            form.FormClosed += Form_FormClosed;
            
            return form;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            handler?.BeginUpdate(CollectionItemType.Name.ToLower() + " change");
        }

        protected override void CancelChanges()
        {
            canceled = true;
            base.CancelChanges();
            handler?.EndUpdate(rollback: true);
        }

        protected override object SetItems(object editValue, object[] value)
        {
            if (canceled) return editValue;

            //return base.SetItems(editValue, value);

            var col = (editValue as ITabularObjectCollection);

            // Manually add/remove items from the collection, instead of doing the default clear+add:
            var newItems = value.Cast<TabularNamedObject>().Where(i => !col.Contains(i)).ToList();
            var removedItems = col.Cast<TabularNamedObject>().Where(i => !value.Contains(i)).ToList();

            removedItems.ForEach(i => col.Remove(i));
            newItems.ForEach(i => col.Add(i));

            return col;
        }

        protected virtual void OnFormClosed(FormClosedEventArgs e)
        {

        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            PropertyInfo propInfo = Context.GetType().GetProperty("OwnerGrid");
            var grid = propInfo.GetValue(Context) as PropertyGrid;
            
            //grid.Refresh();

            OnFormClosed(e);

            if (!canceled)
            {
                handler?.EndUpdate();
            }
        }
    }
}
