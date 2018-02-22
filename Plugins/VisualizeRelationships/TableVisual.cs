using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TabularEditor.TOMWrapper;

namespace VisualizeRelationships
{
    public class Diagram
    {
        public Diagram(Model model)
        {
            Tables.Clear();
            TablesZOrder.Clear();
            Relationships.Clear();

            var i = 0;
            foreach (var t in model.Tables)
            {
                var tv = new TableVisual(this, t, i);
                Tables.Add(t, tv);
                TablesZOrder.Add(tv);
                i++;
            }

        }
        public Dictionary<Table, TableVisual> Tables = new Dictionary<Table, TableVisual>();
        public List<TableVisual> TablesZOrder = new List<TableVisual>();
        public List<RelationshipVisual> Relationships = new List<RelationshipVisual>();

        private HashSet<Relationship> DrawnRelationships = new HashSet<Relationship>();

        public void Draw(Graphics gfx)
        {
            DrawnRelationships.Clear();
            foreach(var t in TablesZOrder)
            {
                t.DrawRelationships(gfx);
            }
            for (int i = TablesZOrder.Count - 1; i >= 0; i--)
            {
                TablesZOrder[i].Draw(gfx);
            }
        }

        public DiagramObject HitTest(Point pt)
        {

        }

        public void ClearSelectedTables()
        {
            foreach (var t in TablesZOrder) t.Selected = false;
        }

        public void ClearSelectedRelationships()
        {
            foreach (var r in Relationships) r.Selected = false;
        }

        public void ClearSelected()
        {
            ClearSelectedTables();
            ClearSelectedRelationships();
        }
    }

    public abstract class DiagramObject
    {
        public DiagramObject(Diagram diagram)
        {
            Diagram = diagram;
        }
        protected readonly Diagram Diagram;
        public bool Selected { get; set; }
        public abstract void Select(bool multiSelect);
    }

    public class RelationshipVisual: DiagramObject
    {
        TableVisual T1 { get { return Diagram.Tables[Relationship.FromTable]; } }
        TableVisual T2 { get { return Diagram.Tables[Relationship.ToTable]; } }

        public RelationshipVisual(Diagram diagram, Relationship relationship) : base(diagram) {
            Relationship = relationship;
        }
        public Relationship Relationship;
        public override void Select(bool multiSelect)
        {

            throw new NotImplementedException();
        }
    }

    public class TableVisual: DiagramObject
    {
        public TableVisual(Diagram diagram, Table table, int increment = 0): base(diagram)
        {
            Table = table;
            X = increment * 20;
            Y = increment * 10;
            Width = 100;
            Height = 100;
        }

        public override void Select(bool multiSelect)
        {
            throw new NotImplementedException();
        }

        public Table Table;
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public void Draw(Graphics gfx)
        {

            var p = new Pen(Brushes.Black, Selected ? 2 : 1);

            gfx.FillRectangle(Selected ? Brushes.LightGray : Brushes.White, X, Y, Width, Height);
            gfx.DrawRectangle(p, X, Y, Width, Height);
            gfx.DrawString(Table.Name, SystemFonts.DefaultFont, Brushes.Black, new RectangleF(X + 2, Y + 2, Width - 4, SystemFonts.DefaultFont.SizeInPoints + 4));
        }

        public void DrawRelationships(Graphics gfx)
        {
            var rels = Table.UsedInRelationships.ToList();
            
        }

        public bool HitTest(Point pt)
        {
            return pt.X >= X && pt.X <= X + Width && pt.Y >= Y && pt.Y <= Y + Height;
        }
    }

}