using Aga.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor
{
    public partial class FormBatchRename : Form
    {
        public FormBatchRename()
        {
            InitializeComponent();
            RenamePattern = "Copy of [name]";
            txtRename.Text = RenamePattern;
            IncludeTranslations = true;
        }

        private void txtRename_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = txtRename.Text.Length > 0;
            RenamePattern = txtRename.Text;
            lblExample.Text = "My Measure => " + Rename("My Measure");
        }

        public bool IncludeTranslations { get; set; }
        public string RenamePattern { get; set; }

        public string Rename(string orgName)
        {
            var i = RenamePattern.IndexOf("[name]", StringComparison.InvariantCultureIgnoreCase);
            return (i == -1 ? RenamePattern : RenamePattern.Splice(i, 6, orgName));

        }

        private void chkTranslations_CheckedChanged(object sender, EventArgs e)
        {
            IncludeTranslations = chkTranslations.Checked;
        }
    }
}
