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
            ruleDefinitionsModel = new RuleDefinitionsTreeModel(Model, Analyzer);
            tvRuleDefinitions.Model = ruleDefinitionsModel;
            tvRules.Model = rulesModel;
        }

        public static void Show(Model model, Analyzer analyzer)
        {
            var form = new BPAManager();
            form.Model = model;
            form.Analyzer = analyzer;
            form.Init();

            form.ShowDialog();
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
                rulesModel.SetRuleDefinition(tvRuleDefinitions.SelectedNode.Tag as RuleDefinition);
        }
    }

    public class RuleDefinition
    {
        public string Name { get; set; }
        public IEnumerable<BestPracticeRule> Rules { get; set; } = new List<BestPracticeRule>();
    }

    class RuleDefinitionsTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        List<RuleDefinition> ruleDefinitions = new List<RuleDefinition>();

        public RuleDefinitionsTreeModel(Model model, Analyzer analyzer)
        {
            ruleDefinitions.Add(new RuleDefinition { Name = "(Model rules)", Rules = analyzer.LocalRules });
            ruleDefinitions.Add(new RuleDefinition { Name = "(Local user rules)", Rules = analyzer.GlobalRules });
            ruleDefinitions.Add(new RuleDefinition { Name = "(Local machine rules)" });
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if(treePath.IsEmpty())
            {
                return ruleDefinitions;
            }
            return Enumerable.Empty<RuleDefinition>();
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

        public void SetRuleDefinition(RuleDefinition ruleDefinition)
        {
            rules = ruleDefinition?.Rules ?? Enumerable.Empty<BestPracticeRule>();
            StructureChanged?.Invoke(this, new TreePathEventArgs());
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                return rules;
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
