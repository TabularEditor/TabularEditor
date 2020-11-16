using System;
using System.Collections.Generic;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper.Undo
{
    internal enum UndoAddRemoveActionType
    {
        Add,
        Remove
    }

    internal class UndoAddRemoveAction : IUndoAction
    {
        ITabularObjectCollection _collection;
        TabularNamedObject _obj;
        UndoAddRemoveActionType _actionType;
        string _json;
        Type _tomObjectType;

        public UndoAddRemoveAction(ITabularObjectCollection collection, TabularNamedObject obj, UndoAddRemoveActionType actionType)
        {
            _tomObjectType = obj.MetadataObject.GetType();
            _json = Microsoft.AnalysisServices.Tabular.JsonSerializer.SerializeObject(obj.MetadataObject, TabularObject.RenewMetadataOptions);
            _collection = collection;
            _obj = obj;
            _actionType = actionType;
        }

        public string ActionName
        {
            get
            {
                return _actionType == UndoAddRemoveActionType.Add ? "add object" : "remove object";
            }
        }

        public void Redo()
        {
            if (_actionType == UndoAddRemoveActionType.Add)
            {
                _obj.Undelete(_collection, _tomObjectType, _json);
                //_collection.Add(_obj);
            }
            else
                _obj.Delete();
                //_collection.Remove(_obj);
        }

        public void Undo()
        {
            if (_actionType == UndoAddRemoveActionType.Add)
                _obj.Delete();
            //_collection.Remove(_obj);
            else
            {
                _obj.Undelete(_collection, _tomObjectType, _json);
                //_collection.Add(_obj);
            }
        }

        public string GetSummary()
        {
            return string.Format("{0} object {{{1}}} in collection {{{2}}}", _actionType == UndoAddRemoveActionType.Add ? "Added" : "Removed",
                _obj.Name, _collection.CollectionName);
        }

        public string GetCode()
        {
            if(_actionType == UndoAddRemoveActionType.Add)
            {
                var path = _obj.GetLinqPath();
                path = path.Substring(0, path.Length - _obj.Name.Length - 4);
                return string.Format("{0}.Add(new {1}() {{ Name = \"{2}\" }});", path, _obj.GetTypeName(), _obj.Name);
            } else
            {
                return _obj.GetLinqPath() + ".Remove();";
            }
        }
    }

    internal class UndoAddRemoveReferenceAction : IUndoAction
    {
        ITabularObjectCollection _collection;
        TabularNamedObject _obj;
        UndoAddRemoveActionType _actionType;

        public UndoAddRemoveReferenceAction(ITabularObjectCollection collection, TabularNamedObject obj, UndoAddRemoveActionType actionType)
        {
            _collection = collection;
            _obj = obj;
            _actionType = actionType;
        }

        public string ActionName
        {
            get
            {
                return _actionType == UndoAddRemoveActionType.Add ? "add object" : "remove object";
            }
        }

        public void Redo()
        {
            if (_actionType == UndoAddRemoveActionType.Add)
            {
                _collection.Add(_obj);
            }
            else
                _collection.Remove(_obj);
        }

        public void Undo()
        {
            if (_actionType == UndoAddRemoveActionType.Add)
                _collection.Remove(_obj);
            else
            {
                _collection.Add(_obj);
            }
        }

        public string GetSummary()
        {
            return string.Format("{0} object {{{1}}} in collection {{{2}}}", _actionType == UndoAddRemoveActionType.Add ? "Added" : "Removed",
                _obj.Name, _collection.CollectionName);
        }

        public string GetCode()
        {
            if (_actionType == UndoAddRemoveActionType.Add)
            {
                var path = _obj.GetLinqPath();
                path = path.Substring(0, path.Length - _obj.Name.Length - 4);
                return string.Format("{0}.Add(new {1}() {{ Name = \"{2}\" }});", path, _obj.GetTypeName(), _obj.Name);
            }
            else
            {
                return _obj.GetLinqPath() + ".Remove();";
            }
        }
    }

}
