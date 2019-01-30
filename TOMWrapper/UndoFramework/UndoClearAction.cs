using System.Collections.Generic;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal class UndoClearAction : IUndoAction
    {
        ITabularObjectCollection _collection;
        TabularNamedObject[] _objs;

        public UndoClearAction(ITabularObjectCollection collection, TabularNamedObject[] content)
        {
            _collection = collection;
            _objs = content;
        }

        public string ActionName { get { return "clear"; } }

        public void Redo()
        {
            _collection.Clear();
        }

        public void Undo()
        {
            foreach (var obj in _objs) obj.Undelete(_collection);
        }

        public string GetSummary()
        {
            return string.Format("Cleared collection {{{0}}}", _collection.CollectionName);
        }

        public string GetCode()
        {
            return string.Empty;
        }
    }

}
