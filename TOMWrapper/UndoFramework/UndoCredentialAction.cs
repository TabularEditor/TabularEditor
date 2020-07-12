using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoCredentialAction : IUndoAction
    {
        StructuredDataSource Obj;
        string credentialPropertyName;
        string oldValue;
        string newValue;

        public string ActionName
        {
            get
            {
                return "credential property change";
            }
        }

        public string GetCode()
        {
            return "// set credential property on " + (Obj as ITabularNamedObject).Name ?? Obj.GetTypeName();
        }

        public string GetSummary()
        {
            return "set credential property on " + ((Obj as ITabularNamedObject).Name ?? Obj.GetTypeName()) + "." + credentialPropertyName + ": " + oldValue + " -> " + newValue;
        }

        public UndoCredentialAction(StructuredDataSource obj, string credentialPropertyName, string newValue, string oldValue)
        {
            Obj = obj;
            this.credentialPropertyName = credentialPropertyName;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public void Redo()
        {
            Obj.Credential[credentialPropertyName] = newValue;
        }

        public void Undo()
        {
            Obj.Credential[credentialPropertyName] = oldValue;
        }
    }
}
