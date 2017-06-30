using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Dynamic;
using TabularEditor.BestPracticeAnalyzer;

namespace TabularEditor.UI.Dialogs
{
    public partial class BPAEditorForm : Form
    {
        public bool EditRule(BestPracticeRule rule)
        {
            Expression = rule.Expression;
            Scope = rule.Scope;
            nameBefore = rule.Name;
            txtName.Text = rule.Name;
            txtID.Text = rule.ID;
            txtDescription.Text = rule.Description;
            numSeverity.Value = rule.Severity;

            btnOK.Enabled = ValidateRule();

            if(ShowDialog() == DialogResult.OK)
            {
                rule.Expression = Expression;
                rule.Scope = Scope;
                rule.Name = txtName.Text;
                rule.Description = txtDescription.Text;
                rule.Severity = (int)numSeverity.Value;
                return true;
            }
            return false;
        }

        private string GetStandardIdFromName(string name)
        {
            return name.ToUpper().Replace(" ", "_");
        }

        public BestPracticeRule NewRule(string name)
        {
            Expression = "";
            Scope = RuleScope.Model;
            nameBefore = name;
            txtName.Text = name;
            txtID.Text = GetStandardIdFromName(name);
            txtDescription.Text = "";
            numSeverity.Value = 10;

            btnOK.Enabled = ValidateRule();

            if (ShowDialog() == DialogResult.OK)
            {
                return new BestPracticeRule {
                    Expression = Expression,
                    Scope = Scope,
                    Name = txtName.Text,
                    ID = txtID.Text,
                    Description = txtDescription.Text,
                    Severity = (int)numSeverity.Value,
                    Enabled = true
                };
            }
            else
                return null;
        }

        public BPAEditorForm()
        {
            InitializeComponent();

            lb = new CheckedListBox();
            lb.CheckOnClick = true;
            lb.FormattingEnabled = true;
            lb.Items.AddRange(Enum.GetValues(typeof(RuleScope)).Cast<RuleScope>().Select(v => v.GetTypeName()).OrderBy(v => v).ToArray());
            lb.BorderStyle = BorderStyle.None;
            lb.ItemCheck += (s, e) => {
                var selection = lb.Items.Cast<string>().Where(
                    i => (((string)lb.Items[e.Index]) != i && lb.CheckedItems.Contains(i))
                        || (((string)lb.Items[e.Index]) == i && e.NewValue == CheckState.Checked));

                customComboBox1.Text = string.Join(",", selection);

                _scope = selection.Select(scope => RuleScopeHelper.GetScope(scope)).Combine();
            };
            customComboBox1.DropDownControl = lb;
        }

        private CheckedListBox lb;

        public string Expression { get { return txtExpression.Text; } private set { txtExpression.Text = value; } }
        public IEnumerable<Type> ScopeTypes
        {
            get { return _scope.Enumerate().Select(s => s.GetScopeType()); }
        }

        private RuleScope _scope;
        public RuleScope Scope
        {
            get
            {
                return _scope;
            }
            private set
            {
                _scope = value;

                // Update UI:
                foreach (int i in lb.CheckedIndices) lb.SetItemChecked(i, false);
                foreach (int i in value.Enumerate().Select(s => lb.Items.IndexOf(s.GetTypeName()))) lb.SetItemChecked(i, true);
            }
        }

        private void RecursivelyDispose(IEnumerable<Control> controls)
        {
            var controlList = controls.ToList();

            controlList.ForEach(c => RecursivelyDispose(c.Controls.Cast<Control>().ToList()));
            controlList.ForEach(c => c.Parent?.Controls?.Remove(c));
            controlList.ForEach(c => c.Dispose());
        }

        /// <summary>
        /// Call this method to validate the dynamic linq expression and rebuild the
        /// visual builder tree. Returns true if the expression was succesfully
        /// validated - false otherwise.
        /// 
        /// This method also displays an info-panel at the top of the screen, in case
        /// validation fails.
        /// </summary>
        /// <returns></returns>
        private bool ValidateRule()
        {
            var result = true;
            if(string.IsNullOrWhiteSpace(txtID.Text))
            {
                pnlInfo.Visible = true;
                lblInfo.Text = "ID cannot be blank";
                return false;
            }
            if(string.IsNullOrWhiteSpace(txtName.Text))
            {
                pnlInfo.Visible = true;
                lblInfo.Text = "Name cannot be blank";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Expression))
            {
                Root = new MultiNode();
                Root.AddNode(new CriteriaNode());
                pnlInfo.Visible = false;
                return false;
            }
            else
            {
                try
                {
                    // Attempt to parse the expression of the assigned rule:
                    foreach (var t in Scope.Enumerate().Select(s => s.GetScopeType()))
                    {
                        var expr = DynamicExpression.ParseLambda(t, typeof(bool), Expression);
                    }

                    // TODO: Uncomment below code to re-enable visual tree builder
                    //var rootNode = CriteriaTreeBuilder.BuildFromExpression(expr.Body);
                    //if (!(rootNode is MultiNode)) Root = rootNode.PromoteToMulti();
                    //else Root = rootNode as MultiNode;

                    pnlInfo.Visible = false;
                }
                catch (Exception ex)
                {
                    lblInfo.Text = "Error: " + ex.Message;
                    pnlInfo.Visible = true;
                    result = false;
                }
            }

            RefreshPanels();

            return result;
        }

        private void RefreshPanels()
        {
            // TODO: To re-enable the visual tree builder, add a panel somewhere on the form and change
            // the name to "pnlTree". Then, remove the comment on the below code:

            // TODO: Uncomment below code to re-enable visual tree builder
            //pnlTree.SuspendDrawing();
            //RecursivelyDispose(pnlTree.Controls.Cast<Control>());

            //if (Root.HasUnparsedChilds)
            //{
            //    lblInfo.Text = "This expression cannot be represented visually. Switch back to the expression editor to continue editing.";
            //    pnlInfo.Visible = true;
            //    persistExpression = true;

            //}
            //else
            //{
            //    BaseNodePanel.RightClickMenu = menuConditions;
            //    RootPanel = BaseNodePanel.GetPanelForNode(Scope.GetScopeType(), Root, pnlTree);
            //    (RootPanel as MultiNodePanel)?.UpdateLabels(true);
            //}

            //pnlTree.ResumeDrawing();
        }

        MultiNode Root;
        BaseNodePanel RootPanel;

        private void button1_Click(object sender, EventArgs e)
        {
            //var cond = new ConditionPanel(pnlTree, menuConditions);
        }

        private void multipleConditionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Try to cast the sender to a ToolStripItem
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    var sourcePanel = owner.SourceControl as BaseNodePanel;
                    //if (sourcePanel != null) sourcePanel.MakeConditional();
                }
            }
        }

        

        private void menuConditions_Opening(object sender, CancelEventArgs e)
        {
            // Retrieve the ContextMenuStrip that owns this ToolStripItem
            ContextMenuStrip owner = sender as ContextMenuStrip;
            if (owner != null)
            {
                // Get the control that is displaying this context menu
                var sourcePanel = owner.SourceControl as BaseNodePanel ?? owner.SourceControl.Parent as BaseNodePanel;
                sourcePanel.Highlight = true;
            }
        }

        private void menuConditions_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            // Retrieve the ContextMenuStrip that owns this ToolStripItem
            ContextMenuStrip owner = sender as ContextMenuStrip;
            if (owner != null)
            {
                // Get the control that is displaying this context menu
                var sourcePanel = owner.SourceControl as BaseNodePanel ?? owner.SourceControl.Parent as BaseNodePanel;
                sourcePanel.Highlight = false;
            }
        }

        private void deleteCriteriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            if (menu != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menu.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    var sourcePanel = owner.SourceControl as BaseNodePanel ?? owner.SourceControl.Parent as BaseNodePanel;

                    sourcePanel.Delete();
                }
            }
        }

        private void addCriteriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;
            if (menu != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menu.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    var sourcePanel = owner.SourceControl as BaseNodePanel ?? owner.SourceControl.Parent as BaseNodePanel;

                    if (sourcePanel is MultiNodePanel)
                    {
                        var c = new CriteriaNodePanel(Scope.GetScopeType(), sourcePanel, new CriteriaNode());
                        (sourcePanel as MultiNodePanel).UpdateLabels();
                        c.Focus();
                    }
                }
            }
        }

        bool persistExpression;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
            {
                // Switched to visual mode:
                Expression = txtExpression.Text;
                ValidateRule();
            }
            else
            {
                // Switched to expression editor mode:
                if (persistExpression)
                {
                    pnlInfo.Visible = false;
                }
                else
                {
                    var code = RootPanel.Node.ToString();
                    if (code.StartsWith("(") && code.EndsWith(")")) code = code.Substring(1, code.Length - 2);
                    txtExpression.Text = code;
                }

                persistExpression = false;
            }
        }

        private void txtExpression_TextChanged(object sender, EventArgs e)
        {
            if (!btnOK.Enabled)
            {
                btnOK.Enabled = true;
                pnlInfo.Visible = false;
            }
        }

        private void txtExpression_Leave(object sender, EventArgs e)
        {
            btnOK.Enabled = ValidateRule();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtID.Text == GetStandardIdFromName(nameBefore))
            {
                txtID.Text = GetStandardIdFromName(txtName.Text);
                nameBefore = txtName.Text;
            }
            else btnOK.Enabled = ValidateRule();
        }

        private string nameBefore = "";

        private void txtID_TextChanged(object sender, EventArgs e)
        {
           btnOK.Enabled = ValidateRule();
        }
    }
}
