using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UIServices;

namespace TabularEditor.UI.Dialogs
{
    public partial class ImportTablesWizard : Form
    {
        public ImportTablesWizard()
        {
            InitializeComponent();
        }

        public static DialogResult ShowWizard(Model model)
        {
            return ShowWizard(model, model.DataSources[0] as ProviderDataSource);
        }

        public static DialogResult ShowWizard(Model model, ProviderDataSource source)
        {
            var dialog = new ImportTablesWizard();
            dialog.importTablesPage1.Init(TypedDataSource.GetFromTabularDs(source));

            var res = dialog.ShowDialog();

            return res;
        }

        private void ImportTablesWizard_Shown(object sender, EventArgs e)
        {
            importTablesPage1.ExpandFirstNode();
        }
    }
}
