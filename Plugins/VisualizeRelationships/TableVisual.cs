using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TabularEditor.TOMWrapper;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;

namespace VisualizeRelationships
{
    public class Diagram
    {
        public static List<Diagram> Deserialize(string json, Model model)
        {
            var result = JsonConvert.DeserializeObject<List<Diagram>>(json);
            foreach (var d in result)
            {
                d.Model = model;
                d.Tables = d.ZOrder.ToDictionary(tv => tv.Table, tv => tv);
            }
            return result;
        }

        public void ForceStep()
        {
            //lock (locker)
            //{

            //    foreach (var t1 in ZOrder)
            //    {
            //        t1.dX = 0;
            //        t1.dY = 0;
            //    }

            //    float f;
            //    for (int i = 0; i < ZOrder.Count; i++)
            //    {
            //        var t1 = ZOrder[i];
            //        var l = t1.RelationshipsFrom
            //            .Select(rel => { TableVisual tv; Tables.TryGetValue(rel.ToTable, out tv); return tv; })
            //            .Where(tv => tv != null).ToList();
            //        foreach (var t2 in l)
            //        {
            //            var dX2 = (t1.X - t2.X) * (t1.X - t2.X);

            //            var dY2 = (t1.Y - t2.Y) * (t1.Y - t2.Y);
            //            var r2 = dX2 + dY2;
            //            var r = (float)Math.Sqrt(r2);

            //            f = -(r - 50.0F * l.Count / 2) / 10.0F;

            //            // 50  -->  0.5
            //            // 100 -->  0
            //            // 150 --> -0.5
            //            // 200 --> 

            //            t1.dX += (int)(f * (t1.X - t2.X) / r);
            //            t1.dY += (int)(f * (t1.Y - t2.Y) / r);

            //            t2.dX -= (int)(f * (t1.X - t2.X) / r);
            //            t2.dY -= (int)(f * (t1.Y - t2.Y) / r);

            //        }

            //        for (int j = i + 1; j < ZOrder.Count; j++)
            //        {
            //            var t2 = ZOrder[j];

            //            var dX2 = (t1.X - t2.X) * (t1.X - t2.X);

            //            var dY2 = (t1.Y - t2.Y) * (t1.Y - t2.Y);
            //            var r2 = dX2 + dY2;
            //            var r = (float)Math.Sqrt(r2);
                        
            //                // Repellant force:
            //                f = 800.0F / Math.Max(1.0F, r);

            //            t1.dX += (int)(f * (t1.X - t2.X) / r);
            //            t1.dY += (int)(f * (t1.Y - t2.Y) / r);

            //            t2.dX -= (int)(f * (t1.X - t2.X) / r);
            //            t2.dY -= (int)(f * (t1.Y - t2.Y) / r);
            //        }
            //    }

            //    foreach (var t1 in ZOrder.Where(tv => !tv.Selected))
            //    {
            //        t1.X += t1.dX;
            //        t1.Y += t1.dY;
            //    }

            //}
        }

        public void Remove(Table table)
        {
            TableVisual tv;
            if(Tables.TryGetValue(table, out tv))
            {
                Tables.Remove(table);
                ZOrder.Remove(tv);
            }
        }

        public void Remove(Relationship rel)
        {
            foreach(var tv in ZOrder)
            {
                tv.RelationshipsFrom.RemoveAll(r => r == rel);
            }
        }

        public static string Serialize(IList<Diagram> diagrams)
        {
            return JsonConvert.SerializeObject(diagrams);
        }

        public Diagram Clone()
        {
            var res = new Diagram(Model) { OffsetX = this.OffsetX, OffsetY = this.OffsetY, Zoom = this.Zoom }; 
            res.Name = Name + " copy";
            res.ZOrder = new List<TableVisual>(ZOrder.Select(obj => obj.CloneTo(res)));
            res.Tables = res.ZOrder.OfType<TableVisual>().ToDictionary(obj => obj.Table, obj => obj);

            return res;
        }

        public Point OZ(float x, float y)
        {
            return new Point((int)((x - OffsetX) * Zoom), (int)((y - OffsetY) * Zoom));
        }
        public Rectangle OZR(float x, float y, int width, int height)
        {
            return new Rectangle((int)((x - OffsetX) * Zoom), (int)((y - OffsetY) * Zoom), (int)(width * Zoom), (int)(height * Zoom));
        }
        public RectangleF OZRF(float x, float y, int width, int height)
        {
            return new RectangleF((int)((x - OffsetX) * Zoom), (int)((y - OffsetY) * Zoom), (int)(width * Zoom), (int)(height * Zoom));
        }



        public void Hide(TableVisual obj)
        {
            ZOrder.Remove(obj);
            if (obj is TableVisual) Tables.Remove((obj as TableVisual).Table);
        }

        public void EnsureVisible(IEnumerable<Table> tables, bool hideOthers)
        {
            var incr = 10;
            foreach(var t in tables)
            {
                if (!Tables.ContainsKey(t))
                {
                    Add(t, incr);
                    incr += 12;
                }
            }
            if(hideOthers)
            {
                foreach(var t in Tables.Keys.Except(tables).ToList())
                {
                    Hide(Tables[t]);
                }
            }
        }

        public void AddRelated(TableVisual obj)
        {
            var tablesToAdd = new List<Table>();

            TableVisual tv;
            foreach(var r in obj.Table.UsedInRelationships)
            {
                var tgt = r.FromTable == obj.Table ? r.ToTable : r.FromTable;
                if(Tables.TryGetValue(tgt, out tv))
                {
                    tv.Selected = true;
                    ZOrder.Remove(tv);
                    ZOrder.Insert(0, tv);
                }
                else if (!tablesToAdd.Contains(tgt)) tablesToAdd.Add(tgt);
            }
            
            for(int i = 0; i < tablesToAdd.Count; i++)
            {
                tv = new TableVisual(this, tablesToAdd[i]);

                var angle = Math.PI * 2 * i / tablesToAdd.Count;
                var cos = Math.Cos(angle) * (obj.W) * 1.8;
                var sin = Math.Sin(angle) * (obj.H) * 1.8;

                tv.X = obj.X + (obj.W / 2) + (int)cos - (tv.W / 2);
                tv.Y = obj.Y + (obj.H / 2) + (int)sin - (tv.H / 2);
                tv.Selected = true;
                Tables.Add(tablesToAdd[i], tv);
                ZOrder.Insert(0, tv);
            }
            obj.Selected = false;
        }

        public void Add(Table table, int incr = 0)
        {
            var tv = new TableVisual(this, table);
            tv.X = OffsetX + incr;
            tv.Y = OffsetY + incr;

            Tables.Add(table, tv);
            ZOrder.Add(tv);
        }

        public void AddRange(IEnumerable<Table> tables)
        {
            var i = 0;
            foreach (var t in tables)
            {
                var tv = new TableVisual(this, t, i);
                Tables.Add(t, tv);
                ZOrder.Add(tv);
                i++;
            }
        }

        private Model _model;

        [JsonIgnore]
        public Model Model
        {
            get { return _model; }
            set {
                _model = value;
                foreach (var tv in ZOrder.ToList())
                {
                    tv.Diagram = this;
                    if (_model.Tables.Contains(tv.T))
                        tv.Table = _model.Tables[tv.T];
                    else
                        ZOrder.Remove(tv);
                }
            }
        }

        public Diagram(Model model)
        {
            Tables.Clear();
            ZOrder.Clear();
            //Relationships.Clear();
            Model = model;
        }

        [JsonIgnore]
        public Dictionary<Table, TableVisual> Tables = new Dictionary<Table, TableVisual>();

        #region Serializable Members
        public List<TableVisual> ZOrder { get; set; } = new List<TableVisual>();

        public string Name { get; set; }

        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public float Zoom { get; set; } = 1.0F;

        #endregion

        object locker = new object();

        public void Draw(Graphics gfx)
        {
            lock (locker)
            {
                for (int i = ZOrder.Count - 1; i >= 0; i--)
                {
                    (ZOrder[i] as TableVisual).DrawRelationships(gfx);
                }
                for (int i = ZOrder.Count - 1; i >= 0; i--)
                {
                    ZOrder[i].Draw(gfx);
                }
            }
        }

        public TableVisual HitTest(Point pt)
        {
            foreach(var t in ZOrder)
            {
                if (t.HitTest(pt)) return t;
            }
            return null;
        }

        public void ClearSelectedTables()
        {
            foreach (var t in ZOrder) t.Selected = false;
        }

        public void ClearSelectedRelationships()
        {
            //foreach (var r in Relationships) r.Selected = false;
        }

        public void ClearSelected()
        {
            ClearSelectedTables();
            ClearSelectedRelationships();
        }
    }

    public class TableVisual
    {
        [JsonIgnore]
        public float dX;
        [JsonIgnore]
        public float dY;

        public virtual void Select(bool multiSelect)
        {
            foreach (var obj in Diagram.ZOrder)
            {
                if (obj.Selected)
                {
                    obj.Selected = false;
                }

            }
            Selected = true;
            Diagram.ZOrder.Remove(this);
            Diagram.ZOrder.Insert(0, this);
        }

        [JsonIgnore]
        public bool Selected { get; set; }

        private Table _table;

        [JsonIgnore]
        public Table Table
        {
            get
            {
                return _table;
            }
            internal set
            {
                _table = value;
                T = _table.Name;
                RelationshipsFrom = _table.UsedInRelationships.Where(r => r.FromTable == _table).ToList();
            }
        }


        public string T { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        [JsonIgnore]
        internal Diagram Diagram;
        public TableVisual CloneTo(Diagram diagram)
        {
            return new TableVisual(diagram, Table)
            {
                X = this.X,
                Y = this.Y,
                W = this.W,
                H = this.H,
                Selected = this.Selected
            };
        }

        public TableVisual()
        {
        }

        public TableVisual(Diagram diagram, Table table, int increment = 0)
        {
            Diagram = diagram;
            X = increment * 20;
            Y = increment * 10;
            W = 100;
            H = 100;

            Table = table;
        }

        [JsonIgnore]
        public List<SingleColumnRelationship> RelationshipsFrom { get; private set; }

        public void Draw(Graphics gfx)
        {

            var p = new Pen(Brushes.Black, Selected ? 2 : 1);

            gfx.FillRectangle(Selected ? Brushes.LightGray : Brushes.White, Diagram.OZR(X, Y, W, H));
            gfx.DrawRectangle(p, Diagram.OZR(X, Y, W, H));
            var font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size * Diagram.Zoom);
            gfx.DrawString(Table.Name, font, Brushes.Black, Diagram.OZRF(X+2, Y+2, W-4, (int)SystemFonts.DefaultFont.SizeInPoints + 4));
        }

        public void DrawRelationships(Graphics gfx)
        {
            var p = Selected ? new Pen(Brushes.Black, 2) : new Pen(Brushes.Gray, 1);

            p.DashPattern = new float[] { 4.0F, 4.0F };

            TableVisual dest;
            foreach (var rel in RelationshipsFrom)
            {
                if (!Diagram.Tables.TryGetValue(rel.ToTable, out dest)) continue;

                p.DashStyle = rel.IsActive ? DashStyle.Solid : DashStyle.Custom;

                if (dest.Selected || Selected) { p.Width = 2; p.Brush = Brushes.Black; }
                else { p.Width = 1; p.Brush = Brushes.Gray; }

                var x1 = X + W / 2;
                var y1 = Y + H / 2;
                var x2 = dest.X + dest.W / 2;
                var y2 = dest.Y + dest.H / 2;

                gfx.DrawLine(p, Diagram.OZ(x1, y1), Diagram.OZ(x2, y2));

                // Center of line:
                var ax = (x1 + x2) / 2;
                var ay = (y1 + y2) / 2;
                var angle = Math.Atan2((x2 - x1), (y1 - y2));

                // Point above line 1:
                var cos = Math.Cos(angle + 0.2);
                var sin = Math.Sin(angle + 0.2);
                var ax1a = (int)(ax + cos * 15);
                var ay1a = (int)(ay + sin * 15);

                // Point above line 2:
                cos = Math.Cos(angle - 0.2);
                sin = Math.Sin(angle +- 0.2);
                var ax1b = (int)(ax + cos * 15);
                var ay1b = (int)(ay + sin * 15);

                // Point below line 1:
                cos = Math.Cos(angle + Math.PI - 0.2);
                sin = Math.Sin(angle + Math.PI - 0.2);
                var ax2a = (int)(ax + cos * 15);
                var ay2a = (int)(ay + sin * 15);

                // Point below line 2:
                cos = Math.Cos(angle + Math.PI + 0.2);
                sin = Math.Sin(angle + Math.PI + 0.2);
                var ax2b = (int)(ax + cos * 15);
                var ay2b = (int)(ay + sin * 15);

                // Point along line:
                cos = Math.Cos(angle + Math.PI / 2);
                sin = Math.Sin(angle + Math.PI / 2);
                var ax3 = (int)(ax + cos * 20);
                var ay3 = (int)(ay + sin * 20);

                // Point behind line:
                cos = Math.Cos(angle + 3 * Math.PI / 2);
                sin = Math.Sin(angle + 3 * Math.PI / 2);
                var ax4 = (int)(ax + cos * 20);
                var ay4 = (int)(ay + sin * 20);

                if (rel.CrossFilteringBehavior == CrossFilteringBehavior.BothDirections)
                {
                    gfx.DrawLine(p, Diagram.OZ(ax4, ay4), Diagram.OZ(ax1b, ay1b));
                    gfx.DrawLine(p, Diagram.OZ(ax4, ay4), Diagram.OZ(ax1b, ay1b));
                }

                gfx.DrawLine(p, Diagram.OZ(ax3, ay3), Diagram.OZ(ax1a, ay1a));
                gfx.DrawLine(p, Diagram.OZ(ax3, ay3), Diagram.OZ(ax2a, ay2a));

            }
        }

        public bool HitTest(Point pt)
        {
            var r = Diagram.OZR(X, Y, H, W);
            return r.Contains(pt);
        }
    }

}