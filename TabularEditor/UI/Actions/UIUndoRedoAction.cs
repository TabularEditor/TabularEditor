using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Actions
{
    public enum UndoRedo
    {
        Undo,
        Redo
    }

    public class UIUndoRedoAction: UIModelAction
    {
        [Browsable(true)]
        public UndoRedo Kind { get; set; } = UndoRedo.Undo;

        protected override void OnExecute(EventArgs e)
        {
            var ctr = UIHelper.GetFocusedControl(Application.OpenForms[0]);
            
            if (ctr is TextBoxBase && PropertyGridHelper.IsPropertyGridControl(ctr) && (ctr as TextBoxBase).CanUndo)
            {
                // Property grids handle undo/redo on their own
            }
            else if(ctr is FastColoredTextBoxNS.FastColoredTextBox)
            {
                if (Kind == UndoRedo.Undo) (ctr as FastColoredTextBoxNS.FastColoredTextBox).Undo();
                else (ctr as FastColoredTextBoxNS.FastColoredTextBox).Redo();
            } else if(ctr is TextBoxBase)
            {
                if (Kind == UndoRedo.Undo) (ctr as TextBoxBase).Undo();
                else (ctr as RichTextBox)?.Redo();
            } else
            {
                using (new Hourglass())
                {
                    if (Kind == UndoRedo.Undo) Handler.UndoManager.Undo();
                    else Handler.UndoManager.Redo();
                }
            }

            base.OnExecute(e);
        }

        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);


            var ctr = UIHelper.GetFocusedControl(Application.OpenForms[0]);

            if (ctr is FastColoredTextBoxNS.FastColoredTextBox)
            {
                var c = ctr as FastColoredTextBoxNS.FastColoredTextBox;
                Text = Kind == UndoRedo.Undo ? "Undo" : "Redo";
                Enabled = Kind == UndoRedo.Undo ? c.UndoEnabled : c.RedoEnabled;
            } else if (ctr is TextBoxBase)
            {
                Text = Kind == UndoRedo.Undo ? "Undo" : "Redo";
                Enabled = Kind == UndoRedo.Undo ? (ctr as TextBoxBase).CanUndo : (ctr as RichTextBox)?.CanRedo ?? false;
            } else if(Handler != null)
            {
                var am = Handler.UndoManager;

                Text = Kind == UndoRedo.Undo ? "Undo " + am.UndoText : "Redo " + am.RedoText;
                Enabled = Kind == UndoRedo.Undo ? am.CanUndo : am.CanRedo;
            }
        }
    }
}
