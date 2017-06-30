using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        DependencyForm DependencyForm = new DependencyForm();

        public void ShowDependencies(IDaxObject dependantObject)
        {
            if (DependencyForm.IsDisposed) DependencyForm = new DependencyForm();
            DependencyForm.Owner = UI.FormMain;
            DependencyForm.RootObject = dependantObject;
            DependencyForm.Show();
        }

        private IExpressionObject _expressionEditor_Current = null;
        public IExpressionObject ExpressionEditor_Current {
            get {
                return _expressionEditor_Current;    
            }
            set
            {
                if (_expressionEditor_Current != null) _expressionEditor_Current.PropertyChanged -= ExpressionEditor_Current_PropertyChanged;
                _expressionEditor_Current = value;
                if (_expressionEditor_Current != null) _expressionEditor_Current.PropertyChanged += ExpressionEditor_Current_PropertyChanged;

                ExpressionEditor_SetText(_expressionEditor_Current != null);
            }
        }

        private void ExpressionEditor_Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name") UI.CurrentMeasureLabel.Text = (sender as IDAXExpressionObject).DaxObjectName + " :=";
            if (e.PropertyName == "Expression")
            {
                if (sender == ExpressionEditor_Current) UI.ExpressionEditor.Text = (sender as IExpressionObject).Expression;
                if (UI.PropertyGrid.SelectedObject == ExpressionEditor_Current) UI.PropertyGrid.Refresh();
                ExpressionEditor_CancelEdit();
            }
        }

        public bool ExpressionEditor_IsEditing { get; private set; } = false;

        private void ExpressionEditor_Init()
        {
            UI.ExpressionEditor.TextChanged += ExpressionEditor_TextChanged;
            //UI.ExpressionEditor.KeyUp += ExpressionEditor_KeyUp;
            UI.ExpressionEditor.KeyPress += ExpressionEditor_KeyPress;
            UI.ExpressionEditor.DragEnter += ExpressionEditor_DragEnter;
            UI.ExpressionEditor.DragLeave += ExpressionEditor_DragLeave;
        }

        private void ExpressionEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ExpressionEditor_IsEditing)
            {
                if (e.KeyChar == 27)
                {
                    ExpressionEditor_CancelEdit();
                    e.Handled = true;
                }
                else if (e.KeyChar == 10)
                {
                    ExpressionEditor_AcceptEdit();
                    e.Handled = true;
                }
            }
        }

        private string Tree_DragBackup;
        private IDataObject Tree_CurrentDragObject;

        private void ExpressionEditor_DragEnter(object sender, DragEventArgs e)
        {
            Tree_CurrentDragObject = e.Data;
            if (draggedNodes != null)
            {
                // To be able to drag objects into the Expression Editor, we must temporarily swap the String contents of the Drag Data object with the DAX name to be inserted.
                // But first, backup the current string contents, to make sure we can still retrieve it if the drag operation leaves the Expression Editor.
                Tree_DragBackup = (string)e.Data.GetData(typeof(string));
                if (draggedNodes[0].Tag is IDaxObject)
                {
                    e.Data.SetData((draggedNodes[0].Tag as IDaxObject).DaxObjectFullName);
                }
                else
                {
                    e.Data.SetData(typeof(string), null);
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void ExpressionEditor_DragLeave(object sender, EventArgs e)
        {
            // If a drag backup was set, restore the backup
            if (Tree_DragBackup != null)
            {
                Tree_CurrentDragObject.SetData(Tree_DragBackup);
            }
        }

        private void ExpressionEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
        }

        private void ExpressionEditor_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if(!ExpressionEditor_IsEditing) ExpressionEditor_Edit(ExpressionEditor_Current, false);

            if (!string.IsNullOrEmpty(UI.ExpressionEditor.Text)) ExpressionParser.SyntaxHighlight(UI.ExpressionEditor);

        }

        private void ExpressionEditor_SetText(bool fromMeasure)
        {
            UI.ExpressionEditor.TextChanged -= ExpressionEditor_TextChanged;

            var i = UI.ExpressionEditor.SelectionStart;
            UI.ExpressionEditor.Text = fromMeasure ? ExpressionEditor_Current.Expression : "";
            if(!string.IsNullOrEmpty(UI.ExpressionEditor.Text)) ExpressionParser.SyntaxHighlight(UI.ExpressionEditor);
            UI.ExpressionEditor.ClearUndo();
            UI.ExpressionEditor.SelectionStart = i;

            UI.CurrentMeasureLabel.Text = fromMeasure ? (_expressionEditor_Current as IDaxObject)?.DaxObjectName + " :=" : "";
            UI.CurrentMeasureLabel.Visible = fromMeasure && _expressionEditor_Current is IDaxObject;

            UI.ExpressionEditor.TextChanged += ExpressionEditor_TextChanged;
        }

        private void ExpressionEditor_Preview()
        {
            // TODO: Consider inspecting the expression of partitions, to determine
            // if the expression editor should use SQL syntax highlighting. This would
            // typically be the case for partitions using OLE DB or SQLNCLI providers.

            var obj = UI.TreeView.SelectedNode?.Tag as IExpressionObject;

            if(!ExpressionEditor_IsEditing)
            {
                ExpressionEditor_Current = obj;
                ExpressionEditor_SetText(obj != null);
                UI.ExpressionEditor.Enabled = obj != null;
            } 
        }

        private void ExpressionEditor_Edit(IExpressionObject obj, bool switchToTab = true)
        {
            // Make sure the ExpressionEditor tab page is visible:
            if (switchToTab)
            {
                var page = UI.ExpressionEditor.Parent as TabPage;
                if (page is TabPage)
                    (page.Parent as TabControl).SelectTab(page);
            }

            // Accept any previous edits:
            if (ExpressionEditor_IsEditing) ExpressionEditor_AcceptEdit();

            if (ExpressionEditor_Current != obj)
            {
                ExpressionEditor_Current = obj;
                ExpressionEditor_SetText(true);
            }
            UI.StatusLabel.Text = "Editing " + (obj as IDaxObject)?.DaxObjectFullName ?? obj.Name;
            ExpressionEditor_IsEditing = true;
            if(!UI.ExpressionEditor.Focused) UI.ExpressionEditor.SelectAll();
            UI.ExpressionEditor.Focus();
        }

        public void ExpressionEditor_CancelEdit()
        {
            if (ExpressionEditor_IsEditing)
            {
                UI.StatusLabel.Text = "";
                ExpressionEditor_IsEditing = false;
                ExpressionEditor_SetText(true);
                UI.TreeView.Focus();
            }
        }

        public void ExpressionEditor_AcceptEdit()
        {
            if (ExpressionEditor_IsEditing)
            {
                UI.StatusLabel.Text = "";
                ExpressionEditor_Current.Expression = UI.ExpressionEditor.Text;
                ExpressionEditor_IsEditing = false;
            }
        }

        public bool ExpressionEditor_IsDirty
        {
            get
            {
                if (ExpressionEditor_IsEditing)
                    return ExpressionEditor_Current.Expression != UI.ExpressionEditor.Text;
                else return false;
            }
        }
    }
}
