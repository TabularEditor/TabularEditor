using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoDataSourceOptionsAction : IUndoAction
    {
        StructuredDataSource Obj;
        string optionPropertyName;
        string oldValue;
        string newValue;

        public string ActionName
        {
            get
            {
                return "option property change";
            }
        }

        public string GetCode()
        {
            return "// set option property on " + (Obj as ITabularNamedObject).Name ?? Obj.GetTypeName();
        }

        public string GetSummary()
        {
            return "set option property on " + ((Obj as ITabularNamedObject).Name ?? Obj.GetTypeName()) + "." + optionPropertyName + ": " + oldValue + " -> " + newValue;
        }

        public UndoDataSourceOptionsAction(StructuredDataSource obj, string optionPropertyName, string newValue, string oldValue)
        {
            Obj = obj;
            this.optionPropertyName = optionPropertyName;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public void Redo()
        {
            Obj.Credential[optionPropertyName] = newValue;
        }

        public void Undo()
        {
            Obj.Credential[optionPropertyName] = oldValue;
        }
    }
}
