using NodeEditor;
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
    public partial class VisualizeRelationshipsForm : Form
    {
        public VisualizeRelationshipsForm()
        {
            InitializeComponent();
        }

        public void ShowGraph(Model model)
        {
            foreach(var table in model.Tables.OfType<Table>())
            {
                var node = nodesControl1.Graph.AddTable(table);
            }

            foreach (var rel in model.Relationships.OfType<SingleColumnRelationship>())
            {
                nodesControl1.Graph.AddRelationship(rel);
            }

            
            Show();
        }
    }
}
