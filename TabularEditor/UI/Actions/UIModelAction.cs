using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using System.ComponentModel;

namespace TabularEditor.UI
{
    /// <summary>
    /// Base class for all actions that rely on the FormMain having a Handler loaded.
    /// All actions that derive from this action will be disabled when a Handler is
    /// not loaded.
    /// </summary>
    [StandardAction]
    public class UIModelAction : Crad.Windows.Forms.Actions.Action
    {
        public event EventHandler<UpdateExEventArgs> UpdateEx;

        protected TabularModelHandler Handler { get { return UIController.Current.Handler; } }
        protected Model Model { get { return UIController.Current.Handler.Model; } }
        protected UITreeSelection Selected { get { return UIController.Current.Selection; } }

        protected override void OnUpdate(EventArgs e)
        {
            if(UpdateEx == null)
            {
                Enabled = Handler != null;
                //base.OnUpdate(e);
                return;
            }

            var args = new UpdateExEventArgs { Enabled = Handler != null };
            UpdateEx?.Invoke(this, args);
            Enabled = args.Enabled && Handler != null;
        }

        protected override void OnExecute(EventArgs e)
        {
            base.OnExecute(e);
        }

        protected override void OnBeforeExecute(CancelEventArgs e)
        {
            UI.UIController.Current.BeforeAction();
            base.OnBeforeExecute(e);
        }
    }

    public class UpdateExEventArgs : EventArgs
    {
        public bool Enabled;
    }

}
