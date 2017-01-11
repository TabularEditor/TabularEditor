using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Actions
{
    public class PasteAction: TextBoxAction
    {
        public PasteAction()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ShortcutKeys = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V);
            this.Text = "&Paste";
            this.ToolTipText = "Paste";
        }

        protected override void OnExecute(EventArgs e)
        {
            if (ActiveTextBox != null)
            {
                // PropertyGrids handle copy/paste on their own - below check fixes issue #20
                if (!PropertyGridHelper.IsPropertyGridControl(ActiveTextBox.Parent))
                    ActiveTextBox.Paste();
            }
            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            Enabled = ActiveTextBox != null && Clipboard.ContainsData(DataFormats.Text);
            base.OnUpdate(e);
        }
    }
}
