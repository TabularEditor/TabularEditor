using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using TabularEditor.PropertyGridUI;
using Microsoft.Data.ConnectionUI;

namespace TabularEditor.UI.Dialogs
{
    public partial class ConnectSqlForm: ICustomEditor
    {
        public ConnectSqlForm()
        {
        }

        public object Edit(object instance, string property, object value, out bool cancel)
        {
            var tabularDs = instance as ProviderDataSource;

            var dcd = new DataConnectionDialog();
            DataSource.AddStandardDataSources(dcd);
            dcd.SelectedDataSource = DataSource.SqlDataSource;
            dcd.SelectedDataProvider = DataProvider.SqlDataProvider;
            var res = DataConnectionDialog.Show(dcd);

            if(res == DialogResult.OK)
            {
                dcd.SelectedDataProvider.Name
            }
        }
    }
}
