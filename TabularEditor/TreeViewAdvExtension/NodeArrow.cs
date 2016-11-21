using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TabularEditor.TreeViewAdvExtension
{
    public class NodeArrow : NodeControl
    {
        public const int ImageSize = 14;
        public const int Width = 14;
        private Bitmap _plus;
        private Bitmap _minus;

        public NodeArrow()
        {
            _plus = TVExtRessources.Expand;
            _minus = TVExtRessources.Collapse;
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            return new Size(Width, Width);
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            if (node.CanExpand)
            {
                Rectangle r = context.Bounds;
                int dy = (int)Math.Round((float)(r.Height - ImageSize) / 2);

                Image img;
                if (node.IsExpanded)
                    img = _minus;
                else
                    img = _plus;
                context.Graphics.DrawImageUnscaled(img, new Point(r.X, r.Y + dy));
                
            }
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                args.Handled = true;
                if (args.Node.CanExpand)
                    args.Node.IsExpanded = !args.Node.IsExpanded;
            }
        }

        public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
        {
            args.Handled = true; // Supress expand/collapse when double click on plus/minus
        }
    }
}
