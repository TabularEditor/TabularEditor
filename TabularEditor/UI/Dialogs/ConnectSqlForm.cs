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
using TabularEditor.UIServices;

namespace TabularEditor.UI.Dialogs
{
    public partial class ConnectSqlForm : ICustomEditor
    {
        public ConnectSqlForm()
        {
        }

        public void New()
        {
            var dcd = new DataConnectionDialog();
            DataSource.AddStandardDataSources(dcd);
            dcd.SelectedDataSource = DataSource.SqlDataSource;
            dcd.SelectedDataProvider = DataProvider.SqlDataProvider;
            var res = DataConnectionDialog.Show(dcd);

            if (res == DialogResult.OK)
            {
                var ds = TypedDataSource.GetFromConnectionUi(dcd);
            }

        }

        public object Edit(object instance, string property, object value, out bool cancel)
        {
            if (!(instance is TOMWrapper.ProviderDataSource)) throw new NotSupportedException("This data source is not supported by Tabular Editor.");
            cancel = true;
            
            var tabularDs = instance as TOMWrapper.ProviderDataSource;
            if (tabularDs is null) return value;

            var ds = TypedDataSource.GetFromTabularDs(tabularDs);
            var dcd = new DataConnectionDialog();
            DataSource.AddStandardDataSources(dcd);
            if (ds.ProviderType != ProviderType.Unknown)
            {
                dcd.SelectedDataSource = ds.DataSource;
                dcd.SelectedDataProvider = ds.DataProvider;
                dcd.ConnectionString = ds.ProviderString;
            } else
            {
                var mbResult = MessageBox.Show("The provider and/or connection string used by this data source, is not supported by Tabular Editor.", "Unknown provider", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (mbResult == DialogResult.Cancel) return value;
            }
            var res = DataConnectionDialog.Show(dcd);

            if (res == DialogResult.OK)
            {
                cancel = false;
                return dcd.ApplyToTabularDs(tabularDs);
            }

            return value;
        }
    }
}
