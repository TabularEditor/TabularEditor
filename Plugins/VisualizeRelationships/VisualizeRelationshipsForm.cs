using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace VisualizeRelationships
{
    public partial class VisualizeRelationshipsForm : Form
    {
        private Diagram _currentDiagram;
        protected Diagram CurrentDiagram
        {
            get { return _currentDiagram; }
            set { _currentDiagram = value; panel1.Invalidate(); ValidateButtons(); }
        }
        public List<Diagram> AllDiagrams { get; private set; }

        private Model _model;
        public Model Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model == value) return;
                _model = value;

                var diagramsJson = _model.GetAnnotation("TabularEditor_Diagrams");
                if (string.IsNullOrEmpty(diagramsJson))
                {
                    AllDiagrams = new List<Diagram>();
                }
                else
                {
                    AllDiagrams = Diagram.Deserialize(diagramsJson, _model);
                }

                listView1.Items.Clear();
                listView1.Items.AddRange(AllDiagrams.Select(d => new ListViewItem(d.Name) { Tag = d }).ToArray());
                if (listView1.Items.Count > 0) listView1.Items[0].Selected = true;
                else CurrentDiagram = null;
            }
        }

        public VisualizeRelationshipsForm()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });

            listView1_Resize(this, null);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (CurrentDiagram == null) return;
            CurrentDiagram.Draw(e.Graphics);
        }

        bool ProbeDrag = false;
        bool IsDragging = false;
        Point DragStart;
        int dX;
        int dY;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentDiagram == null) return;

            ProbeDrag = true;
            DragStart = e.Location;

            var multiSelect = ModifierKeys.HasFlag(Keys.Control) || ModifierKeys.HasFlag(Keys.Shift);
            var hit = CurrentDiagram.HitTest(e.Location);
            if(hit == null && !multiSelect)
            {
                CurrentDiagram.ClearSelected();
                DragObjects = null;
                dX = e.X;
                dY = e.Y;
                orgOffsetX = CurrentDiagram.OffsetX;
                orgOffsetY = CurrentDiagram.OffsetY;
            }
            else if(hit != null)
            {
                hit.Select(multiSelect);
                var p = CurrentDiagram.OZ(hit.X, hit.Y);
                dX = e.X - p.X;
                dY = e.Y - p.Y;
                DragObjects = CurrentDiagram.ZOrder.Where(obj => obj.Selected).ToList();
            }

            panel1.Invalidate();

            ValidateButtons();
        }

        private void ValidateButtons()
        {
            // Diagram list toolstrip:
            toolStripButton5.Enabled = CurrentDiagram != null;

            // Diagram canvas toolstrip:
            if (CurrentDiagram == null)
            {
                toolStrip1.Enabled = false;
                return;
            }
            else
                toolStrip1.Enabled = true;

            var tablesSelected = DragObjects != null && DragObjects.Count > 0;

            btnHideTable.Enabled = tablesSelected;
            btnAddRelated.Enabled = tablesSelected;
        }

        int orgOffsetX;
        int orgOffsetY;

        List<TableVisual> DragObjects;

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if(ProbeDrag && !IsDragging)
            {
                if(DragStart.Dist2(e.Location) >= 9)
                {
                    // Begin drag
                    IsDragging = true;
                }
            }

            if(IsDragging)
            {
                if (DragObjects == null)
                {
                    CurrentDiagram.OffsetX = orgOffsetX + (int)((dX - e.X) / CurrentDiagram.Zoom);
                    CurrentDiagram.OffsetY = orgOffsetY + (int)((dY - e.Y) / CurrentDiagram.Zoom);
                }
                else
                {
                    foreach (var obj in DragObjects)
                    {
                        obj.X = (int)((e.X - dX) / CurrentDiagram.Zoom) + CurrentDiagram.OffsetX;
                        obj.Y = (int)((e.Y - dY) / CurrentDiagram.Zoom) + CurrentDiagram.OffsetY;
                    }
                }
                panel1.Invalidate();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if(IsDragging)
            {
                // End drag
            }

            ProbeDrag = false;
            IsDragging = false;
        }

        private void VisualizeRelationshipsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var json = Diagram.Serialize(AllDiagrams);
                Model.SetAnnotation("TabularEditor_Diagrams", json);

                e.Cancel = true;
                Hide();
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            CurrentDiagram.Zoom = CurrentDiagram.Zoom * 1.1F;
            panel1.Invalidate();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            CurrentDiagram.Zoom = CurrentDiagram.Zoom / 1.1F;
            panel1.Invalidate();
        }

        private void btnZoom100_Click(object sender, EventArgs e)
        {
            CurrentDiagram.Zoom = 1.0F;
            panel1.Invalidate();
        }

        private void btnHideTable_Click(object sender, EventArgs e)
        {
            if (DragObjects == null) return;
            foreach(var obj in DragObjects)
            {
                CurrentDiagram.Hide(obj);
            }
            DragObjects = null;

            panel1.Invalidate();
        }

        private void btnAddRelated_Click(object sender, EventArgs e)
        {
            if (DragObjects == null || DragObjects.Count == 0) return;
            CurrentDiagram.AddRelated(DragObjects[0] as TableVisual);

            panel1.Invalidate();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addTableDlg = new AddTablesDialog();
            if(addTableDlg.Show(CurrentDiagram) == DialogResult.OK)
            {
                CurrentDiagram.EnsureVisible(addTableDlg.SelectedTables, true);
                panel1.Invalidate();
            }
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            listView1.Columns[0].Width = listView1.Width - 5 - SystemInformation.VerticalScrollBarWidth;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 0) { CurrentDiagram = null; return; }
            CurrentDiagram = listView1.SelectedItems[0].Tag as Diagram;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var d = new Diagram(Model) { Name = "New Diagram" };
            AddAndSelectDiagram(d);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (CurrentDiagram == null) return;
            var d = CurrentDiagram.Clone();
            AddAndSelectDiagram(d);
        }

        private void AddAndSelectDiagram(Diagram d)
        {
            AllDiagrams.Add(d);

            listView1.SelectedItems.Clear();
            listView1.Items.Add(new ListViewItem(d.Name) { Tag = d, Selected = true });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CurrentDiagram != null)
            {
                CurrentDiagram.ForceStep();
                panel1.Invalidate();
            }
        }

        private void btnForceDirected_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void btnForceDirected_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = btnForceDirected.Checked;
        }
    }

    public static class PointHelper
    {
        // Returns the squared distance between two points:
        public static int Dist2(this Point P1, Point P2)
        {
            return (P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P1.Y - P2.Y);
        }
    }
}
