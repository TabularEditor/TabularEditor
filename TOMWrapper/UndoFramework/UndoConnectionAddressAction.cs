using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoConnectionAddressAction : IUndoAction
    {
        StructuredDataSource Obj;
        string addressPropertyName;
        string oldValue;
        string newValue;

        public string ActionName
        {
            get
            {
                return "address property change";
            }
        }

        public string GetCode()
        {
            return "// set address property on " + (Obj as ITabularNamedObject).Name ?? Obj.GetTypeName();
        }

        public string GetSummary()
        {
            return "set address property on " + ((Obj as ITabularNamedObject).Name ?? Obj.GetTypeName()) + "." + addressPropertyName + ": " + oldValue + " -> " + newValue;
        }

        public UndoConnectionAddressAction(StructuredDataSource obj, string addressPropertyName, string newValue, string oldValue)
        {
            Obj = obj;
            this.addressPropertyName = addressPropertyName;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public void Redo()
        {
            Obj.Address[addressPropertyName] = newValue;
        }

        public void Undo()
        {
            Obj.Address[addressPropertyName] = oldValue;
        }
    }
}
