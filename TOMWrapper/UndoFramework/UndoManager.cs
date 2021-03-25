using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper.Undo
{
    /// <summary>
    /// UndoManager implements an undo/redo handling system. The system implements two stacks: One for undoable operations (i.e. "going back"),
    /// and one for redoable operations (i.e. undoable operations that has been undone, "going forward"). The UndoManager supports setting
    /// checkpoints, rolling all changes back, etc.
    /// 
    /// To register an undoable operation, call the "Add" method. Use the public methods "Undo()", "Redo()", "Clear()", etc. to control the
    /// stack of operations. Use "BeginBatch()" and "EndBatch()" to register batch of undoable operations that will be undone/redone together
    /// in a single batch.
    /// </summary>
    public sealed class UndoManager
    {
        TabularModelHandler _handler;
        Stack<IUndoAction> _UndoStack = new Stack<IUndoAction>();
        Stack<IUndoAction> _RedoStack = new Stack<IUndoAction>();

        /// <summary>
        /// Whether or not the undo manager is collecting operations.
        /// </summary>
        public bool Enabled => SuspendCount == 0;
        public void Suspend()
        {
            SuspendCount++;
        }
        public void Resume()
        {
            SuspendCount--;
        }
        private int SuspendCount = 0;

        /// <summary>
        /// Rolls back all changes done to the model.
        /// </summary>
        /// <param name="toCheckPoint">If this is set to true, the model is only rolled back to the last set checkpoint (typically when the model was last saved).</param>
        public void Rollback(bool toCheckPoint = false)
        {
            while((toCheckPoint && !AtCheckpoint) || (!toCheckPoint && UndoSize > 0))
            {
                XDo(false, true);
            }
        }

        /// <summary>
        /// Call this method to indicate that a change was applied, even when it might not be undoable (such as annotation changes).
        /// </summary>
        public void FlagChange()
        {
            checkPointCount--;
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
        public bool AtCheckpoint
        {
            get
            {
                if (_UndoStack.Count == checkPointCount) return true;
                if (_UndoStack.Count > checkPointCount && _UndoStack.Take(_UndoStack.Count - checkPointCount).All(u => u is UndoBatchAction uba && uba.Begin)) return true;
                else return false;
            }
        }

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

        /// <summary>
        /// Creates the UndoManager. Remember to set 'Enabled' to true after creation.
        /// </summary>
        /// <param name="handler"></param>
        public UndoManager(TabularModelHandler handler)
        {
            _handler = handler;
        }

        bool inProgress;
        public bool UndoInProgress => inProgress;
        public bool IsUndoing { get; private set; }
        public bool IsRedoing { get; private set; }

        internal void XDo(bool redo, bool inversable)
        {
            if (!Enabled) throw new InvalidOperationException("UndoManager is not enabled.");
            //if (undoDepth == -1 && batchDepth != 0) throw new InvalidOperationException("Cannot undo/redo while a batch is in progress.");

            _handler.BeginUpdate(null);

            var stack = redo ? _RedoStack : _UndoStack;
            var inverseStack = redo ? _UndoStack : _RedoStack;

            if (stack.Count > 0)
            {
                inProgress = true;
                IsRedoing = redo;
                IsUndoing = !IsRedoing;

                var item = stack.Pop();
                if (redo) { item.Redo(); }
                else item.Undo();

                if(inversable) inverseStack.Push(item);

                if (item is UndoBatchAction)
                {
                    var batchDepth = (item as UndoBatchAction).Depth;

                    do
                    {
                        item = stack.Pop();
                        if (redo)
                            item.Redo();
                        else item.Undo();

                        if(inversable) inverseStack.Push(item);
                    }
                    while (!(item is UndoBatchAction) || (item as UndoBatchAction).Depth > batchDepth);


                }
                
                IsRedoing = false;
                IsUndoing = false;
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
        public int BatchSize => batchSizeCounter;
        UndoBatchAction batch = null;

        /// <summary>
        /// Begins a batch of undoable operations.
        /// </summary>
        /// <param name="batchName">A descriptive name for the batch.</param>
        public void BeginBatch(string batchName)
        {
            if (!Enabled) return;

            if (inProgress) return;
            if(batchDepth == 0) batchSizeCounter = 0;

            batch = new UndoBatchAction(batchName, BatchDepth, true);
            _UndoStack.Push(batch);

            batchDepth++;
        }

        /// <summary>
        /// Ends a batch of undoable operations.
        /// </summary>
        /// <param name="rollback">Set to true if the batch needs to be undone immediately.</param>
        /// <returns></returns>
        public int EndBatch(bool rollback = false)
        {
            if (!Enabled) return 0;

            if (inProgress) return 0;
            if (batchDepth == 0) throw new InvalidOperationException("EndBatch() called before BeginBatch().");
            batchDepth--;

            if ((_UndoStack.Peek() as UndoBatchAction)?.Begin ?? false)
            {
                // If the last action on the stack is a begin-BatchAction, it means no actions was performed
                // in this batch. Pop the last BatchAction to avoid having an empty batch on the stack.
                var a = _UndoStack.Pop() as UndoBatchAction;

                if (!a.Begin) throw new InvalidOperationException(); // should not happen

                return 0;
            }
            else if ((_UndoStack.Skip(1).FirstOrDefault() as UndoBatchAction)?.Begin ?? false)
            {
                // If the batch action only contains a single non-batch action, we might as well pop the batch
                // and add the non-batch action directly to the stack:
                var a = _UndoStack.Pop();
                var beginBatch = _UndoStack.Pop();
                _UndoStack.Push(a);
            }
            else
            {
                var actionName = _UndoStack.OfType<UndoBatchAction>().First(uba => uba.Depth == batchDepth).ActionName;
                _UndoStack.Push(new UndoBatchAction(actionName, batchDepth, false));
            }

            // Rollback if requested:
            if (rollback)
            {
                XDo(false, false);
                return 0;
            }

            UndoStateChanged?.Invoke(this, new EventArgs());
            return batchSizeCounter;
        }


        public int UndoSize => _UndoStack.Count;
        public int UndoSteps => CountSteps(_UndoStack);
        public int RedoSize => _RedoStack.Count;
        public int RedoSteps => CountSteps(_RedoStack);

        private int CountSteps(Stack<IUndoAction> actions)
        {
            int count = 0;
            int batchDepth = 0;
            foreach (var action in actions)
            {
                if (batchDepth == 0) count++;
                if (action is UndoBatchAction uba)
                {
                    batchDepth += uba.Begin ? 1 : -1;
                }
            }
            return count;
        }


        public int BatchDepth { get { return batchDepth; } }

        public event EventHandler UndoStateChanged;

        /// <summary>
        /// Call this method when an action is performed, that should be undoable
        /// </summary>
        /// <param name="action"></param>
        internal void Add(IUndoAction action)
        {
            if (!Enabled) return;

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
