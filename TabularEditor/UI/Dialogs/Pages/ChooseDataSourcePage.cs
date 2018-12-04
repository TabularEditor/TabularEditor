using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class ChooseDataSourcePage : UserControl
    {
        public ChooseDataSourcePage()
        {
            InitializeComponent();
        }

        public ImportMode Mode {
            get
            {
                if (rdbExisting.Checked) return ImportMode.UseExistingDs;
                else if (rdbNew.Checked) return ImportMode.UseNewDs;
                else if (rdbTemporary.Checked) return ImportMode.UseTempDs;
                else return ImportMode.UseClipboard;
            }
        }

        public ProviderDataSource CurrentDataSource => listView1.SelectedItems.Count == 1 ? model.DataSources[listView1.SelectedItems[0].Text] as ProviderDataSource : null;
        Model model;
        public void Init(Model model)
        {
            listView1.View = View.List;
            this.model = model;
            listView1.SmallImageList = UIController.Current.Elements.FormMain.tabularTreeImages;

            if(model.DataSources.Count(ds => ds.Type == DataSourceType.Provider) == 0)
            {
                rdbExisting.Enabled = false;
                rdbNew.Checked = true;
            }
            else
            {
                foreach(var ds in model.DataSources.OfType<ProviderDataSource>())
                {
                    listView1.Items.Add(ds.Name, 36);
                }
                rdbExisting.Checked = true;
            }
        }

        private void rdbExisting_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbExisting.Checked)
                listView1.HideSelection = false;
            else
                listView1.HideSelection = true;

            OnValidated(new EventArgs());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1)
            {
                rdbExisting.Checked = true;
            }

            OnValidated(new EventArgs());
        }
    }

    public enum ImportMode
    {
        UseExistingDs,
        UseNewDs,
        UseTempDs,
        UseClipboard
    }
}
