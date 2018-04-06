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
using TabularEditor.TOMWrapper.Utils;

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

                ExpressionEditor_SetText();
            }
        }

        private void ExpressionEditor_Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name") UI.CurrentMeasureLabel.Text = 
                    ((sender as IDaxObject)?.DaxObjectName ?? 
                    ((sender as IExpressionObject).Name + ".Expression")) + " :=";
            if (e.PropertyName.EndsWith("Expression"))
            {
                if (sender == ExpressionEditor_Current) UI.ExpressionEditor.Text = GetText();
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

            UI.ExpressionSelector.ComboBox.ValueMember = "Value";
            UI.ExpressionSelector.ComboBox.DisplayMember = "Description";

            syntaxHighlightTimer.Tick += ExpressionEditor_SyntaxHighlightTick;
        }

        Task syntaxHighlightTask;

        private void ExpressionEditor_SyntaxHighlightTick(object sender, EventArgs e)
        {
            if(syntaxHighlightTask == null || syntaxHighlightTask.Status != TaskStatus.Running)
            {
                syntaxHighlightTimer.Enabled = false;
                syntaxHighlightTimer.Interval = 500;
                syntaxHighlightTask = new Task(() => ExpressionParser.SyntaxHighlight(UI.ExpressionEditor));
                syntaxHighlightTask.Start();
            }
        }

        private void ExpressionEditor_ExpressionSelectorChanged(object sender, EventArgs e)
        {
            if (ExpressionEditor_Current is IDaxDependantObject)
            {
                ExpressionEditor_SetText();
            }
        }

        sealed class DaxPropertyComboBoxItem
        {
            public string Description { get; private set; }
            public DAXProperty Value { get; private set; }
            public DaxPropertyComboBoxItem(DAXProperty value) { Value = value; Description = Enum.GetName(typeof(DAXProperty), value).SplitCamelCase(); }
            public static Dictionary<DAXProperty, DaxPropertyComboBoxItem> Items = new Dictionary<DAXProperty, DaxPropertyComboBoxItem>(
                Enum.GetValues(typeof(DAXProperty)).Cast<DAXProperty>().ToDictionary(v => v, v => new DaxPropertyComboBoxItem(v))
                );
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

        public void ExpressionEditor_BeginEdit()
        {
            if (!ExpressionEditor_IsEditing) ExpressionEditor_Edit(ExpressionEditor_Current, false);
        }

        Timer syntaxHighlightTimer = new Timer() { Interval = 500, Enabled = false };

        private void ExpressionEditor_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            ExpressionEditor_BeginEdit();

            if(syntaxHighlightTimer.Enabled) syntaxHighlightTimer.Enabled = false;
            syntaxHighlightTimer.Interval = 500;
            ExpressionEditor_SyntaxHighlight();
        }

        private void ExpressionEditor_SyntaxHighlight()
        {
            // For short DAX expressions, do synchronous syntax highlighting. Otherwise, do asynchrounous:
            if (UI.ExpressionEditor.Text.Length < 2000) ExpressionParser.SyntaxHighlight(UI.ExpressionEditor);
            else syntaxHighlightTimer.Enabled = true;
        }

        private string GetText()
        {
            if(ExpressionEditor_Current is IDaxDependantObject)
            {
                return (ExpressionEditor_Current as IDaxDependantObject).GetDAX(CurrentDaxProperty) ?? "";
            }
            else if (ExpressionEditor_Current != null)
            {
                return ExpressionEditor_Current.Expression ?? "";
            }
            return "";
        }
        private void SetText(string value)
        {
            if (ExpressionEditor_Current is IDaxDependantObject)
            {
                (ExpressionEditor_Current as IDaxDependantObject).SetDAX(CurrentDaxProperty, value);
            }
            else if (ExpressionEditor_Current != null)
            {
                ExpressionEditor_Current.Expression = value;
            }
        }

        private DAXProperty CurrentDaxProperty
        {
            get
            {
                return (UI.ExpressionSelector.SelectedItem as DaxPropertyComboBoxItem)?.Value ?? DAXProperty.Expression;
            }
            set
            {
                UI.ExpressionSelector.SelectedItem = DaxPropertyComboBoxItem.Items[value];
            }
        }

        private void ExpressionEditor_SetText()
        {
            UI.ExpressionEditor.TextChanged -= ExpressionEditor_TextChanged;

            var i = UI.ExpressionEditor.SelectionStart;
            UI.ExpressionEditor.Text = GetText();
            if (!string.IsNullOrEmpty(UI.ExpressionEditor.Text))
            {
                if (syntaxHighlightTimer.Enabled) syntaxHighlightTimer.Enabled = false;
                syntaxHighlightTimer.Interval = 1;
                ExpressionEditor_SyntaxHighlight();
            }
            UI.ExpressionEditor.ClearUndo();
            UI.ExpressionEditor.SelectionStart = i;

            // Only show label bar for DAX objects, when editing their expression:
            if (_expressionEditor_Current is IDaxObject)
            {
                UI.CurrentMeasureLabel.Text = (_expressionEditor_Current as IDaxObject).DaxObjectName + " :=";
                UI.CurrentMeasureLabel.Visible = true;
            } else
            {
                UI.CurrentMeasureLabel.Visible = false;
            }

            UI.ExpressionEditor.TextChanged += ExpressionEditor_TextChanged;
        }

        private void ExpressionEditor_Preview()
        {
            // TODO: Consider inspecting the expression of partitions, to determine
            // if the expression editor should use SQL syntax highlighting. This would
            // typically be the case for partitions using OLE DB or SQLNCLI providers.

            var obj = UI.TreeView.SelectedNode?.Tag as IExpressionObject;

            // Tables have "Default Detail Rows Expressions", but only for CompatibilityLevel 1400 or newer.
            if (obj is Table && !(obj is CalculatedTable) && Handler.CompatibilityLevel < 1400) obj = null;

            if (ExpressionEditor_IsEditing) ExpressionEditor_AcceptEdit();

            // Update the Expression Selector combobox:
            UI.ExpressionSelector.ComboBox.SelectedValueChanged -= ExpressionEditor_ExpressionSelectorChanged;
            UI.ExpressionSelector.Items.Clear();
            var ddo = obj as IDaxDependantObject;
            if (ddo != null)
            {
                UI.ExpressionSelector.Enabled = true;
                foreach (var daxProp in ddo.GetDAXProperties()) UI.ExpressionSelector.Items.Add(DaxPropertyComboBoxItem.Items[daxProp]);
                CurrentDaxProperty = ddo.GetDefaultDAXProperty();
            }
            else
            {
                UI.ExpressionSelector.Enabled = false;
            }
            UI.ExpressionSelector.ComboBox.SelectedValueChanged += ExpressionEditor_ExpressionSelectorChanged;

            ExpressionEditor_Current = obj;
            UI.ExpressionEditor.Enabled = obj != null;
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
                ExpressionEditor_SetText();
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
                ExpressionEditor_SetText();
                UI.TreeView.Focus();
            }
        }

        public void ExpressionEditor_AcceptEdit()
        {
            if (ExpressionEditor_IsEditing)
            {
                UI.StatusLabel.Text = "";
                SetText(UI.ExpressionEditor.Text);
                ExpressionEditor_IsEditing = false;
            }
        }

        public bool ExpressionEditor_IsDirty
        {
            get
            {
                if (ExpressionEditor_IsEditing)
                    return GetText() != UI.ExpressionEditor.Text.Replace("\r", "");
                else return false;
            }
        }
    }
}
