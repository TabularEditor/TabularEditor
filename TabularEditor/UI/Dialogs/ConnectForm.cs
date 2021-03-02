using Microsoft.AnalysisServices.Tabular;
using System;
using System.Windows.Forms;
using TabularEditor.UIServices;

namespace TabularEditor.UI.Dialogs
{


    public partial class ConnectForm : Form
    {
        public ConnectForm()
        {
            InitializeComponent();
        }

        public static string ConnectionString { get; private set; }
        public static Server Server { get; private set; }
        public static LocalInstance LocalInstance { get; private set; }

        public static DialogResult Show(string caption = "Connect to Tabular Server")
        {
            var form = new ConnectForm() { Text = caption };
            var res = form.ShowDialog();
            return res;
        }

        private void ConnectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel) return;

            using (new Hourglass())
            {
                ConnectionString = connectPage.GetConnectionString();
                Server = connectPage.GetServer();
                LocalInstance = connectPage.LocalInstance;
                if (Server == null) e.Cancel = true;
            }
        }

        private void connectPage_Validation(object sender, ValidationEventArgs e)
        {
            btnOK.Enabled = e.IsValid;
        }
    }
}
