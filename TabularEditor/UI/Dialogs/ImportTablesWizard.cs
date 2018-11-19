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
            var dialog = new ImportTablesWizard();
            dialog.importTablesPage1.Init(TypedDataSource.GetFromTabularDs(model.DataSources[0] as ProviderDataSource));

            var res = dialog.ShowDialog();

            return res;
        }
    }
}
