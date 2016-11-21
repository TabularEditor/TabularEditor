using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Dialogs
{
    public partial class ReplaceForm : Form
    {
        private static ReplaceForm _singleton = null;
        public static ReplaceForm Singleton
        {
            get
            {
                if (_singleton == null) _singleton = new ReplaceForm();
                return _singleton;
            }
        }
        public string Title { get { return this.Text; } set { this.Text = Title; } }

        public ReplaceForm()
        {
            InitializeComponent();
        }

        public string Pattern
        {
            get { return txtPattern.Text; }
        }

        public string ReplaceWith
        {
            get { return txtReplaceWith.Text; }
        }

        public bool MatchCase
        {
            get { return chkMatchCase.Checked; }
        }

        public bool WholeWord
        {
            get { return chkWholeWord.Checked; }
        }

        public bool RegEx
        {
            get { return chkRegEx.Checked; }
        }

        public bool IncludeTranslations
        {
            get
            {
                return chkTranslations.Checked;
            }
        }

        private void txtPattern_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !string.IsNullOrEmpty(txtPattern.Text);
        }

        private void ReplaceForm_Shown(object sender, EventArgs e)
        {
            txtPattern.Focus();
        }
    }
}
