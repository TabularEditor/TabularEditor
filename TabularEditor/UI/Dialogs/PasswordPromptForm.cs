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
    public partial class PasswordPromptForm : Form
    {
        public PasswordPromptForm()
        {
            InitializeComponent();
        }

        static public DialogResult Show(string dataSourceName, string username, out string password)
        {
            var form = new PasswordPromptForm();
            form.lblText.Text = form.lblText.Text.Replace("{0}", dataSourceName);
            form.txtUsername.Text = username;
            var result = form.ShowDialog();

            if (result == DialogResult.OK)
                password = form.txtPassword.Text;
            else
                password = null;

            return result;
        }

        private void PasswordPromptForm_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }
    }
}
