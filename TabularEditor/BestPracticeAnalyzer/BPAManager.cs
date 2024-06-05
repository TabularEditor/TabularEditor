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
using TabularEditor.UI.Dialogs;

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
            EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);

            ruleDefinitionsModel = new RuleDefinitionsTreeModel(Analyzer);
            tvRuleDefinitions.Model = ruleDefinitionsModel;
            tvRules.Model = rulesModel;

            UpdateUI();
        }

        HashSet<BestPracticeRule> EffectiveRules;

        public static void Show(Analyzer analyzer)
        {
            var model = analyzer.Model;
            var form = new BPAManager();
            form.lblNoModelWarning.Visible = model == null;
            form.Model = model;
            form.Analyzer = analyzer;
            form.Init();

            analyzer.UpdateEnabled();
            
            if (form.ShowDialog(ActiveForm) == DialogResult.OK)
            {
                if (model != null)
                {
                    UIController.Current.Handler.BeginUpdate("BPA rule management");

                    // Persist ignore changes to model:
                    var ignoreHandler = new AnalyzerIgnoreRules(model);
                    var newIgnored = new HashSet<string>(analyzer.EffectiveRules.Where(r => !r.Enabled).Select(r => r.ID));

                    foreach (var rule in newIgnored.Except(ignoreHandler.RuleIDs).ToList())
                        ignoreHandler.RuleIDs.Add(rule);
                    foreach (var rule in ignoreHandler.RuleIDs.Except(newIgnored).ToList())
                        ignoreHandler.RuleIDs.Remove(rule);
                    ignoreHandler.Save(model);

                    analyzer.ModelRules.Save(null);
                }

                analyzer.LocalUserRules.Save(analyzer.BasePath);
                analyzer.LocalMachineRules.Save(analyzer.BasePath);
                analyzer.SaveExternalRuleCollections();

                foreach (var externalRuleCollection in analyzer.ExternalRuleCollections) externalRuleCollection.Save(analyzer.BasePath);

                if (model != null)
                {
                    UIController.Current.Handler.EndUpdate();
                    UIController.Current.InvokeBPABackground(false);
                }
            }
            else
            {
                var addedRules = form.rulesModel.AddedRules;
                var deletedRules = form.rulesModel.DeletedRules;
                var modifiedRules = form.rulesModel.ModifiedRules;

                // Remove rules that were added:
                foreach (var ruleKvp in addedRules) { ruleKvp.Value.Rules.Remove(ruleKvp.Key); }

                // Restore rules that were deleted:
                foreach (var ruleKvp in deletedRules) { ruleKvp.Value.Add(ruleKvp.Key); }

                // Restore rules that were modified:
                foreach (var ruleKvp in modifiedRules) ruleKvp.Key.AssignFrom(ruleKvp.Value);

                // Restore rule enabled status:
                foreach (var rule in analyzer.EffectiveRules) rule.UpdateEnabled(model);

                // Restore external rule collections:
                analyzer.LoadExternalRuleCollections();
            }
            
        }

        private void btnRemoveRuleDefinition_Click(object sender, EventArgs e)
        {
            if (CurrentCollection?.Internal == false)
            {
                Analyzer.ExternalRuleCollections.Remove(CurrentCollection);
                ruleDefinitionsModel.DoStructureChanged();
            }
        }

        private void tvRuleDefinitions_SelectionChanged(object sender, EventArgs e)
        {
            EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);

            if (tvRuleDefinitions.SelectedNode == null)
                rulesModel.SetRuleDefinition(null);
            else
            {
                rulesModel.SetRuleDefinition(tvRuleDefinitions.SelectedNode.Tag as IRuleDefinition);
                colScope.IsVisible = CurrentCollection != null;
                colDefinition.IsVisible = CurrentCollection == null;
            }

            tvRules.ExpandAll();

            UpdateUI();
        }

        private void txtName_DrawText(object sender, Aga.Controls.Tree.NodeControls.DrawEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule rule)
            {
                if (!EffectiveRules.Contains(rule))
                    e.Font = new Font(e.Font, FontStyle.Strikeout);
            }
            else if (e.Node.Tag is RuleCategory) e.FullRowDraw = true;
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
        IEnumerable<BestPracticeRule> CurrentRules => tvRules.SelectedNodes.Select(n => n.Tag).OfType<BestPracticeRule>();

        private void UpdateUI()
        {
            btnRemoveRuleDefinition.Enabled = CurrentCollection?.Internal == false;
            btnUp.Enabled = CurrentCollection?.Internal == false && Analyzer.ExternalRuleCollections.IndexOf(CurrentCollection) > 0;
            btnDown.Enabled = CurrentCollection?.Internal == false && Analyzer.ExternalRuleCollections.IndexOf(CurrentCollection) < Analyzer.ExternalRuleCollections.Count - 1;

            btnNewRule.Enabled = CurrentCollection?.AllowEdit == true;
            btnEditRule.Enabled = CurrentRules.Count() == 1 && (CurrentCollection ?? Analyzer.EffectiveCollectionForRule(CurrentRules.FirstOrDefault().ID)).AllowEdit;
            btnClone.Enabled = CurrentCollection?.AllowEdit == true && CurrentRules.Count() == 1 && (CurrentCollection ?? Analyzer.EffectiveCollectionForRule(CurrentRules.FirstOrDefault().ID)).AllowEdit;
            btnDeleteRule.Enabled = CurrentRules.Any() && CurrentRules.All(r => (CurrentCollection ?? Analyzer.EffectiveCollectionForRule(r.ID)).AllowEdit);
            btnMoveTo.Enabled = CurrentCollection != null && CurrentRules.Any();
        }

        private BPAEditorForm editor = new BPAEditorForm();

        private void btnNewRule_Click(object sender, EventArgs e)
        {
            editor.PopulateCategories(Analyzer.EffectiveRules);
            var newRule = editor.NewRule(Analyzer.GetUniqueId("New Rule"));
            if (newRule != null)
            {
                rulesModel.AddedRules[newRule] = CurrentCollection;
                CurrentCollection.Add(newRule);
                EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
                rulesModel.RefreshCategories();
                var node = tvRules.FindNodeByTag(newRule);
                if(node != null)
                {
                    tvRules.EnsureVisible(node);
                    tvRules.SelectedNode = node;
                    tvRules.Focus();
                }
            }
        }

        private void btnEditRule_Click(object sender, EventArgs e)
        {
            var rule = CurrentRules.FirstOrDefault();
            if (rule == null) return;
            
            var orgRule = rulesModel.ModifiedRules.ContainsKey(rule) ? rulesModel.ModifiedRules[rule] : rule.Clone();

            editor.PopulateCategories(Analyzer.EffectiveRules);
            if (editor.EditRule(rule))
            {
                rulesModel.ModifiedRules[rule] = orgRule;
                EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
                rulesModel.RefreshCategories();
                tvRules.Focus();
            }
        }

        private void btnDeleteRule_Click(object sender, EventArgs e)
        {
            foreach (var rule in CurrentRules)
            {
                var currentCollection = CurrentCollection ?? Analyzer.EffectiveCollectionForRule(rule.ID);
                rulesModel.DeletedRules[rule] = currentCollection;
                currentCollection.Rules.Remove(rule);
            }
            EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
            rulesModel.RefreshCategories();
        }

        private void chkRuleEnabled_CheckStateChanged(object sender, TreePathEventArgs e)
        {
            var ignoredRule = e.Path.LastNode as BestPracticeRule;
            foreach (var rule in Analyzer.AllRules.Where(r => r.ID.EqualsI(ignoredRule.ID))) rule.Enabled = ignoredRule.Enabled;
        }

        private void chkRuleEnabled_IsVisibleValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            e.Value = e.Node.Tag is BestPracticeRule;
        }

        private void txtDefinition_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule rule)
            {
                e.Value = CurrentCollection?.Name ?? Analyzer.EffectiveCollectionForRule(rule.ID).Name;
            }
        }

        private void btnAddRuleDefinition_Click(object sender, EventArgs e)
        {
            if (BPAManagerAddCollectionDialog.Show(Analyzer, this))
            {
                ruleDefinitionsModel.DoStructureChanged();
            }
            Activate();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var currentPrecedence = Analyzer.ExternalRuleCollections.IndexOf(CurrentCollection);
            if (CurrentCollection?.Internal == false && currentPrecedence < Analyzer.ExternalRuleCollections.Count - 1)
            {
                Analyzer.ExternalRuleCollections.RemoveAt(currentPrecedence);
                Analyzer.ExternalRuleCollections.Insert(currentPrecedence + 1, CurrentCollection);
                EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
                ruleDefinitionsModel.DoStructureChanged();
                tvRules.Invalidate();
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var currentPrecedence = Analyzer.ExternalRuleCollections.IndexOf(CurrentCollection);
            if (CurrentCollection?.Internal == false && currentPrecedence > 0)
            {
                Analyzer.ExternalRuleCollections.RemoveAt(currentPrecedence);
                Analyzer.ExternalRuleCollections.Insert(currentPrecedence - 1, CurrentCollection);
                EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
                ruleDefinitionsModel.DoStructureChanged();
                tvRules.Invalidate();
            }
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            var rule = CurrentRules.FirstOrDefault();
            if (rule != null) {
                var newRule = rule.Clone();
                newRule.ID = Analyzer.GetUniqueId(newRule.ID);

                rulesModel.AddedRules[newRule] = CurrentCollection;
                CurrentCollection.Add(newRule);
                EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
                rulesModel.RefreshCategories();
                var node = tvRules.FindNodeByTag(newRule);
                if (node != null)
                {
                    tvRules.EnsureVisible(node);
                    tvRules.SelectedNode = node;
                    tvRules.Focus();
                }
            }
        }

        private void btnMoveTo_Click(object sender, EventArgs e)
        {
            if(MoveToCollectionDialog.Show(CurrentCollection, CurrentRules.Count() > 1, Analyzer, out BestPracticeCollection destination, out bool clone))
            {
                foreach (var rule in CurrentRules.ToList())
                {
                    var currentCollection = CurrentCollection ?? Analyzer.EffectiveCollectionForRule(rule.ID);

                    if (clone)
                    {
                        var newRule = rule.Clone();
                        destination.Rules.Add(newRule);
                        rulesModel.AddedRules[newRule] = destination;
                    }
                    else
                    {
                        rulesModel.DeletedRules[rule] = currentCollection;
                        currentCollection.Rules.Remove(rule);

                        rulesModel.AddedRules[rule] = destination;
                        destination.Rules.Add(rule);
                    }
                }

                EffectiveRules = new HashSet<BestPracticeRule>(Analyzer.EffectiveRules);
                rulesModel.RefreshCategories();
            }
        }
    }

    public interface IRuleDefinition
    {
        string Name { get; }
        IEnumerable<BestPracticeRule> Rules { get; }
        bool Internal { get; }
    }

    public class EffectiveRules: IRuleDefinition
    {
        Analyzer analyzer;

        public string Name => "(Effective rules)";
        public EffectiveRules(Analyzer analyzer)
        {
            this.analyzer = analyzer;
        }
        public IEnumerable<BestPracticeRule> Rules => analyzer.EffectiveRules;
        public bool Internal => true;
    }

    class RuleDefinitionsTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        Analyzer analyzer;
        EffectiveRules effectiveRules;

        public RuleDefinitionsTreeModel(Analyzer analyzer)
        {
            this.analyzer = analyzer;
            effectiveRules = new EffectiveRules(analyzer);
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if(treePath.IsEmpty())
            {
                if (effectiveRules != null) yield return effectiveRules;
                if (analyzer.ModelRules != null) yield return analyzer.ModelRules;
                foreach (var externalRules in analyzer.ExternalRuleCollections) yield return externalRules;
                if (analyzer.LocalUserRules != null) yield return analyzer.LocalUserRules;
                if (analyzer.LocalMachineRules != null) yield return analyzer.LocalMachineRules;
            }
        }

        public bool IsLeaf(TreePath treePath)
        {
            return true;
        }

        public void DoStructureChanged()
        {
            StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
        }
    }

    class RuleCategory
    {
        public string Name { get; set; }
        public List<BestPracticeRule> Rules { get; set; }
    }

    class RulesTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        IEnumerable<BestPracticeRule> rules = Enumerable.Empty<BestPracticeRule>();

        public Dictionary<BestPracticeRule, BestPracticeCollection> DeletedRules = new Dictionary<BestPracticeRule, BestPracticeCollection>();
        public Dictionary<BestPracticeRule, BestPracticeCollection> AddedRules = new Dictionary<BestPracticeRule, BestPracticeCollection>();

        /// <summary>
        /// Links the original (unmodified) rule to the modified rule:
        /// </summary>
        public Dictionary<BestPracticeRule, BestPracticeRule> ModifiedRules = new Dictionary<BestPracticeRule, BestPracticeRule>();
        IRuleDefinition currentCollection;
        Dictionary<string, RuleCategory> categories = new Dictionary<string, RuleCategory>(StringComparer.InvariantCultureIgnoreCase);

        public void SetRuleDefinition(IRuleDefinition collection)
        {
            currentCollection = collection;
            RefreshCategories();
        }

        public void RefreshCategories()
        {
            rules = currentCollection?.Rules ?? Enumerable.Empty<BestPracticeRule>();
            foreach (var category in categories.Values) category.Rules.Clear();
            foreach(var ruleGroup in rules.GroupBy(r => r.Category.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                RuleCategory category;
                if (!categories.TryGetValue(ruleGroup.Key, out category))
                {
                    category = new RuleCategory
                    {
                        Name = string.IsNullOrWhiteSpace(ruleGroup.Key) ? "(Uncategorized)" : ruleGroup.Key,
                        Rules = new List<BestPracticeRule>()
                    };
                    categories.Add(ruleGroup.Key, category);
                }
                category.Rules.AddRange(ruleGroup);
            }

            DoStructureChanged();
        }

        public void DoStructureChanged()
        {
            StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                return categories.Values.Where(c => c.Rules.Count > 0).OrderBy(c => c.Name);
            }
            else if (treePath.LastNode is RuleCategory c)
            {
                return c.Rules.OrderBy(r => r.Name);
            }
            else
                return Enumerable.Empty<BestPracticeRule>();
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (treePath.LastNode is RuleCategory) return false;
            else return true;
        }
    }
}
