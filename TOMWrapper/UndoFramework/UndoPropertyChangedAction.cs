using System;
using System.Linq;
using System.Reflection;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UndoFramework
{
    public class UndoPropertyChangedAction : IUndoAction
    {
        object oldValue;
        object newValue;
        TabularObject tabularObject;
        Type objectType;
        PropertyInfo prop;
        string index;

        public string GetSummary()
        {
            var name = tabularObject.GetObjectPath();
            if (prop.Name == "Name") name = name.Substring(0, name.Length - newValue.ToString().Length) + oldValue.ToString();
            return string.Format("Changed {{{0}.{1}{4}}}: \"{2}\" --> \"{3}\"", name, prop.Name, oldValue?.ToString(), newValue?.ToString(),
                string.IsNullOrEmpty(index) ? "" : "[" + index + "]");
        }

        public string GetCode()
        {
            string value = null;
            var t = newValue.GetType();
            if (newValue is string) value = StringHelper.ToLiteral(newValue.ToString());
            else if (t.IsPrimitive) value = StringHelper.ToLiteral(newValue);
            else if (t.IsEnum) value = t.Name + "." + t.GetEnumName(newValue);
            else if (newValue is TabularNamedObject) value = ((TabularNamedObject)newValue).GetLinqPath();
            else
                value = newValue.ToString();


            if(tabularObject is TabularNamedObject)
            {
                var path = ((TabularNamedObject)tabularObject).GetLinqPath();
                if (prop.Name == "Name") path = path.Replace("[\"" + newValue.ToString() + "\"]", "[\"" + oldValue.ToString() + "\"]");
                var idx = string.IsNullOrEmpty(index) ? "" : "[\"" + index + "\"]";

                return string.Format("{0}.{1}{2} = {3};", path, prop.Name, idx, value);
            } else
            {
                return "// " + GetSummary();
            }
        }

        public UndoPropertyChangedAction(TabularObject tabularObject, string propertyName, object oldValue, object newValue, string index = null)
        {
            this.tabularObject = tabularObject;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.objectType = tabularObject.GetType();
            this.prop = objectType.GetProperty(propertyName);
            this.index = index;
            this.ActionName = GetActionNameFromProperty(propertyName);
        }

        public static string GetActionNameFromProperty(string propertyName)
        {
            return propertyName.SplitCamelCase().ToLower().Replace("display ", "").Replace(" string", "") + " change";
        }

        public string ActionName { get; private set; }

        public bool TreeStructureChanged
        {
            get
            {
                return false;
            }
        }

        public void Undo()
        {
            if (index == null)
                prop.SetValue(tabularObject, oldValue);
            else
            {
                // Handle indexer properties (only string indexers):
                var c = prop.GetValue(tabularObject);
                var p = c.GetType().GetProperties().FirstOrDefault(pi => pi.GetIndexParameters().FirstOrDefault()?.ParameterType == typeof(string));
                p.SetValue(c, oldValue, new[] { index });
            }
        }

        public void Redo()
        {
            if (index == null)
                prop.SetValue(tabularObject, newValue);
            else
            {
                // Handle indexer properties (only string indexers):
                var c = prop.GetValue(tabularObject);
                var p = c.GetType().GetProperties().FirstOrDefault(pi => pi.GetIndexParameters().FirstOrDefault()?.ParameterType == typeof(string));
                p.SetValue(c, newValue, new[] { index });
            }
        }
    }

}
