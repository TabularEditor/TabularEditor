using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.BestPracticeAnalyzer;
using TabularEditor.TOMWrapper;
using System.Linq.Dynamic;
using Aga.Controls.Tree;
using System.Collections;
using System.Threading;

namespace TabularEditor.UI.Dialogs
{
    public partial class BPAForm : Form
    {
        ListViewGroup lvgLocal;
        ListViewGroup lvgGlobal;

        Analyzer analyzer;

        public Dictionary<string, BestPracticeRule> RuleIndex = new Dictionary<string, BestPracticeRule>();

        public Model Model { get { return analyzer.Model; } set { SetModel(value); } }

        private void SetModel(Model model)
        {
            if (model != analyzer.Model)
            {
                btnAdd.Enabled = model != null;
                btnAnalyzeAll.Enabled = model != null;
                analyzer.Model = model;
                tvResults.Model = null;
                toolStripStatusLabel1.Text = "";
            }
        }

        public BPAForm()
        {
            InitializeComponent();

            lvgGlobal = listView1.Groups.Add("global", "Global Best Practices");
            lvgLocal = listView1.Groups.Add("local", "Model Specific Rules");

            analyzer = new Analyzer();
            analyzer.CollectionChanged += Analyzer_CollectionChanged;

            btnAnalyzeAll.Enabled = Model != null;
            btnAdd.Enabled = Model != null;
            btnEdit.Enabled = Model != null;

            tvResults.DefaultToolTipProvider = new AnalyzerResultTooltip();
        }

        private void Analyzer_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateListView();
        }

        private Color GetColor(int compatibilityLevel)
        {
            if (Model == null) return Color.Black;
            return Model.Database.CompatibilityLevel >= compatibilityLevel ? Color.Black : Color.Gray;
        }

        public void PopulateListView()
        {
            RuleIndex = analyzer.GlobalRules.Concat(analyzer.LocalRules).ToDictionary(r => r.ID, r => r);

            populatingList = true;

            var newItems = analyzer.GlobalRules.Select(r =>
                new ListViewItem(new[] { null, r.Name, r.ScopeString, r.Severity.ToString(), r.Category, r.Description }, lvgGlobal) { Name = r.ID, Checked = r.Enabled, Tag = r, ForeColor = GetColor(r.CompatibilityLevel) })
                .Concat(analyzer.LocalRules.Select(r =>
                new ListViewItem(new[] { null, r.Name, r.ScopeString, r.Severity.ToString(), r.Category, r.Description }, lvgLocal) { Name = r.ID, Checked = r.Enabled, Tag = r, ForeColor = GetColor(r.CompatibilityLevel) })).ToArray();

            listView1.Items.Clear();
            listView1.Items.AddRange(newItems);
            populatingList = false;
        }

        BPAEditorForm editor = new BPAEditorForm();

        private void btnAnalyzeAll_Click(object sender, EventArgs e)
        {
            AnalyzeAll();
        }

        //private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    var item = tvResults.GetItemAt(e.X, e.Y);
        //    if (item != null) Goto(item);
        //}

        public void Goto(ListViewItem item)
        {
            var obj = item.Tag as TabularNamedObject;
            UIController.Current.Goto(obj);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && e.Item != null)
            {
                Analyze(e.Item.Tag as BestPracticeRule);
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            var item = listView1.SelectedItems.Count == 1 ? listView1.SelectedItems[0] : null;

            btnDelete.Enabled = item?.Group == lvgLocal;
            btnMakeLocal.Text = item?.Group == lvgLocal ? "Make global" : "Make local";
            btnMakeLocal.Enabled = Model != null && item != null;

            btnEdit.Enabled = item != null;
            btnAdd.Enabled = Model != null;
        }

        internal AnalyzerResultsModel AnalyzerResultsTreeModel { get; private set; } = new AnalyzerResultsModel();

        /*public void PrepareUI()
        {
            BestPracticeRule rule = null;
            ListViewGroup group = null;
            groups = new List<ListViewGroup>();
            items = new List<ListViewItem>();
            foreach (var result in LatestAnalyzerResults.Where(r => !r.InvalidCompatibilityLevel && !r.RuleHasError && !r.Ignored))
            {
                if (result.Rule != rule)
                {
                    rule = result.Rule;
                    group = new ListViewGroup(rule.ID, rule.Name);
                    groups.Add(group);
                }
                var item = new ListViewItem(new[] {
                            result.ObjectName,
                            result.Object.GetTypeName(),
                            rule.ID }, group);
                item.ToolTipText = rule.Name;
                item.Tag = result.Object;
                items.Add(item);
            }
        }*/

        public void RefreshUI()
        {
            var oC = AnalyzerResultsTreeModel.ObjectCount;
            var rC = AnalyzerResultsTreeModel.RuleCount;
            toolStripStatusLabel1.Text = string.Format("{0} object{1} in violation of the selected rule{2}.", oC, oC == 1 ? "" : "s", rC == 1 ? "" : "s");

            /*var ruleWithError = LatestAnalyzerResults.FirstOrDefault(r => r.RuleHasError);
            if (ruleWithError != null)
                toolStripStatusLabel1.Text = string.Format("Rule error: {0}", ruleWithError.RuleError);
            else
            {
                var ruleWithInvalidCL = LatestAnalyzerResults.FirstOrDefault(r => r.InvalidCompatibilityLevel);
                if (ruleWithInvalidCL != null)
                    toolStripStatusLabel1.Text = string.Format("Rule '{0}' is not applicable to models of Compatibility Level {1}", ruleWithInvalidCL.Rule.Name, Model.Database.CompatibilityLevel);
            }*/
        }

        public void Analyze(IEnumerable<BestPracticeRule> rules)
        {
            AnalyzerResultsTreeModel.Update(analyzer.Analyze(rules));
            RefreshUI();
        }

        public void Analyze(BestPracticeRule rule)
        {
            Analyze(Enumerable.Repeat(rule, 1));
        }

        public void AnalyzeAll()
        {
            Analyze(analyzer.GlobalRules.Concat(analyzer.LocalRules).Where(r => r.Enabled));
        }

        /// <summary>
        /// Analyzes all rules.
        /// </summary>
        /// <param name="token"></param>
        public void AnalyzeAll(CancellationToken token)
        {
            AnalyzerResultsTreeModel.Update(analyzer.AnalyzeAll(token));
        }

        private bool populatingList;

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (populatingList) return;

            if (Model == null)
            {
                e.Item.Checked = false;
            }
            else
            {
                analyzer.IgnoreRule(e.Item.Tag as BestPracticeRule, !e.Item.Checked);
                (e.Item.Tag as BestPracticeRule).Enabled = e.Item.Checked;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //if (tvResults.SelectedItems.Count == 0) {
            //    e.Cancel = true;
            //    return;
            //}
            //var plural = tvResults.SelectedItems.Count > 1;

            //// SubItems[2] contains the ID of the respective rule:
            //var rules = tvResults.SelectedItems.Cast<ListViewItem>().Select(i => i.SubItems[2].Text).Distinct().Select(n => RuleIndex[n]).ToList();

            //bpaResultGoTo.Visible = !plural;
            //bpaResultGoToSep.Visible = !plural;

            //var p = "Selected object" + (plural ? "s" : "");
            //bpaResultIgnoreRule.Enabled = rules.Count == 1;
            //bpaResultIgnoreSelected.Text = p;
            //bpaResultScriptSelected.Text = p;
            //bpaResultFixSelected.Text = p;

            //var canFix = rules.Any(r => !string.IsNullOrEmpty(r.FixExpression));
            //bpaResultScript.Enabled = canFix;
            //bpaResultFix.Enabled = canFix;
        }

        private void bpaResultGoTo_Click(object sender, EventArgs e)
        {
            //if(tvResults.SelectedItems.Count == 1)
            //{
            //    Goto(tvResults.SelectedItems[0]);
            //}
        }

        private void bpaResultIgnoreSelected_Click(object sender, EventArgs e)
        {
            //bool unsupported = false;

            //foreach (ListViewItem item in tvResults.SelectedItems)
            //{
            //    var rule = RuleIndex[item.SubItems[3].Text];
            //    var obj = item.Tag as IAnnotationObject;

            //    if (obj == null) unsupported = true;
            //    else analyzer.IgnoreRule(rule, true, obj);
            //}

            //if (unsupported)
            //{
            //    MessageBox.Show("One or more of the selected objects does not support annotations. For this reason, the rule cannot be ignored on these objects.", "Cannot ignore rule", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        private void bpaResultIgnoreRule_Click(object sender, EventArgs e)
        {
            //var rules = tvResults.SelectedItems.Cast<ListViewItem>().Select(i => i.SubItems[3].Text).Distinct().Select(n => RuleIndex[n]).ToList();

            //foreach (var rule in rules)
            //{
            //    analyzer.IgnoreRule(rule);
            //    listView1.Items[rule.ID].Checked = false;
            //}
        }

        private void bpaResultScriptSelected_Click(object sender, EventArgs e)
        {
            //var script = string.Join("\n", tvResults.SelectedItems.Cast<ListViewItem>().Select(
            //    i =>
            //    {
            //        var obj = i.Tag as TabularNamedObject;
            //        var rule = RuleIndex[i.SubItems[3].Text];
            //        if (string.IsNullOrEmpty(rule.FixExpression)) return string.Format("// No automatic fix for rule '{0}' on object {1}", i.SubItems[2], i.SubItems[0]);
            //        return obj.GetLinqPath() + "." + rule.FixExpression + ";";
            //    }
            //    ).ToArray());

            //Clipboard.SetText(script);
            //MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            editor.PopulateCategories(analyzer.AllRules);
            var newRule = editor.NewRule(analyzer.GetUniqueId("New Rule"));
            if (newRule != null)
            {
                analyzer.AddRule(newRule);
                analyzer.SaveLocalRulesToModel();
                UIController.Current.InvokeBPABackground();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var rule = listView1.SelectedItems[0].Tag as BestPracticeRule;
                editor.PopulateCategories(analyzer.AllRules);
                var oldRuleId = rule.ID;
                if(editor.EditRule(rule))
                {
                    if(analyzer.LocalRules.Contains(rule))
                    {
                        analyzer.SaveLocalRulesToModel();
                    } else if(analyzer.GlobalRules.Contains(rule))
                    {
                        var bpc = new BestPracticeCollection();
                        bpc.Add(rule);

                        var globalRulesFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";

                        bpc.AddFromJsonFile(globalRulesFile);
                        if (oldRuleId != rule.ID) {
                            // ID changed - let's delete the rule with the old ID:
                            var oldRule = bpc.FirstOrDefault(r => r.ID == oldRuleId);
                            if (oldRule != null) bpc.Remove(oldRule);
                        }
                        bpc.SaveToFile(globalRulesFile);
                    }

                    PopulateListView();
                    UIController.Current.InvokeBPABackground();
                }
            }
        }

        private void BPAForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form instead of closing it:
            Hide();
            e.Cancel = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Group == lvgLocal)
            {
                var item = listView1.SelectedItems[0];
                var res = MessageBox.Show("Are you sure you want to delete this rule from the model?", "Delete rule?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel) return;

                analyzer.LocalRules.Remove(item.Tag as BestPracticeRule);
                analyzer.SaveLocalRulesToModel();
                item.Remove();
            }
        }

        private void LocalToGlobal()
        {
            // Convert a rule from local to global:
            var item = listView1.SelectedItems[0];
            var rule = item.Tag as BestPracticeRule;
            analyzer.LocalRules.Remove(rule);
            analyzer.GlobalRules.Add(rule);
            item.Group = lvgGlobal;

            analyzer.SaveLocalRulesToModel();

            // Save global rules (adding the newly promoted rule):
            var bpc = new BestPracticeCollection();
            bpc.Add(rule);

            var globalRulesFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";

            bpc.AddFromJsonFile(globalRulesFile);
            bpc.SaveToFile(globalRulesFile);
        }

        private void GlobalToLocal()
        {
            // Convert a rule from global to local:
            var item = listView1.SelectedItems[0];
            var rule = item.Tag as BestPracticeRule;
            analyzer.GlobalRules.Remove(rule);
            analyzer.LocalRules.Add(rule);
            item.Group = lvgLocal;

            analyzer.SaveLocalRulesToModel();

            // Save global rules (less the newly demoted rule):
            var bpc = new BestPracticeCollection();
            var globalRulesFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";
            bpc.AddFromJsonFile(globalRulesFile);
            var deleteRule = bpc.FirstOrDefault(r => r.ID.Equals(rule.ID, StringComparison.InvariantCultureIgnoreCase));
            if (deleteRule != null)
            {
                bpc.Remove(deleteRule);
                bpc.SaveToFile(globalRulesFile);
            }
        }

        private void btnMakeLocal_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Group == lvgLocal)
            {
                LocalToGlobal();
            }
            else
            if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Group == lvgGlobal)
            {
                GlobalToLocal();
            }

            UpdateUI();
        }

        private void bpaResultScriptRule_Click(object sender, EventArgs e)
        {
            //if(tvResults.SelectedItems.Count == 1)
            //{
            //    var item = tvResults.SelectedItems[0];
            //    var rule = RuleIndex[item.SubItems[3].Text];

            //    if (string.IsNullOrEmpty(rule.FixExpression))
            //    {
            //        MessageBox.Show("No automatic fix exists on this rule.", "No automatic fix", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }

            //    var script = string.Join("\n", analyzer.Analyze(rule).Select(
            //        ar =>
            //        {
            //            var obj = ar.Object;
            //            return obj.GetLinqPath() + "." + rule.FixExpression + ";";
            //        }
            //        ).ToArray());

            //    Clipboard.SetText(script);
            //    MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}
        }

        private int listView1SortColumn = -1;
        private int listView2SortColumn = -1;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var listView = sender as ListView;
            var sortColumn = listView == listView1 ? listView1SortColumn : listView2SortColumn;

            // Determine whether the column is the same as the last column clicked.
    if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                if (listView == listView1) listView1SortColumn = e.Column;
                else listView2SortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView.Sorting == SortOrder.Ascending)
                    listView.Sorting = SortOrder.Descending;
                else
                    listView.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            listView.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView.Sorting);
        }

        //C#
        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                        ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
            }
        }

        private void txtObjectName_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            if(e.Node.Tag is BestPracticeRule rule)
            {
                var objCount = AnalyzerResultsTreeModel.ObjectCountByRule(rule);
                e.Value = rule.Name + " (" + objCount + " object" + (objCount == 1 ? "" : "s") + ")";
            }
            else if(e.Node.Tag is AnalyzerResult result)
            {
                e.Value = result.ObjectName;
            }
        }

        public void ShowBPA()
        {
            tvResults.Model = AnalyzerResultsTreeModel;
            Show();
            BringToFront();
        }

        private void txtObjectName_DrawText(object sender, Aga.Controls.Tree.NodeControls.DrawEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule)
            {
                e.Font = new Font(e.Font, FontStyle.Bold);
                if (e.Control == txtObjectName)
                    e.FullRowDraw = true;
                else if (e.Control == txtObjectType)
                    e.SkipDraw = true;
            }
        }

        private void tvResults_RowDraw(object sender, TreeViewRowDrawEventArgs e)
        {
            
        }

        bool treeViewResizing = false;

        private void tvResults_Resize(object sender, EventArgs e)
        {
            AutofitColObject();
        }

        private void colObject_WidthChanged(object sender, EventArgs e)
        {
            if (!treeViewResizing)
            {
                colType.MinColumnWidth = 0;
                colType.MaxColumnWidth = 0;
                colType.Width = tvResults.ClientRectangle.Width - colObject.Width -
                    (tvResults.VerticalScrollbarVisible ? SystemInformation.VerticalScrollBarWidth : 0);
                colType.MinColumnWidth = colType.Width;
                colType.MaxColumnWidth = colType.Width;
            }
        }

        private void AutofitColObject()
        {
            treeViewResizing = true;
            colObject.Width = tvResults.ClientRectangle.Width - colType.Width -
                (tvResults.VerticalScrollbarVisible ? SystemInformation.VerticalScrollBarWidth : 0);
            treeViewResizing = false;
        }

        private void tvResults_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            AutofitColObject();
        }

        private void tvResults_Collapsed(object sender, TreeViewAdvEventArgs e)
        {
            AutofitColObject();
        }
    }
}
