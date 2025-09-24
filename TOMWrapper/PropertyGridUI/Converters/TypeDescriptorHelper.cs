using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI;

// It's quite tricky to spawn a CollectionEditor from scratch (i.e. without relying on the ProperyGrid infrastructure)
// So we create this helper class that can give us an ITypeDescriptorContext object, which is needed when calling
// Editor.EditValue(...)
static class TypeDescriptorHelper
{
    public static ITypeDescriptorContext GetContext(ITabularNamedObject instance, string propertyName)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

        var host = new DesignerHost();
        var propertyDescriptor = TypeDescriptor.GetProperties(instance).Find(propertyName, true);
        Debug.Assert(propertyDescriptor != null, $"Property '{propertyName}' not found on type '{instance.GetType().FullName}'");
        return new CustomContext(instance, propertyDescriptor, host);
    }

    private sealed class CustomContext(object instance, PropertyDescriptor propertyDescriptor, IDesignerHost host) : ITypeDescriptorContext
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IWindowsFormsEditorService))
            {
                // Provide a modal editor service for showing dialogs
                return new SimpleEditorService();
            }
            if (serviceType == typeof(IDesignerHost))
            {
                // Provide the designer host service
                return host;
            }
            return null;
        }
        
        public void OnComponentChanged() { }
        public bool OnComponentChanging() => true;

        public IContainer Container => null;
        public object Instance { get; } = instance ?? throw new ArgumentNullException(nameof(instance));
        public PropertyDescriptor PropertyDescriptor { get; } = propertyDescriptor ?? throw new ArgumentNullException(nameof(propertyDescriptor));
    }

    // Provides IWindowsFormsEditorService so the editor can show a dialog
    private sealed class SimpleEditorService : IWindowsFormsEditorService, IServiceProvider
    {
        public DialogResult ShowDialog(Form dialog)
        {
            return dialog.ShowDialog();
        }

        public void DropDownControl(Control control)
        {
            // For dropdown editors (not typically used with CollectionEditor)
        }

        public void CloseDropDown()
        {
            // For dropdown editors
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IWindowsFormsEditorService))
                return this;
            return null;
        }
    }
}
