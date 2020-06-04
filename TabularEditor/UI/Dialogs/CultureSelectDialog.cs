using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Dialogs
{
    public partial class CultureSelectDialog : Form
    {
        CultureInfo[] cultures;
        string[] cultureNames;

        public CultureInfo SelectedCulture;

        public CultureSelectDialog()
        {
            InitializeComponent();

            cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            cultureNames = cultures.Select(c => c.Name).ToArray();

            listView1.VirtualListSize = cultures.Length;
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var c = cultures[e.ItemIndex];
            e.Item = new ListViewItem(c.Name + " - " + c.DisplayName);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedIndices.Count == 1) {
                btnOK.Enabled = true;
                SelectedCulture = cultures[listView1.SelectedIndices[0]];
            } else
            {
                btnOK.Enabled = false;
                SelectedCulture = null;
            }
        }

        private void listView1_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            e.Index = Array.BinarySearch<string>(cultureNames, e.Text,
                Comparer<string>.Create((x, y) => x.StartsWith(y) ? 0 : x.CompareTo(y))
                );
        }
    }
}
