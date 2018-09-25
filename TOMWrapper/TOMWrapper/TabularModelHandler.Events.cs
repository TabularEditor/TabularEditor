using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public class ObjectChangedEventArgs
    {
        public TabularObject TabularObject { get; private set; }
        public string PropertyName { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public ObjectChangedEventArgs(TabularObject tabularObject, string propertyName, object oldValue, object newValue)
        {
            TabularObject = tabularObject;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ObjectChangingEventArgs
    {
        public bool Cancel { get; set; } = false;
        public TabularObject TabularObject { get; private set; }
        public string PropertyName { get; private set; }
        public object NewValue { get; private set; }

        public ObjectChangingEventArgs(TabularObject tabularObject, string propertyName, object newValue)
        {
            TabularObject = tabularObject;
            PropertyName = propertyName;
            NewValue = newValue;
            Cancel = false;
        }
    }

    public class ObjectDeletingEventArgs
    {
        public bool Cancel { get; set; } = false;
        public TabularObject TabularObject { get; private set; }
        public ObjectDeletingEventArgs(TabularObject tabularObject)
        {
            TabularObject = tabularObject;
        }
    }

    public class ObjectDeletedEventArgs
    {
        public TabularObject TabularObject { get; private set; }
        public ObjectDeletedEventArgs(TabularObject tabularObject)
        {
            TabularObject = tabularObject;
        }
    }

    public delegate void ObjectChangingEventHandler(object sender, ObjectChangingEventArgs e);
    public delegate void ObjectChangedEventHandler(object sender, ObjectChangedEventArgs e);
    public delegate void ObjectDeletingEventHandler(object sender, ObjectDeletingEventArgs e);
    public delegate void ObjectDeletedEventHandler(object sender, ObjectDeletedEventArgs e);

    public partial class TabularModelHandler
    {

        public event ObjectChangingEventHandler ObjectChanging;
        public event ObjectChangedEventHandler ObjectChanged;
        public event ObjectDeletingEventHandler ObjectDeleting;
        public event ObjectDeletedEventHandler ObjectDeleted;

        internal void DoObjectDeleting(TabularObject obj, ref bool cancel)
        {
            var e = new ObjectDeletingEventArgs(obj);
            ObjectDeleting?.Invoke(this, e);
            cancel = e.Cancel;
        }
        internal void DoObjectDeleted(TabularObject obj, ITabularNamedObject parentBeforeDeletion)
        {
            if (obj is IFolderObject) Tree.RebuildFolderCacheForTable(parentBeforeDeletion as Table);

            var e = new ObjectDeletedEventArgs(obj);
            ObjectDeleted?.Invoke(this, e);
        }

        internal void DoObjectChanging(TabularObject obj, string propertyName, object newValue, ref bool cancel)
        {
            var e = new ObjectChangingEventArgs(obj, propertyName, newValue);
            ObjectChanging?.Invoke(this, e);
            cancel = e.Cancel;
        }
        internal void DoObjectChanged(TabularObject obj, string propertyName, object oldValue, object newValue)
        {
            var e = new ObjectChangedEventArgs(obj, propertyName, oldValue, newValue);
            ObjectChanged?.Invoke(this, e);
        }
    }
}
