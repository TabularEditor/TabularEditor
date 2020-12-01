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
    public partial class SaveCustomActionForm : Form
    {
        public Context Context
        {
            get
            {
                return chkListboxContexts.CheckedItems.Cast<string>()
                    .Select(v => { Context c; Enum.TryParse(v, out c); return c; })
                    .Combine();
            }
            set
            {
                for(var i = 0; i < chkListboxContexts.Items.Count; i++)
                {
                    Context c;
                    Enum.TryParse((string)chkListboxContexts.Items[i], out c);

                    chkListboxContexts.SetItemChecked(i, value.HasX(c));
                }
                ValidateOK();
            }
        }

        public SaveCustomActionForm()
        {
            InitializeComponent();

            linkLabel1.Links.Add(27, 29, Path.GetDirectoryName(ScriptEngine.CustomActionsJsonPath));
            linkLabel1.Links.Add(73, 20, "https://docs.tabulareditor.com/Custom-Actions.html");

            chkListboxContexts.Items.AddRange(
                Enum.GetValues(typeof(Context)).Cast<Context>()
                    .Where(v => v.Has1(Context.SingularObjects))
                    .Select(v => Enum.GetName(typeof(Context),v)).ToArray()
                );
            ValidateOK();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            ValidateOK();
        }

        private void ValidateOK()
        {
            btnOK.Enabled = !string.IsNullOrEmpty(txtName.Text)
                && chkListboxContexts.CheckedIndices.Count > 0;
        }

        private void chkListboxContexts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)(
                () => ValidateOK()));
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
