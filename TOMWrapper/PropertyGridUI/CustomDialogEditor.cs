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
    /// <summary>
    /// The CustomEditors static class, can be used to register custom editors that should be used when
    /// a specified property is edited in the PropertyGrid. An editor provided this way, must implement
    /// the ICustomEditor interface.
    /// 
    /// An example of a custom editor is the FormDisplayFolderSelect which displays a hierarchical view
    /// of Display Folders in a table. The editor is used for editing Display Folder strings in the
    /// property grid.
    /// 
    /// Properties that should be editable in a Custom Editors, must be decorated with the Editor attribute
    /// like so:
    /// 
    ///     [Editor(typeof(CustomDialogEditor), typeof(System.Drawing.Design.UITypeEditor))]
    ///     public string FormatString { get; set; }
    /// </summary>
    public interface ICustomEditor
    {
        object Edit(object instance, string property, object value, out bool cancel);
    }

    public static class CustomEditors
    {
        private static Dictionary<string, ICustomEditor> editors = new Dictionary<string, ICustomEditor>();

        public static void RegisterEditor(string propertyName, ICustomEditor editor)
        {
            if (editors.ContainsKey(propertyName)) return;
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
            if (context == null)
                return UITypeEditorEditStyle.Modal;
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
