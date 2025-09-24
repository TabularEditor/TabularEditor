using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.PropertyGridUI
{
    /// <summary>
    /// A CollectionEditor that automatically refreshes the parent PropertyGrid when the CollectionEditor form is closed.
    /// This ensures that no expanded items (using the DictionaryProperty) still show old (deleted) members after the
    /// form is closed.
    /// </summary>
    internal class RefreshGridCollectionEditor : CollectionEditor
    {
        TabularModelHandler handler;
        bool cancelled = false;
        protected internal bool Cancelled => cancelled;
        Type itemType = null;
        protected List<object> CurrentItems { get; private set; }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var providerWrapper = new ServiceProviderWrapper(provider, this);
            return base.EditValue(context, providerWrapper, value);
        }

        public RefreshGridCollectionEditor(Type type) : base(typeof(IList))
        {
            while(type != null)
            {
                var genericEnumerableType = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if(genericEnumerableType != null)
                {
                    itemType = genericEnumerableType.GetGenericArguments()[0];
                    break;
                }
                type = type.BaseType;
            }

            if (itemType == null)
                throw new NotSupportedException("This collection editor can only work with collections that derive from IEnumerable<T>.");
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
            cancelled = false;
            handler?.BeginUpdate(CollectionItemType.Name.ToLower() + " change");
        }

        protected override void CancelChanges()
        {
            cancelled = true;
            base.CancelChanges();
            handler?.EndUpdate(rollback: true);
        }

        protected override object SetItems(object editValue, object[] value)
        {
            if (cancelled) return editValue;

            //return base.SetItems(editValue, value);

            var col = (editValue as ITabularObjectCollection);

            // Manually add/remove items from the collection, instead of doing the default clear+add:
            var newItems = value.Cast<TabularNamedObject>().Where(i => !col.Contains(i)).ToList();
            var removedItems = col.Cast<TabularNamedObject>().Where(i => !value.Contains(i)).ToList();

            removedItems.ForEach(i => col.Remove(i));
            newItems.ForEach(i => col.Add(i));

            return col;
        }

        protected override void DestroyInstance(object instance)
        {
            this.CurrentItems?.Remove(instance);
            base.DestroyInstance(instance);
        }

        protected override object CreateInstance(Type itemType)
        {
            var instance = CreateCustomInstance(itemType);
            this.CurrentItems?.Add(instance);
            return instance;
        }

        protected virtual object CreateCustomInstance(Type itemType)
        {
            return base.CreateInstance(itemType);
        }

        protected override object[] GetItems(object editValue)
        {
            var items = (editValue as ITabularObjectCollection).Cast<object>();
            this.CurrentItems = items.ToList();
            return items.ToArray();
        }

        protected override Type CreateCollectionItemType()
        {
            return itemType;
        }

        protected virtual void OnFormClosed(FormClosedEventArgs e)
        {
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            //PropertyInfo propInfo = Context.GetType().GetProperty("OwnerGrid");
            //var grid = propInfo.GetValue(Context) as PropertyGrid;            
            //grid.Refresh();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                CancelChanges();
            }

            OnFormClosed(e);

            if (!cancelled)
            {
                handler?.EndUpdate();
            }
        }
    }

    class EditorServiceWrapper(IWindowsFormsEditorService baseEditorService, RefreshGridCollectionEditor collectionEditor): IWindowsFormsEditorService
    {
        public void CloseDropDown()
        {
            baseEditorService.CloseDropDown();
        }
        public void DropDownControl(Control control)
        {
            baseEditorService.DropDownControl(control);
        }
        public DialogResult ShowDialog(Form dialog)
        {
            // The default behavior of .ShowDialog is to return "DialogResult.Cancel", even if the user hit the "OK" button,
            // when no properties on the edited object were changed. We want to override this behavior, because we also
            // need to deal with read-only properties which may themselves be objects that could be modified.
            var result = baseEditorService.ShowDialog(dialog);
            if (collectionEditor.Cancelled) return DialogResult.Cancel;
            return DialogResult.OK;
        }
    }

    class ServiceProviderWrapper(IServiceProvider baseServiceProvider, RefreshGridCollectionEditor collectionEditor): IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IWindowsFormsEditorService))
            {
                return new EditorServiceWrapper(baseServiceProvider.GetService(serviceType) as IWindowsFormsEditorService, collectionEditor);
            }
            return baseServiceProvider.GetService(serviceType);
        }
    }
}
