using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper;
using System.Reflection;

namespace TabularEditor.PropertyGridUI
{
    internal class ColumnSetCollectionEditor : CancellableAddCollectionEditor
    {
        public ColumnSetCollectionEditor(Type type) : base(type) { }

        protected override object AddObject(object[] currentItems, out bool cancel)
        {
            return CustomEditors.Edit(Context.Instance, Properties.GROUPBYCOLUMNS, currentItems, out cancel);
        }
    }

    internal abstract class CancellableAddCollectionEditor : RefreshGridCollectionEditor
    {
        public CancellableAddCollectionEditor(Type type) : base(type) { }

        Delegate orgAddClickHandler;
        Delegate orgRemoveClickHandler;
        CollectionForm form;

        protected override CollectionForm CreateCollectionForm()
        {
            form = base.CreateCollectionForm();
            var addButton = form.Controls.Find("addButton", true).First();
            orgAddClickHandler = DisableEvents(addButton, "Click")[0];
            addButton.Click += OverrideAddButtonClick;

            var removeButton = form.Controls.Find("removeButton", true).First();
            orgRemoveClickHandler = DisableEvents(removeButton, "Click")[0];
            removeButton.Click += OverrideRemoveButtonClick;
            return form;
        }

        protected abstract object AddObject(object[] currentItems, out bool cancel);

        private object newObject;
        private void OverrideAddButtonClick(object sender, EventArgs e)
        {
            var selectedItems = AddObject(CurrentItems.ToArray(), out bool cancel);
            if (cancel) return;
            if (selectedItems is object[] multipleItems)
            {
                foreach (var item in multipleItems)
                {
                    newObject = item;
                    orgAddClickHandler.DynamicInvoke(sender, e);
                }
            }
            else
            {
                newObject = selectedItems;
                orgAddClickHandler.DynamicInvoke(sender, e);
            }
        }

        private void OverrideRemoveButtonClick(object sender, EventArgs e)
        {
            var selectedItems = (form.Controls.Find("listbox", true)[0] as ListBox).SelectedItems;
            if (selectedItems.Count > 0)
            {
                var listItemType = selectedItems[0].GetType();
                var valueProp = listItemType.GetProperty("Value");
                var actualItems = selectedItems.OfType<object>().Select(o => valueProp.GetValue(o)).ToList();
                foreach (var item in actualItems) CurrentItems.Remove(item);
            }

            orgRemoveClickHandler.DynamicInvoke(sender, e);
        }

        protected override object CreateCustomInstance(Type itemType)
        {
            return newObject;
        }

        static Delegate[] DisableEvents(Control ctrl, string eventName)
        {
            PropertyInfo propertyInfo = ctrl.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            EventHandlerList eventHandlerList = propertyInfo.GetValue(ctrl, new object[] { }) as EventHandlerList;
            FieldInfo fieldInfo = typeof(Control).GetField("Event" + eventName, BindingFlags.NonPublic | BindingFlags.Static);

            object eventKey = fieldInfo.GetValue(ctrl);
            var eventHandler = eventHandlerList[eventKey] as Delegate;
            Delegate[] invocationList = eventHandler.GetInvocationList();
            foreach (EventHandler item in invocationList)
            {
                ctrl.GetType().GetEvent(eventName).RemoveEventHandler(ctrl, item);
            }
            return invocationList;

        }
    }

}