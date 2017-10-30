using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoAnnotationAction : IUndoAction
    {
        IAnnotationObject Obj;
        string name;
        string oldValue;
        string newValue;

        public string ActionName
        {
            get
            {
                return "annotation change";
            }
        }

        public string GetCode()
        {
            return "// set annotation on " + (Obj as ITabularNamedObject).Name ?? Obj.GetTypeName();
        }

        public string GetSummary()
        {
            return "set annotation on " + ((Obj as ITabularNamedObject).Name ?? Obj.GetTypeName()) + "." + name + ": " + oldValue + " -> " + newValue;
        }

        public UndoAnnotationAction(IAnnotationObject obj, string annotationName, string newValue, string oldValue)
        {
            Obj = obj;
            this.name = annotationName;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public void Redo()
        {
            Obj.SetAnnotation(name, newValue);
        }

        public void Undo()
        {
            Obj.SetAnnotation(name, oldValue);
        }
    }
}
