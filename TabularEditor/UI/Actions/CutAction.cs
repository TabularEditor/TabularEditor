using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI.Actions
{
    public class CutAction: TextBoxAction
    {
        public CutAction()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X);
            this.Text = "Cu&t";
            this.ToolTipText = "Cut";
        }

        protected override void OnExecute(EventArgs e)
        {
            if (ActiveTextBox != null) ActiveTextBox.Cut();
            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            Enabled = (ActiveTextBox != null && ActiveTextBox.SelectionLength > 0 && !ActiveTextBox.ReadOnly);
            base.OnUpdate(e);
        }
    }
}
