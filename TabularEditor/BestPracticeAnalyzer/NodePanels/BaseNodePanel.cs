using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.BestPracticeAnalyzer
{
    // Panels handle user interaction in the BPAEditorForm. They are responsible for updating the underlying CriteriaTree structure,
    // whenever rules are added, changed, etc. Furthermore, the BaseNodePanel.GetPanelForNode method may be used to build the panel
    // structure from an existing CriteriaTree structure.

    /// <summary>
    /// An object that support the nesting of multiple panels below it
    /// </summary>
    public class BaseNodePanel: Panel
    {
        public const int PanelHeight = 26;
        public const int MarginTop = 2;
        public const int MarginLeft = 3;

        public static ContextMenuStrip RightClickMenu;

        public BaseNode Node;

        public void Delete()
        {
            if(ParentPanel != null)
            {
                ParentPanel.RemoveChild(this);
            }

            Parent?.Controls?.Remove(this);
            Dispose();
        }

        static public BaseNodePanel GetPanelForNode(Type baseType, BaseNode node, Panel parent)
        {
            if(node is MultiNode)
            {
                var pnl = new MultiNodePanel(parent, node as MultiNode);
                foreach (var c in (node as MultiNode).Children) GetPanelForNode(baseType, c, pnl);
                return pnl;
            }
            else if(node is CriteriaNode)
            {
                return new CriteriaNodePanel(baseType, parent, node as CriteriaNode);
            }
                
            throw new NotSupportedException();
            //new BaseNodePanel(parent, node);
        }

        public BaseNodePanel(Panel parent, BaseNode node)
        {
            Node = node;

            ContextMenuStrip = RightClickMenu;

            Dock = DockStyle.Top;
            Height = PanelHeight;

            if (parent is MultiNodePanel)
            {
                (parent as MultiNodePanel).AddChild(this);
            }
            else
            {
                parent.Controls.Add(this);
                parent.Controls.SetChildIndex(this, 0);
            }
            Indent = ParentPanel == null ? 0 : (ParentPanel.Indent + 1);
        }
        private int _indent;
        private int Indent
        {
            get { return _indent; }
            set
            {
                _indent = value;
                BackColor = Color.FromArgb(255 - _indent * 9, 255 - _indent * 14, 240 - _indent * 12);
            }
        }

        public void MoveUp()
        {
            if (ParentPanel == null) return;

            Indent--;
            var grandParentIndex = ParentPanel.Parent.Controls.IndexOf(ParentPanel);

            var grandParent = ParentPanel.ParentPanel;
            var grandParentPanel = ParentPanel.Parent;

            ParentPanel.RemoveChild(this);
            if (grandParent != null) grandParent.AddChild(this, grandParentIndex);
            else
            {
                grandParentPanel.Controls.Add(this);
                grandParentPanel.Controls.SetChildIndex(this, grandParentIndex);
            }
        }

        public MultiNodePanel ParentPanel { get { return Parent?.Parent as MultiNodePanel; } }

        private bool highlight;

        public bool Highlight
        {
            get
            {
                return highlight;
            }

            set
            {
                highlight = value;
                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var h = ClientSize.Height - 1;
            var w = ClientSize.Width - 1;

            e.Graphics.DrawLine(Pens.LightGray, 0, h, w, h);
            
            if(highlight)
            {
                var pen = new Pen(Color.Blue, 2);
                var rect = e.ClipRectangle;
                e.Graphics.DrawRectangle(pen, 1, 1, w - 1, h - 2);
            }
        }
    }
}
