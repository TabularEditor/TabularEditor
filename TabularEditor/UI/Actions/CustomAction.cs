using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.Scripting;
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
                ScriptHelper.BeforeScriptExecution();
                base.Execute(arg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error executing custom action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                ScriptHelper.AfterScriptExecution();
                TabularModelHandler.Singleton.EndUpdateAll();
            }
        }

        public void ExecuteInScript(object arg)
        {
            base.Execute(arg);
        }
    }
}
