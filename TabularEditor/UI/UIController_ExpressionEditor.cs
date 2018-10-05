using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UIServices;

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
                syntaxHighlightTask = new Task(() => currentTokens = ExpressionParser.SyntaxHighlight(UI.ExpressionEditor));
                syntaxHighlightTask.Start();
            }
        }

        private List<Antlr4.Runtime.IToken> currentTokens;

        private void ExpressionEditor_ExpressionSelectorChanged(object sender, EventArgs e)
        {
            if (ExpressionEditor_Current is IDaxDependantObject)
            {
                SetText(UI.ExpressionEditor.Text, LastDaxProperty);
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

        public void ExpressionEditor_GoToDefinition()
        {
            // Navigate to symbol under cursor
            var obj = ExpressionEditor_Current as IDaxDependantObject;
            var dependsOnList = DependsOnList.GetDependencies(obj, UI.ExpressionEditor.Text, CurrentDaxProperty);

            var dest = dependsOnList.GetObjectAt(CurrentDaxProperty, UI.ExpressionEditor.SelectionStart);
            if (dest == null)
            {
                var token = ExpressionParser.GetTokenAtPos(currentTokens, UI.ExpressionEditor.SelectionStart);
                if (token != null)
                {
                    if (token.Channel == DAXLexer.KEYWORD_CHANNEL) Process.Start("https://dax.guide/" + token.Text.ToLowerInvariant());
                }
                else
                    UI.StatusLabel.Text = "Cannot navigate to symbol under cursor.";
            }
            else
                Goto(dest);
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

        bool ExpressionEditor_SuspendTextChanged = false;

        private void ExpressionEditor_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (ExpressionEditor_SuspendTextChanged) return;

            ExpressionEditor_BeginEdit();

            if(syntaxHighlightTimer.Enabled) syntaxHighlightTimer.Enabled = false;
            syntaxHighlightTimer.Interval = 500;
            ExpressionEditor_SyntaxHighlight();
        }

        public bool ExpressionEditor_IsDax
        {
            get
            {
                if (ExpressionEditor_Current == null) return false;
                switch (ExpressionEditor_Current.ObjectType)
                {
                    case ObjectType.Measure:
                    case ObjectType.Table:
                    case ObjectType.Column:
                    case ObjectType.KPI:
                    case ObjectType.RLSFilterExpression:
                        // These are the only object types that contain DAX expression properties:
                        return true;
                    default:
                        // Other object types (partitions) do typically not contain DAX expressions:
                        return false;
                }
            }
        }

        private void ExpressionEditor_SyntaxHighlight()
        {
            if (!ExpressionEditor_IsDax) return;

            Console.WriteLine("Do syntax highlight");

            // For short DAX expressions, do synchronous syntax highlighting. Otherwise, do asynchrounous:
            if (UI.ExpressionEditor.Text.Length < 2000) currentTokens = ExpressionParser.SyntaxHighlight(UI.ExpressionEditor);
            else syntaxHighlightTimer.Enabled = true;
        }

        public void ExpressionEditor_SwitchToSemicolons()
        {
            if (ExpressionEditor_IsDax)
            {
                ExpressionEditor_SuspendTextChanged = true;
                UI.ExpressionEditor.Text = ExpressionParser.CommasToSemicolons(UI.ExpressionEditor.Text);
                ExpressionEditor_SuspendTextChanged = false;
                ExpressionParser.SyntaxHighlight(UI.ExpressionEditor);
            }
        }

        public void ExpressionEditor_SwitchToCommas()
        {
            if (ExpressionEditor_IsDax)
            {
                ExpressionEditor_SuspendTextChanged = true;
                UI.ExpressionEditor.Text = ExpressionParser.SemicolonsToCommas(UI.ExpressionEditor.Text);
                ExpressionEditor_SuspendTextChanged = false;
                ExpressionParser.SyntaxHighlight(UI.ExpressionEditor);
            }
        }

        private string GetText()
        {
            string value;

            if(ExpressionEditor_Current is IDaxDependantObject)
            {
                value = (ExpressionEditor_Current as IDaxDependantObject).GetDAX(CurrentDaxProperty) ?? "";
            }
            else if (ExpressionEditor_Current != null)
            {
                value = ExpressionEditor_Current.Expression ?? "";
            }
            else return "";

            // Do semicolon replacement if needed:
            if (ExpressionEditor_IsDax && Preferences.Current.UseSemicolonsAsSeparators)
                value = ExpressionParser.CommasToSemicolons(value);

            return value;
        }

        private void SetText(string value)
        {
            SetText(value, CurrentDaxProperty);
        }

        private void SetText(string value, DAXProperty prop)
        {
            // Do semicolon replacement if needed:
            if (ExpressionEditor_IsDax && Preferences.Current.UseSemicolonsAsSeparators)
                value = ExpressionParser.SemicolonsToCommas(value);

            if (ExpressionEditor_Current is IDaxDependantObject)
            {
                (ExpressionEditor_Current as IDaxDependantObject).SetDAX(prop, value);
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
        private DAXProperty LastDaxProperty;

        private void ExpressionEditor_SetText()
        {
            ExpressionEditor_SuspendTextChanged = true;

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
            }
            else
            {
                UI.CurrentMeasureLabel.Visible = false;
            }

            ExpressionEditor_SuspendTextChanged = false;

            LastDaxProperty = CurrentDaxProperty;
        }

        private void ExpressionEditor_Preview(IExpressionObject obj)
        {
            // TODO: Consider inspecting the expression of partitions, to determine
            // if the expression editor should use SQL syntax highlighting. This would
            // typically be the case for partitions using OLE DB or SQLNCLI providers.

            if (ExpressionEditor_Current != null)
            {
                (IsNavigatingBack ? Forward : Back).Push(ExpressionEditor_Current);
            }

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
