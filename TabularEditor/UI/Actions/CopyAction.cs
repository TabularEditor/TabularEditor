using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI.Actions
{
    public class CopyAction: TextBoxAction
    {
        public CopyAction()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C);
            this.Text = "&Copy";
            this.ToolTipText = "Copy";
        }

        protected override void OnExecute(EventArgs e)
        {
            if (ActiveTextBox != null) ActiveTextBox.Copy();
            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            Enabled = (ActiveTextBox != null && ActiveTextBox.SelectionLength > 0);
            base.OnUpdate(e);
        }
    }
}
