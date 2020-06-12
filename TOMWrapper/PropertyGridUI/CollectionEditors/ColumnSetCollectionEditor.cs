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

        protected override object AddObject(out bool cancel)
        {
            return CustomEditors.Edit(Context.Instance, Properties.GROUPBYCOLUMNS, (Context.Instance as Column).GroupByColumns, out cancel);
        }
    }

    internal abstract class CancellableAddCollectionEditor : RefreshGridCollectionEditor
    {
        public CancellableAddCollectionEditor(Type type) : base(type) { }

        Delegate orgClickHandler;

        protected override CollectionForm CreateCollectionForm()
        {
            var form = base.CreateCollectionForm();
            var addButton = form.Controls.Find("addButton", true).First() as Button;
            orgClickHandler = DisableEvents(addButton, "Click")[0];
            addButton.Click += OverrideAddButtonClick;
            return form;
        }

        protected abstract object AddObject(out bool cancel);

        private object newObject;
        private void OverrideAddButtonClick(object sender, EventArgs e)
        {
            newObject = AddObject(out bool cancel);
            if (cancel) return;
            orgClickHandler.DynamicInvoke(sender, e);
        }

        protected override object CreateInstance(Type itemType)
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