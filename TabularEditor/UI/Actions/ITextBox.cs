using System.Windows.Forms;

namespace TabularEditor.UI.Actions
{
    public interface ITextBox
    {
        bool CanRedo { get; }
        bool CanSelect { get; }
        bool CanUndo { get; }
        bool ReadOnly { get; }
        string SelectedText { get; set; }
        int SelectionLength { get; }
        string Text { get; set; }

        void Clear();
        void Copy();
        void Cut();
        void Paste();
        void Redo();
        void SelectAll();
        void Undo();
        Control Parent { get; }
    }
}