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
    public partial class NewModelDialog : Form
    {
        public NewModelDialog()
        {
            InitializeComponent();
        }

        private void NewModelDialog_Shown(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 2;
        }

        public int CompatibilityLevel
        {
            get
            {
                return comboBox1.SelectedIndex == 0 ? 1200
                    : comboBox1.SelectedIndex == 1 ? 1400
                    : comboBox1.SelectedIndex == 2 ? 1500
                    : 1560;
            }
        }
        public bool PbiDatasetModel => comboBox1.SelectedIndex == 3;
    }
}
