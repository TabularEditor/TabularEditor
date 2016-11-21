using System.Collections.Generic;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UndoFramework
{
    public interface IUndoAction
    {
        void Undo();
        void Redo();
        string ActionName { get; }
        string GetSummary();
        string GetCode();
    }
}
