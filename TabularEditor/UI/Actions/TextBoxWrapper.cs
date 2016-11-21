using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Actions
{
    /// <summary>
    /// A wrapper that implements ITextBox, representing either a TextBoxBase derivative, or a FastColoredTextBox.
    /// </summary>
    public class TextboxWrapper : ITextBox
    {
        TextBoxBase tb;
        FastColoredTextBoxNS.FastColoredTextBox fc;
        bool isFc = false;

        public TextboxWrapper(TextBoxBase textBox)
        {
            tb = textBox;
            isFc = false;
        }
        public TextboxWrapper(FastColoredTextBoxNS.FastColoredTextBox textBox)
        {
            fc = textBox;
            isFc = true;
        }

        public bool ReadOnly
        {
            get
            {
                return isFc ? fc.ReadOnly : tb.ReadOnly;
            }
        }

        public int SelectionLength
        {
            get
            {
                return isFc ? fc.SelectionLength : tb.SelectionLength;
            }
        }

        public void Cut()
        {
            if (isFc) fc.Cut(); else tb.Cut();
        }

        public void Undo()
        {
            if (isFc) fc.Undo(); else tb.Undo();
        }

        public void Redo()
        {
            if (isFc) fc.Redo(); else (tb as RichTextBox)?.Redo();
        }



        public void Copy()
        {
            if (isFc) fc.Copy(); else tb.Copy();
        }

        public void Paste()
        {
            if (isFc) fc.Paste(); else tb.Paste();
        }

        public void Clear()
        {
            if (isFc) fc.Clear(); else tb.Clear();
        }

        public void SelectAll()
        {
            if (isFc) fc.SelectAll(); else tb.SelectAll();
        }

        public bool CanSelect
        {
            get
            {
                return isFc ? fc.CanSelect : tb.CanSelect;
            }
        }

        public bool CanUndo
        {
            get
            {
                return isFc ? fc.UndoEnabled : tb.CanUndo;
            }
        }
        public bool CanRedo
        {
            get
            {
                return isFc ? fc.RedoEnabled : (tb as RichTextBox)?.CanRedo ?? false;
            }
        }

        public string SelectedText
        {
            get { return isFc ? fc.SelectedText : tb.SelectedText; }
            set { if (isFc) fc.SelectedText = value; else tb.SelectedText = value; }
        }

        public string Text
        {
            get { return isFc ? fc.Text : tb.Text; }
            set { if (isFc) fc.Text = value; else tb.Text = value; }
        }

    }
}
