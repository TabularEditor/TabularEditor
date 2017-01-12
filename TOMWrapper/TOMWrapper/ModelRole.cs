using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class ModelRole
    {
        [Browsable(true), DisplayName("Row Level Filters"), Category("Security")]
        public RoleRLSIndexer RowLevelSecurity { get; private set; }
        
        public void InitRLSIndexer()
        {
            RowLevelSecurity = new RoleRLSIndexer(this);
        }
    }
}
