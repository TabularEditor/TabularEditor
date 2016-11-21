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
    public partial class DeployingForm : Form
    {
        public DeployingForm()
        {
            InitializeComponent();
        }

        public Action DeployAction;

        private void DeployingForm_Shown(object sender, EventArgs e)
        {
            var r = DeployAction.BeginInvoke((ar) => {
                ThreadClose();
            }, null);
        }

        private bool _closed = false;

        public void ThreadClose()
        {
            if (!_closed)
            {
                _closed = true;
                Invoke(new InvokeDelegate(Close));
            }
        }

        private delegate void InvokeDelegate();
    }
}
