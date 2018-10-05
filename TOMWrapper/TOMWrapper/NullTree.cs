using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{


    public class NullTree : TOMWrapper.TabularTree
    {
        public NullTree(TabularModelHandler handler) : base(handler)
        {
        }

        public override void OnNodesChanged(ITabularObject nodeItem)
        {
        }

        public override void OnNodesInserted(ITabularObject parent, params ITabularObject[] children)
        {
        }

        public override void OnNodesRemoved(ITabularObject parent, params ITabularObject[] children)
        {
        }

        public override void OnStructureChanged(ITabularNamedObject obj = null)
        {
        }
    }
}
