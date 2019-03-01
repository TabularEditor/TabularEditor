using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Dialogs
{
    public partial class DeployingForm : Form
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public DeployingForm()
        {
            InitializeComponent();
        }
        public CancellationToken CancelToken => _cancellationTokenSource.Token;

        public Action DeployAction;

        private void DeployingForm_Shown(object sender, EventArgs e)
        {
            DeployAction.BeginInvoke((ar) => {
                ThreadClose();
            }, null);
        }

        private void linkLabelCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelCancel.Enabled = false;
            linkLabelCancel.Text = "Cancelling ...";
            linkLabelCancel.Refresh();

            _cancellationTokenSource.Cancel();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
