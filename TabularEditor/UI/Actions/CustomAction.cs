using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Actions
{
    public class CustomAction : Action
    {
        public CustomAction(EnabledDelegate enabled, ExecuteDelegate execute, string name, bool hideWhenDisabled = true, Context validContexts = Context.SingularObjects) : base(enabled, execute, (a,b) => name, hideWhenDisabled, validContexts)
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
        public string BaseName
        {
            get
            {
                return base.Name;
            }
        }

        public override void Execute(object arg)
        {
            try
            {
                TabularModelHandler.Singleton.BeginUpdate(Name.Split('\\').Last());
                base.Execute(arg);
                TabularModelHandler.Singleton.EndUpdateAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error executing custom action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TabularModelHandler.Singleton.EndUpdateAll(true);
            }
        }

        public void ExecuteInScript(object arg)
        {
            base.Execute(arg);
        }
    }
}
