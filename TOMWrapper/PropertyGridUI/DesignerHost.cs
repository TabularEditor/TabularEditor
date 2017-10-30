using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor
{
    /// <summary>
    /// The DesignerHost is a custom implementation of IDesignerHost and ISite, which is needed to hookup
    /// any PropertyGrid components used in the UI to enable proper Undo/Redo functionality. Otherwise,
    /// when changing the properties of multiple objects at once, the operation is not batched, meaning
    /// that the UI will be updated when a property value is set for every object in the selection. This
    /// will cause slow updates when many objects are selected, and furthermore, undoing the entire operation
    /// is a hassle, as one Undo-action is needed per object.
    /// 
    /// To set up a PropertyGrid to use this host, use the following code:
    /// 
    ///     MyPropertyGrid.Site = new DesignerHost();
    /// </summary>
    public class DesignerHost : IDesignerHost, ISite
    {
        #region Unused ISite members
        public IComponent Component { get { return null; } }
        public IContainer Container { get { return null; } }
        public bool DesignMode { get { return false; } }
        public string Name { get { return null; } set { } }
        #endregion
        #region Unused IDesignerHost members
        // public IContainer Container -- already implemented as part of ISite
        public bool Loading { get { return false; } }
        public IComponent RootComponent { get { return null; } }
        public string RootComponentClassName { get { return String.Empty; } }
        public void Activate() { }
        public IComponent CreateComponent(Type componentClass, string name) { return null; }
        public IComponent CreateComponent(Type componentClass)
        {
            return CreateComponent(componentClass, null);
        }
        public void DestroyComponent(IComponent component) { }
        public IDesigner GetDesigner(IComponent component) { return null; }
        public Type GetType(string typeName) { return null; }
        public void AddService(Type serviceType, object serviceInstance, bool promote) { }
        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) { }
        public void AddService(Type serviceType, object serviceInstance)
        {
            AddService(serviceType, serviceInstance, false);
        }
        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            AddService(serviceType, callback, false);
        }
        public void RemoveService(Type serviceType, bool promote) { }
        public void RemoveService(Type serviceType)
        {
            RemoveService(serviceType, false);
        }
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler LoadComplete;
        public event DesignerTransactionCloseEventHandler TransactionClosed;
        public event DesignerTransactionCloseEventHandler TransactionClosing;
        public event EventHandler TransactionOpened;
        public event EventHandler TransactionOpening;
        #endregion

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDesignerHost)) return this;
            else return null;
        }

        #region Transaction handling (used for UndoManager integration)
        public bool InTransaction { get { return false; } }
        public string TransactionDescription { get { return String.Empty; } }
        public DesignerTransaction CreateTransaction(string description)
        {
            if (description == null) return null;
            return new UndoableDesignerTransaction(description);
        }
        public DesignerTransaction CreateTransaction()
        {
            return CreateTransaction(null);
        }
        #endregion
    }

    internal class UndoableDesignerTransaction : DesignerTransaction
    {
        int originalBatchDepth;
        TabularModelHandler Handler { get { return TabularModelHandler.Singleton; } }

        public UndoableDesignerTransaction(string description)
        {
            originalBatchDepth = Handler.UndoManager.BatchDepth;
            Handler.BeginUpdate(description);
        }

        protected override void OnCancel()
        {
            // Rollback all changes done since the transaction started:
            while (Handler.UndoManager.BatchDepth > originalBatchDepth)
            {
                Handler.EndUpdate(true, true);
            }

        }

        protected override void OnCommit()
        {
            // Commit all changes done since the transaction started:
            while (Handler.UndoManager.BatchDepth > originalBatchDepth)
            {
                Handler.EndUpdate(true, false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            OnCancel();
            base.Dispose(disposing);
        }
    }
}
