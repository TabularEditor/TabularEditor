using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    public interface ICustomEditor
    {
        object Edit(object instance, string property, object value, out bool cancel);
    }

    public static class CustomEditors
    {
        private static Dictionary<string, ICustomEditor> editors = new Dictionary<string, ICustomEditor>();

        public static void RegisterEditor(string propertyName, ICustomEditor editor)
        {
            editors.Add(propertyName, editor);
        }

        internal static object Edit(object instance, string property, object value, out bool cancel)
        {
            cancel = true;
            ICustomEditor editor;
            if(editors.TryGetValue(property, out editor))
            {
                return editor.Edit(instance, property, value, out cancel);
            }
            return value;
        }

        internal static bool HasEditorFor(string property)
        {
            return editors.ContainsKey(property);
        }
    }

    internal class CustomDialogEditor: UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return CustomEditors.HasEditorFor(context.PropertyDescriptor.Name) ? UITypeEditorEditStyle.Modal : UITypeEditorEditStyle.None;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            bool cancel;
            var newValue = CustomEditors.Edit(context.Instance, context.PropertyDescriptor.Name, value, out cancel);
            if (cancel) return value;
            else return newValue;
        }
    }
}
