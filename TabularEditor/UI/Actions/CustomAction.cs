using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI.Actions
{
    public class CustomAction : Action
    {
        public CustomAction(EnabledDelegate enabled, ExecuteDelegate execute, string name, bool hideWhenDisabled = false) : base(enabled, execute, (a,b) => name, hideWhenDisabled)
        {
        }

        public override string Name
        {
            get
            {
                var n = base.Name;
                if (n.Contains(@"\")) return n;
                else return @"Custom actions\" + n;
            }
        }
    }
}
