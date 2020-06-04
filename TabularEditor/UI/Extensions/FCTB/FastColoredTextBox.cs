using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI.Extensions.FCTB
{
    public class FastColoredTextBox: FastColoredTextBoxNS.FastColoredTextBox
    {
        ReplaceForm betterReplaceForm;

        public override void ShowReplaceDialog(string findText)
        {
            if (ReadOnly)
                return;
            if (betterReplaceForm == null)
                betterReplaceForm = new ReplaceForm(this);

            if (findText != null)
                betterReplaceForm.tbFind.Text = findText;
            else if (!Selection.IsEmpty && Selection.Start.iLine == Selection.End.iLine)
                betterReplaceForm.tbFind.Text = Selection.Text;

            betterReplaceForm.tbFind.SelectAll();
            betterReplaceForm.Show();
            betterReplaceForm.Focus();
        }

        public override void ShowReplaceDialog()
        {
            this.ShowReplaceDialog(null);
        }
    }
}
