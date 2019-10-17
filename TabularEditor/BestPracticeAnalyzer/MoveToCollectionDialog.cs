using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.BestPracticeAnalyzer
{
    public partial class MoveToCollectionDialog : Form
    {
        public MoveToCollectionDialog()
        {
            InitializeComponent();
        }

        public static bool Show(BestPracticeCollection source, bool plural, Analyzer analyzer, out BestPracticeCollection destinationCollection, out bool clone)
        {
            var form = new MoveToCollectionDialog();
            form.lblHeader.Text = "Which collection do you want to move the selected rule" + (plural ? "s" : "") + " to?";
            form.chkCopy.Text = "Clone rule" + (plural ? "s" : "") + " to collection";
            form.chkCopy.Enabled = source.AllowEdit;
            if (!source.AllowEdit) form.chkCopy.Checked = true; // Only allow copying when sourcing rules from a read-only collection
            form.comboBox1.DisplayMember = "Name";
            form.comboBox1.Items.AddRange(analyzer.Collections.Where(rc => rc.AllowEdit && rc != source).ToArray());
            if (form.comboBox1.Items.Count == 0) form.btnOK.Enabled = true;
            else form.comboBox1.SelectedIndex = 0;

            destinationCollection = null;
            clone = false;

            if(form.ShowDialog() == DialogResult.OK)
            {
                clone = form.chkCopy.Checked;
                destinationCollection = form.comboBox1.SelectedItem as BestPracticeCollection;
                return true;
            }
            return false;
        }
    }
}
