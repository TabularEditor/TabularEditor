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
    public class CutAction: ClipboardAction
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
            else if (SelectedNodes != null)
            {
                // Copy objects represented by selected nodes to clipboard
                var json = Serializer.SerializeObjects(UIController.Current.Selection.OfType<TabularNamedObject>(),
                    Preferences.Current.Copy_IncludeTranslations, Preferences.Current.Copy_IncludePerspectives,
                    Preferences.Current.Copy_IncludeRLS
#if CL1400
                    , Preferences.Current.Copy_IncludeOLS
#endif
                    );
                Clipboard.SetText(json, TextDataFormat.UnicodeText);
                UIController.Current.Selection.Delete();
            }

            base.OnExecute(e);

        }

        protected override void OnUpdate(EventArgs e)
        {
            Enabled = (ActiveTextBox != null && ActiveTextBox.SelectionLength > 0 && !ActiveTextBox.ReadOnly)
                || (SelectedNodes != null && UIController.Current.Selection.All(obj => obj.CanDelete() && obj is IClonableObject));
            base.OnUpdate(e);
        }
    }
}
