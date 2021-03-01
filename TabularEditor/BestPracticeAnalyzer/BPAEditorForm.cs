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
        public void PopulateCategories(IEnumerable<BestPracticeRule> collection)
        {
            var categories = collection
                .Where(r => !string.IsNullOrEmpty(r.Category))
                .Select(r => r.Category.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c);

            cmbCategory.Items.Clear();
            cmbCategory.Items.AddRange(categories.ToArray());
        }

        public bool EditRule(BestPracticeRule rule)
        {
            initializing = true;

            Expression = rule.Expression;
            Scope = rule.Scope;
            nameBefore = rule.Name;
            txtName.Text = rule.Name;
            txtID.Text = rule.ID;
            txtDescription.Text = rule.Description;
            numSeverity.Value = rule.Severity > 5 ? 5 : rule.Severity;
            cmbCompatibility.SelectedIndex = rule.CompatibilityLevel == 1400 ? 1 : (rule.CompatibilityLevel == 1470 ? 2 : 0);
            cmbCategory.Text = rule.Category?.Trim();

            initializing = false;
            btnOK.Enabled = ValidateRule();
            txtExpression.Focus();

            if(ShowDialog() == DialogResult.OK)
            {
                rule.Expression = Expression;
                rule.Scope = Scope;
                rule.Name = txtName.Text;
                rule.ID = txtID.Text;
                rule.Description = txtDescription.Text;
                rule.Severity = (int)numSeverity.Value;
                rule.CompatibilityLevel = cmbCompatibility.SelectedIndex == 0 ? 1200 : (cmbCompatibility.SelectedIndex == 1 ? 1400 : 1470);
                rule.Category = cmbCategory.Text;
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
            initializing = true;

            Expression = "";
            Scope = RuleScope.Model;
            nameBefore = name;
            txtName.Text = name;
            txtID.Text = GetStandardIdFromName(name);
            txtDescription.Text = "";
            numSeverity.Value = 1;
            cmbCompatibility.SelectedIndex = 0;

            initializing = false;
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
                    Enabled = true,
                    CompatibilityLevel = cmbCompatibility.SelectedIndex == 0 ? 1200 : (cmbCompatibility.SelectedIndex == 1 ? 1400 : 1470),
                    Category = cmbCategory.Text
            };
            }
            else
                return null;
        }

        public BPAEditorForm()
        {
            InitializeComponent();
            
            this.FormClosing += BPAEditorForm_FormClosing;

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

        private void BPAEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if((e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None) && this.DialogResult == DialogResult.OK)
            {
                if (!ValidateRule())
                {
                    var dr = MessageBox.Show("The rule expression contains errors. Are you sure you want to close the Rule Editor?", "Rule contains errors", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Cancel) e.Cancel = true;
                }
            }
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

        private bool ValidateRuleData(out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                errorMessage = "ID cannot be blank";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorMessage = "Name cannot be blank";
                return false;
            }

            return true;
        }
        
        private bool ValidateRuleInternal(out string errorMessage)
        {
            errorMessage = null;
            var result = ValidateRuleData(out errorMessage);
            if (!result) return result;

            try
            {
                // Attempt to parse the expression of the assigned rule:
                foreach (var t in Scope.Enumerate().Select(s => s.GetScopeType()))
                {
                    var expr = DynamicExpression.ParseLambda(t, typeof(bool), Expression);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Call this method to validate the dynamic linq expression and rebuild the
        /// visual builder tree. Returns true if the expression was successfully
        /// validated - false otherwise.
        /// 
        /// This method also displays an info-panel at the top of the screen, in case
        /// validation fails.
        /// </summary>
        /// <returns></returns>
        private bool ValidateRule()
        {
            if(!ValidateRuleInternal(out string errorMessage))
            {
                lblInfo.Text = "Error: " + errorMessage;
                pnlInfo.Visible = true;
                return false;
            }
            else
            {
                pnlInfo.Visible = false;
                return true;
            }
        }

        private void txtExpression_TextChanged(object sender, EventArgs e)
        {
            if (initializing) return;

            if (!btnOK.Enabled)
            {
                btnOK.Enabled = true;
                pnlInfo.Visible = false;
            }
        }

        private void txtExpression_Leave(object sender, EventArgs e)
        {
            ValidateRule();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (initializing) return;

            if (txtID.Text == GetStandardIdFromName(nameBefore))
            {
                txtID.Text = GetStandardIdFromName(txtName.Text);
                nameBefore = txtName.Text;
            }
            else btnOK.Enabled = ValidateRule();
        }

        private string nameBefore = "";
        private bool initializing;

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            if (initializing) return;

           btnOK.Enabled = ValidateRule();
        }
    }
}
