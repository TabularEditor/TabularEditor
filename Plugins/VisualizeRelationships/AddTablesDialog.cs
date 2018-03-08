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

namespace VisualizeRelationships
{
    public partial class AddTablesDialog : Form
    {
        public AddTablesDialog()
        {
            InitializeComponent();

            lvAllTables.Columns[0].Width = lvAllTables.Width - 4 - SystemInformation.VerticalScrollBarWidth;
            lvSelectedTables.Columns[0].Width = lvSelectedTables.Width - 4 - SystemInformation.VerticalScrollBarWidth;
        }

        Model model;

        public DialogResult Show(Diagram diagram)
        {
            model = diagram.Model;
            foreach (var t in model.Tables.OrderBy(t => t.Name))
            {
                if(diagram.Tables.ContainsKey(t))
                {
                    lvSelectedTables.Items.Add(t.Name);
                }
                else
                {
                    lvAllTables.Items.Add(t.Name);
                }
            }

            return ShowDialog();
        }

        public IEnumerable<Table> SelectedTables
        {
            get
            {
                return lvSelectedTables.Items.OfType<ListViewItem>().Select(t => model.Tables[t.Text]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in lvAllTables.Items.OfType<ListViewItem>().ToList())
            {
                lvAllTables.Items.RemoveAt(item.Index);
                lvSelectedTables.Items.Add(item);
            }

            lvAllTables.Sort();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvSelectedTables.Items.OfType<ListViewItem>().ToList())
            {
                lvSelectedTables.Items.RemoveAt(item.Index);
                lvAllTables.Items.Add(item);
            }

            lvSelectedTables.Sort();
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            if (lvAllTables.SelectedItems.Count == 0) return;
            foreach (ListViewItem item in lvAllTables.SelectedItems.OfType<ListViewItem>().ToList())
            {
                lvAllTables.Items.RemoveAt(item.Index);
                lvSelectedTables.Items.Add(item);
                item.Selected = true;
            }

            lvSelectedTables.Sort();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lvSelectedTables.SelectedItems.Count == 0) return;
            foreach (ListViewItem item in lvSelectedTables.SelectedItems.OfType<ListViewItem>().ToList())
            {
                lvSelectedTables.Items.RemoveAt(item.Index);
                lvAllTables.Items.Add(item);
                item.Selected = true;
            }

            lvAllTables.Sort();
        }

        private void lvAllTables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hit = lvAllTables.HitTest(e.Location);
            if(hit.Item != null)
            {
                lvAllTables.Items.Remove(hit.Item);
                lvSelectedTables.Items.Add(hit.Item);

                lvSelectedTables.Sort();
            }
        }

        private void lvSelectedTables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hit = lvSelectedTables.HitTest(e.Location);
            if (hit.Item != null)
            {
                lvSelectedTables.Items.Remove(hit.Item);
                lvAllTables.Items.Add(hit.Item);

                lvAllTables.Sort();
            }
        }
    }
}
