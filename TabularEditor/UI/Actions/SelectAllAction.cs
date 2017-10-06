using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI.Actions
{
    public class SelectAllAction: ClipboardAction
    {
        public SelectAllAction()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A);
            this.Text = "Select &All";
            this.ToolTipText = "Select All";
        }

        protected override void OnExecute(EventArgs e)
        {
            if (ActiveTextBox != null && ActiveTextBox.CanSelect) ActiveTextBox.SelectAll();
            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            Enabled = (ActiveTextBox != null && ActiveTextBox.CanSelect);
            base.OnUpdate(e);
        }
    }
}
