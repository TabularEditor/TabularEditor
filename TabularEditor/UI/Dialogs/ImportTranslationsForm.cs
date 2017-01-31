using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Dialogs
{
    public partial class ImportTranslationsForm : Form
    {
        public ImportTranslationsForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnImport.Enabled = File.Exists(txtJsonFile.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtJsonFile.Text = openFileDialog1.FileName;
            }
        }

        public bool OverwriteExisting
        {
            get { return chkOverwriteExisting.Checked; }
        }
        public bool IgnoreInvalid
        {
            get { return chkIgnoreInvalid.Checked; }
        }

        public string JsonFile
        {
            get { return txtJsonFile.Text; }
        }
    }
}
