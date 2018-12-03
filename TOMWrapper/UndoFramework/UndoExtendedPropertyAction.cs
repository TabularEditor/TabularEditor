using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoExtendedPropertyAction : IUndoAction
    {
        IExtendedPropertyObject Obj;
        string name;
        string oldValue;
        string newValue;
        ExtendedPropertyType deletedType;

        public string ActionName
        {
            get
            {
                return "extended property change";
            }
        }

        public string GetCode()
        {
            return "// set extended property on " + (Obj as ITabularNamedObject).Name ?? Obj.GetTypeName();
        }

        public string GetSummary()
        {
            return "set extended property on " + ((Obj as ITabularNamedObject).Name ?? Obj.GetTypeName()) + "." + name + ": " + oldValue + " -> " + newValue;
        }

        public UndoExtendedPropertyAction(IExtendedPropertyObject obj, string extendedPropertyName, string newValue, string oldValue, ExtendedPropertyType deletedType)
        {
            Obj = obj;
            this.name = extendedPropertyName;
            this.newValue = newValue;
            this.oldValue = oldValue;
            this.deletedType = deletedType;
        }

        public void Redo()
        {
            Obj.SetExtendedProperty(name, newValue, deletedType);
        }

        public void Undo()
        {
            Obj.SetExtendedProperty(name, oldValue, deletedType);
        }
    }
}
