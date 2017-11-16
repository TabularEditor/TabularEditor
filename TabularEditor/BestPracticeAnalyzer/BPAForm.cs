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

namespace TabularEditor.UI.Dialogs
{
    public partial class BPAForm : Form
    {
        ListViewGroup lvgLocal;
        ListViewGroup lvgGlobal;

        Analyzer analyzer;

        public Dictionary<string, BestPracticeRule> RuleIndex = new Dictionary<string, BestPracticeRule>();

        public Model Model { get { return analyzer.Model; } set { SetModel(value); } }
        public TreeViewAdv ModelTree { get; set; }
        public FormMain FormMain { get; set; }

        private void SetModel(Model model)
        {
            btnAdd.Enabled = model != null;
            btnAnalyzeAll.Enabled = model != null;
            analyzer.Model = model;
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
            listView1.SelectedIndices.Clear();
            Analyze();
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = listView2.GetItemAt(e.X, e.Y);
            if (item != null) Goto(item);
        }

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

        public void Analyze(IEnumerable<BestPracticeRule> rules)
        {
            if (Model != null)
            {
                listView2.Items.Clear();
                var results = analyzer.Analyze(rules).ToList();
                listView2.Items.AddRange(
                    results.Where(r => !r.InvalidCompatibilityLevel && !r.RuleHasError && !r.Ignored).Select(r => new ListViewItem(new[] { r.ObjectName, r.Object.GetTypeName(), r.RuleName, r.Rule.ID }) { Tag = r.Object }).ToArray());

                var oC = listView2.Items.Count;
                var rC = rules.Count();
                toolStripStatusLabel1.Text = string.Format("{0} object{1} in violation of the selected rule{2}.", oC, oC == 1 ? "" : "s", rC == 1 ? "" : "s");

                var ruleWithError = results.FirstOrDefault(r => r.RuleHasError);
                if (ruleWithError != null)
                    toolStripStatusLabel1.Text = string.Format("Rule error: {0}", ruleWithError.RuleError);
                else
                {
                    var ruleWithInvalidCL = results.FirstOrDefault(r => r.InvalidCompatibilityLevel);
                    if (ruleWithInvalidCL != null)
                        toolStripStatusLabel1.Text = string.Format("Rule '{0}' is not applicable to models of Compatibility Level {1}", ruleWithInvalidCL.Rule.Name, Model.Database.CompatibilityLevel);
                }
            }
        }

        public void Analyze(BestPracticeRule rule)
        {
            Analyze(Enumerable.Repeat(rule, 1));
        }

        public void Analyze()
        {
            Analyze(analyzer.GlobalRules.Concat(analyzer.LocalRules).Where(r => r.Enabled));
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
            if (listView2.SelectedItems.Count == 0) {
                e.Cancel = true;
                return;
            }
            var plural = listView2.SelectedItems.Count > 1;

            // SubItems[3] contains the ID of the respective rule:
            var rules = listView2.SelectedItems.Cast<ListViewItem>().Select(i => i.SubItems[3].Text).Distinct().Select(n => RuleIndex[n]).ToList();

            bpaResultGoTo.Visible = !plural;
            bpaResultGoToSep.Visible = !plural;

            var p = "Selected object" + (plural ? "s" : "");
            bpaResultIgnoreRule.Enabled = rules.Count == 1;
            bpaResultIgnoreSelected.Text = p;
            bpaResultScriptSelected.Text = p;
            bpaResultFixSelected.Text = p;

            var canFix = rules.Any(r => !string.IsNullOrEmpty(r.FixExpression));
            bpaResultScript.Enabled = canFix;
            bpaResultFix.Enabled = canFix;
        }

        private void bpaResultGoTo_Click(object sender, EventArgs e)
        {
            if(listView2.SelectedItems.Count == 1)
            {
                Goto(listView2.SelectedItems[0]);
            }
        }

        private void bpaResultIgnoreSelected_Click(object sender, EventArgs e)
        {
            bool unsupported = false;

            foreach (ListViewItem item in listView2.SelectedItems)
            {
                var rule = RuleIndex[item.SubItems[3].Text];
                var obj = item.Tag as IAnnotationObject;

                if (obj == null) unsupported = true;
                else analyzer.IgnoreRule(rule, true, obj);
            }

            if (unsupported)
            {
                MessageBox.Show("One or more of the selected objects does not support annotations. For this reason, the rule cannot be ignored on these objects.", "Cannot ignore rule", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void bpaResultIgnoreRule_Click(object sender, EventArgs e)
        {
            var rules = listView2.SelectedItems.Cast<ListViewItem>().Select(i => i.SubItems[3].Text).Distinct().Select(n => RuleIndex[n]).ToList();

            foreach (var rule in rules)
            {
                analyzer.IgnoreRule(rule);
                listView1.Items[rule.ID].Checked = false;
            }
        }

        private void bpaResultScriptSelected_Click(object sender, EventArgs e)
        {
            var script = string.Join("\n", listView2.SelectedItems.Cast<ListViewItem>().Select(
                i =>
                {
                    var obj = i.Tag as TabularNamedObject;
                    var rule = RuleIndex[i.SubItems[3].Text];
                    if (string.IsNullOrEmpty(rule.FixExpression)) return string.Format("// No automatic fix for rule '{0}' on object {1}", i.SubItems[2], i.SubItems[0]);
                    return obj.GetLinqPath() + "." + rule.FixExpression + ";";
                }
                ).ToArray());

            Clipboard.SetText(script);
            MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            editor.PopulateCategories(analyzer.AllRules);
            var newRule = editor.NewRule(analyzer.GetUniqueId("New Rule"));
            if (newRule != null)
            {
                analyzer.AddRule(newRule);
                analyzer.SaveLocalRulesToModel();
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
            if(listView2.SelectedItems.Count == 1)
            {
                var item = listView2.SelectedItems[0];
                var rule = RuleIndex[item.SubItems[3].Text];

                if (string.IsNullOrEmpty(rule.FixExpression))
                {
                    MessageBox.Show("No automatic fix exists on this rule.", "No automatic fix", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var script = string.Join("\n", analyzer.Analyze(rule).Select(
                    ar =>
                    {
                        var obj = ar.Object;
                        return obj.GetLinqPath() + "." + rule.FixExpression + ";";
                    }
                    ).ToArray());

                Clipboard.SetText(script);
                MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
    }
}
