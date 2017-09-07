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
        protected Diagram Graph;

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
                Graph = new Diagram(_model);
            }
        }

        public VisualizeRelationshipsForm()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });
        }

        HashSet<Relationship> DrawnRelationships = new HashSet<Relationship>();

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graph.Draw(e.Graphics);
        }

        bool ProbeDrag = false;
        bool IsDragging = false;
        Point DragStart;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ProbeDrag = true;
            DragStart = e.Location;

            var multiSelect = ModifierKeys.HasFlag(Keys.Control) || ModifierKeys.HasFlag(Keys.Shift);
            var hit = Graph.HitTest(e.Location);
            if(hit == null && !multiSelect)
            {
                Graph.ClearSelected();
            } else if(hit != null)
            {
                hit.Select(multiSelect);
            }

            panel1.Invalidate();
        }

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
                Tables[0].X = e.X;
                Tables[0].Y = e.Y;
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
