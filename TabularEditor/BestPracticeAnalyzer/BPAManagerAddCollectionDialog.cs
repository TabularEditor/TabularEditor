using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TabularEditor.BestPracticeAnalyzer
{
    public partial class BPAManagerAddCollectionDialog : Form
    {
        public BPAManagerAddCollectionDialog()
        {
            InitializeComponent();
        }

        static public bool Show(Analyzer analyzer)
        {
            var form = new BPAManagerAddCollectionDialog();

            if (form.ShowDialog() == DialogResult.Cancel) return false;

            if (form.rdbNewFile.Checked) return NewFile();
            if (form.rdbLocalFile.Checked) return LocalFile();
            if (form.rdbUrl.Checked) return Url();

            return true;
        }

        static public bool NewFile()
        {
            throw new NotImplementedException();
        }
        static public bool LocalFile()
        {
            throw new NotImplementedException();
        }
        static public bool Url()
        {
            throw new NotImplementedException();
        }
    }
}
