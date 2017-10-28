using Crad.Windows.Forms.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UIServices;

namespace TabularEditor.UI.Actions
{
    public class CopyAction: ClipboardAction
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
            if (ActiveTextBox != null)
            {
                // PropertyGrids handle copy/paste on their own - below check fixes issue #20
                if (!PropertyGridHelper.IsPropertyGridControl(ActiveTextBox.Parent))
                    ActiveTextBox.Copy();
            }
            else if (SelectedNodes != null)
            {
                // Copy objects represented by selected nodes to clipboard
                var json = Serializer.SerializeObjects(UIController.Current.Selection.OfType<TabularNamedObject>(),
                    Preferences.Current.Copy_IncludeTranslations, Preferences.Current.Copy_IncludePerspectives,
                    Preferences.Current.Copy_IncludeRLS, Preferences.Current.Copy_IncludeOLS
                    );
                Clipboard.SetText(json, TextDataFormat.UnicodeText);
            }
            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            Enabled = (ActiveTextBox != null && ActiveTextBox.SelectionLength > 0) || 
                (TreeHasFocus && UIController.Current.Selection.All(obj => obj is IClonableObject));
            base.OnUpdate(e);
        }
    }
}
