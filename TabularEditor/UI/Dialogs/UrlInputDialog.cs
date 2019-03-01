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
    public partial class UrlInputDialog : Form
    {
        public UrlInputDialog()
        {
            InitializeComponent();
        }

        static public bool Show(out string url)
        {
            var form = new UrlInputDialog();
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                url = form.txtUrl.Text;
                return true;
            }
            else
            {
                url = null;
                return false;
            }
        }

        private void PasswordPromptForm_Shown(object sender, EventArgs e)
        {
            txtUrl.Focus();
        }

        private void UrlInputDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult == DialogResult.OK)
            {
                try
                {
                    using (var hourglass = new Hourglass())
                    {
                        BestPracticeAnalyzer.BestPracticeCollection.GetUrl(txtUrl.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading Rule File from URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }
    }
}
