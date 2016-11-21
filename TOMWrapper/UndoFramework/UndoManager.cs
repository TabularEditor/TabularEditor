using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UndoFramework
{
    public class UndoManager
    {
        TabularModelHandler _handler;
        Stack<IUndoAction> _UndoStack = new Stack<IUndoAction>();
        Stack<IUndoAction> _RedoStack = new Stack<IUndoAction>();

        public void Rollback()
        {
            while(_UndoStack.Count > 0)
            {
                Undo();
            }
        }

        int checkPointCount = 0;
        /// <summary>
        /// Sets the checkpoint. You can use the <see cref="AtCheckpoint"/> property to determine
        /// if the undo stack is currently at the checkpoint (meaning no changes have been done).
        /// </summary>
        public void SetCheckpoint()
        {
            checkPointCount = _UndoStack.Count;
        }
        /// <summary>
        /// Returns true if no changes have been done relative to the last checkpoint set.
        /// </summary>
        public bool AtCheckpoint { get { return _UndoStack.Count == checkPointCount; } }

        /// <summary>
        /// Returns a string containing the entire history of changes applied to the model
        /// in a human-readable way.
        /// </summary>
        /// <returns></returns>
        public string GetHistory()
        {
            var sb = new StringBuilder();

            var indent = 0;
            foreach(var act in _UndoStack.Reverse())
            {
                if(act is UndoBatchAction && !(act as UndoBatchAction).Begin) indent -= 1;

                sb.Append(new string(' ', indent * 2));
                sb.AppendLine(act.GetSummary());

                if (act is UndoBatchAction && (act as UndoBatchAction).Begin) indent += 1;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string containing the C# code needed to recreate the entire history
        /// of changes applied to the model.
        /// </summary>
        /// <returns></returns>
        public string GetHistoryCode()
        {
            var sb = new StringBuilder();

            foreach(var act in _UndoStack.Reverse())
            {
                var code = act.GetCode();
                if(!string.IsNullOrEmpty(code)) sb.AppendLine(code);
            }

            return sb.ToString();
        }

        public UndoManager(TabularModelHandler handler)
        {
            _handler = handler;
        }

        private bool inProgress = false;

        internal void XDo(bool redo, bool inversable)
        {
            if (batchDepth != 0) throw new InvalidOperationException("Cannot undo/redo while a batch is in progress.");

            _handler.BeginUpdate(null);

            var stack = redo ? _RedoStack : _UndoStack;
            var inverseStack = redo ? _UndoStack : _RedoStack;

            if (stack.Count > 0)
            {
                inProgress = true;

                var item = stack.Pop();
                if (redo) item.Redo();
                else item.Undo();

                if(inversable) inverseStack.Push(item);

                if (item is UndoBatchAction)
                {
                    do
                    {
                        item = stack.Pop();
                        if (redo) item.Redo();
                        else item.Undo();

                        if(inversable) inverseStack.Push(item);
                    }
                    while (!(item is UndoBatchAction));


                }

                inProgress = false;
            }
            UndoStateChanged?.Invoke(this, new EventArgs());

            _handler.EndUpdate(false);
        }

        public void Undo()
        {
            XDo(false, true);
        }
        public void Redo()
        {
            XDo(true, true);
        }
        public void Clear()
        {
            if (inProgress) throw new InvalidOperationException("Cannot clear undo/redo stack while undo/redo operation is in progress.");
            _UndoStack.Clear();
            _RedoStack.Clear();
            batchDepth = 0;
            batch = null;
        }

        private int batchDepth = 0;
        private int batchSizeCounter = 0;
        UndoBatchAction batch = null;

        /// <summary>
        /// Begins a batch of undoable operations.
        /// </summary>
        /// <param name="batchName">A descriptive name for the batch.</param>
        public void BeginBatch(string batchName)
        {
            if (inProgress) return;
            if(batchDepth == 0)
            {
                batchSizeCounter = 0;
                batch = new UndoBatchAction(batchName, true);
                _UndoStack.Push(batch);
            }
            batchDepth++;
        }

        /// <summary>
        /// Ends a batch of undoable operations.
        /// </summary>
        /// <param name="undo">Set to true if the batch needs to be undone immediately.</param>
        /// <returns></returns>
        public int EndBatch(bool undo = false)
        {
            if (inProgress) return 0;
            if (batchDepth == 0) throw new InvalidOperationException("EndBatch() called before BeginBatch().");
            batchDepth--;
            if(batchDepth == 0)
            {
                if (_UndoStack.Peek() is UndoBatchAction)
                {
                    // If the last action on the stack is also a BatchAction, it means no actions was performed
                    // in this batch. Pop the last BatchAction to avoid having an empty batch on the stack.
                    var a = _UndoStack.Pop() as UndoBatchAction;

                    if (!a.Begin) throw new InvalidOperationException(); // should not happen

                    return 0;
                }
                else
                {
                    _UndoStack.Push(new UndoBatchAction(batch.ActionName, false));
                    if (undo)
                    {
                        XDo(false, false);
                        return 0;
                    }
                }
            }
            UndoStateChanged?.Invoke(this, new EventArgs());
            return batchSizeCounter;
        }

        public event EventHandler UndoStateChanged;

        /// <summary>
        /// Call this method when an action is performed, that should be undoable
        /// </summary>
        /// <param name="action"></param>
        public void Add(IUndoAction action)
        {
            if (inProgress) return;
            _UndoStack.Push(action);
            if (CanRedo) _RedoStack.Clear();
            batchSizeCounter++;
            if (!(action is UndoBatchAction) && batchDepth == 0)
                UndoStateChanged?.Invoke(this, new EventArgs());
        }

        public bool CanUndo { get { return _UndoStack.Count > 0; } }
        public bool CanRedo { get { return _RedoStack.Count > 0; } }

        public string UndoText { get { return CanUndo ? _UndoStack.Peek().ActionName : string.Empty; } }
        public string RedoText { get { return CanRedo ? _RedoStack.Peek().ActionName : string.Empty; } }
    }
}
