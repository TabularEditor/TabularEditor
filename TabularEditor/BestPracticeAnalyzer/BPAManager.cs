using Aga.Controls.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI;

namespace TabularEditor.BestPracticeAnalyzer
{
    public partial class BPAManager : Form
    {
        public BPAManager()
        {
            InitializeComponent();
        }

        Model Model;
        Analyzer Analyzer;

        RulesTreeModel rulesModel = new RulesTreeModel();
        RuleDefinitionsTreeModel ruleDefinitionsModel;

        private void Init()
        {
            EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.AllRules);

            ruleDefinitionsModel = new RuleDefinitionsTreeModel(Model, Analyzer);
            tvRuleDefinitions.Model = ruleDefinitionsModel;
            tvRules.Model = rulesModel;

            UpdateUI();
        }

        HashSet<BestPracticeRule> EffectiveRules;

        public static void Show(Model model, Analyzer analyzer)
        {
            var form = new BPAManager();
            form.Model = model;
            form.Analyzer = analyzer;
            form.Init();

            foreach (var rule in analyzer.ModelRules) rule.UpdateEnabled(model);
            foreach (var rule in analyzer.LocalUserRules) rule.UpdateEnabled(model);
            foreach (var rule in analyzer.LocalMachineRules) rule.UpdateEnabled(model);

            if (form.ShowDialog() == DialogResult.OK)
            {
                UIController.Current.Handler.BeginUpdate("BPA rule management");

                // Persist ignore changes to model:
                var ignoreHandler = new AnalyzerIgnoreRules(model);
                var newIgnored = new HashSet<string>(analyzer.AllRules.Where(r => !r.Enabled).Select(r => r.ID));

                foreach (var rule in newIgnored.Except(ignoreHandler.RuleIDs).ToList())
                    ignoreHandler.RuleIDs.Add(rule);
                foreach (var rule in ignoreHandler.RuleIDs.Except(newIgnored).ToList())
                    ignoreHandler.RuleIDs.Remove(rule);
                ignoreHandler.Save(model);

                // Persist rule removal/additions to model / files:
                foreach (var kvp in form.rulesModel.ToBeDeleted) kvp.Value.Rules.Remove(kvp.Key);
                foreach (var kvp in form.rulesModel.ToBeAdded) kvp.Value.Add(kvp.Key);
                UIController.Current.Handler.EndUpdate();
            }
            else
            {
                foreach (var rule in analyzer.AllRules) rule.UpdateEnabled(model);
            }

        }

        private void btnRemoveRuleDefinition_Click(object sender, EventArgs e)
        {
            //lvRuleDefinitions.SelectedItems[0].Remove();
            //DoResize();
        }

        private void tvRuleDefinitions_SelectionChanged(object sender, EventArgs e)
        {
            if (tvRuleDefinitions.SelectedNode == null)
                rulesModel.SetRuleDefinition(null);
            else
                rulesModel.SetRuleDefinition(tvRuleDefinitions.SelectedNode.Tag as BestPracticeCollection);

            UpdateUI();
        }

        private void txtName_DrawText(object sender, Aga.Controls.Tree.NodeControls.DrawEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule rule)
            {
                if(!EffectiveRules.Contains(rule))
                    e.Font = new Font(e.Font, FontStyle.Strikeout);
            }
        }

        private void chkRuleEnabled_IsEditEnabledValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule rule)
                e.Value = EffectiveRules.Contains(rule);
        }

        private void tvRules_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        BestPracticeCollection CurrentCollection => tvRuleDefinitions.SelectedNode?.Tag as BestPracticeCollection;
        IEnumerable<BestPracticeRule> CurrentRules => tvRules.SelectedNodes.Select(n => n.Tag).Cast<BestPracticeRule>();

        private void UpdateUI()
        {
            btnRemoveRuleDefinition.Enabled = !(CurrentCollection?.Internal ?? true);

            btnNewRule.Enabled = CurrentCollection?.AllowEdit ?? false;
            btnEditRule.Enabled = (CurrentCollection?.AllowEdit ?? false) && tvRules.SelectedNodes.Count == 1;
            btnDeleteRule.Enabled = (CurrentCollection?.AllowEdit ?? false) && tvRules.SelectedNodes.Count > 0;
            btnCopyTo.Enabled = tvRules.SelectedNodes.Count > 0;
            btnMoveTo.Enabled = tvRules.SelectedNodes.Count > 0;
        }

        private void btnNewRule_Click(object sender, EventArgs e)
        {
            //rulesModel.ToBeAdded[rule] = CurrentCollection;
            //rulesModel.DoStructureChanged();
        }

        private void btnEditRule_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteRule_Click(object sender, EventArgs e)
        {
            foreach (var rule in CurrentRules) rulesModel.ToBeDeleted[rule] = CurrentCollection;
            rulesModel.DoStructureChanged();
        }

        private void chkRuleEnabled_CheckStateChanged(object sender, TreePathEventArgs e)
        {
            var ignoredRule = e.Path.LastNode as BestPracticeRule;

            foreach (var rule in Analyzer.ModelRules) if (rule.ID.EqualsI(ignoredRule.ID)) rule.Enabled = ignoredRule.Enabled;
            foreach (var rule in Analyzer.LocalUserRules) if (rule.ID.EqualsI(ignoredRule.ID)) rule.Enabled = ignoredRule.Enabled;
            foreach (var rule in Analyzer.LocalMachineRules) if (rule.ID.EqualsI(ignoredRule.ID)) rule.Enabled = ignoredRule.Enabled;
        }
    }

    class RuleDefinitionsTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        List<BestPracticeCollection> ruleDefinitions = new List<BestPracticeCollection>();

        public RuleDefinitionsTreeModel(Model model, Analyzer analyzer)
        {
            ruleDefinitions.Add(analyzer.ModelRules);
            ruleDefinitions.Add(analyzer.LocalUserRules);
            ruleDefinitions.Add(analyzer.LocalMachineRules);
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if(treePath.IsEmpty())
            {
                return ruleDefinitions;
            }
            return Enumerable.Empty<BestPracticeCollection>();
        }

        public bool IsLeaf(TreePath treePath)
        {
            return true;
        }
    }

    class RulesTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        IEnumerable<BestPracticeRule> rules = Enumerable.Empty<BestPracticeRule>();

        public Dictionary<BestPracticeRule, BestPracticeCollection> ToBeDeleted = new Dictionary<BestPracticeRule, BestPracticeCollection>();
        public Dictionary<BestPracticeRule, BestPracticeCollection> ToBeAdded = new Dictionary<BestPracticeRule, BestPracticeCollection>();
        BestPracticeCollection currentCollection;

        public void SetRuleDefinition(BestPracticeCollection collection)
        {
            currentCollection = collection;
            rules = collection?.Rules ?? Enumerable.Empty<BestPracticeRule>();
            DoStructureChanged();
        }

        public void DoStructureChanged()
        {
            StructureChanged?.Invoke(this, new TreePathEventArgs());
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                return rules.Except(ToBeDeleted.Keys).Concat(ToBeAdded.Where(kvp => kvp.Value == currentCollection).Select(kvp => kvp.Key));
            }
            else
                return Enumerable.Empty<BestPracticeRule>();
        }

        public bool IsLeaf(TreePath treePath)
        {
            return true;
        }
    }
}
