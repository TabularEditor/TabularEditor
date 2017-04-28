using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper
{
    public enum ObjectOrder
    {
        Alphabetical,
        Metadata
    }

    public class TabularObjectComparer : IComparer<ITabularNamedObject>, IComparer
    {
        private TabularTree _tree;
        private ObjectOrder _order;
        public ObjectOrder Order {
            get { return _order; }
            set {
                if (value == _order) return;
                _order = value;
                _tree.OnStructureChanged();
            }
        }

        public TabularObjectComparer(TabularTree tree, ObjectOrder order)
        {
            _tree = tree;
            _order = order;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as ITabularNamedObject, y as ITabularNamedObject);
        }
        
        public int Compare(ITabularNamedObject x, ITabularNamedObject y)
        {
            var c = x.ObjectType.CompareTo(y.ObjectType);
            if (c == 0)
            {
                if (x.ObjectType == ObjectType.Level && y.ObjectType == ObjectType.Level)
                    return (x as Level).Ordinal.CompareTo((y as Level).Ordinal);

                // Compare the metadata indices of the two objects:
                var metadataComparison = x.MetadataIndex.CompareTo(y.MetadataIndex);

                // If we're ordering by metadata, and objects don't have have the metadataindex, return the comparison:
                if (_order == ObjectOrder.Metadata && metadataComparison != 0) return metadataComparison;

                // ...in all other cases, use alphabetical ordering:
                return string.Compare(x.GetName(_tree.Culture), y.GetName(_tree.Culture), true);
            }
            return c;
        }
    }
}
