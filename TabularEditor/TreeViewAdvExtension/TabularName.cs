using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree.NodeControls;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Tree
{
    /// <summary>
    /// NodeControl that will return the translated name of the tabular object,
    /// given a specific Culture.
    /// </summary>
    public class TabularNodeTextBox : Aga.Controls.Tree.NodeControls.NodeTextBox
    {
        private UIController UI;

        protected override bool CanEdit(TreeNodeAdv node)
        {
            return node.Tag is ITabularNamedObject
                && !(node.Tag is LogicalGroup)
                && !(node.Tag is Culture)
                && !(node.Tag is Relationship);
        }

        public TabularNodeTextBox(UIController UI)
        {
            this.DrawText += TabularNodeTextBox_DrawText;
            this.UI = UI;
        }

        private void TabularNodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            bool hasTrans = ((e.Node.Tag as ITranslatableObject)?.TranslatedNames?.TranslatedCount > 0);

            if ((e.Node.Tag as Column)?.IsKey == true)
                e.Font = BoldFont;

            if (e.Node.Tag is IHideableObject)
            {
                e.TextColor = (e.Node.Tag as IHideableObject).IsHidden ?
                    (hasTrans ? Color.FromArgb(127, 127, 255) : Color.Gray) :
                    (hasTrans ? Color.Blue : e.Node.Tree.ForeColor);
            }
            else if (e.Node.Tag is Relationship)
            {
                e.TextColor = !(e.Node.Tag as Relationship).IsActive ? Color.Gray : e.Node.Tree.ForeColor;
            }
            else
            {
                e.TextColor = hasTrans ? Color.Blue : e.Node.Tree.ForeColor;
            }
        }

        private readonly Font BoldFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        public override string GetToolTip(TreeNodeAdv node)
        {
            var err = (node.Tag as IErrorMessageObject)?.ErrorMessage;
            return err ?? string.Empty;
        }

        public override object GetValue(TreeNodeAdv node)
        {
            var level = node?.Tag as Level;
            if(level != null)
            {
                return string.Format("{0} ({1})", level.GetName(UI.Tree?.Culture), level.Column.GetName(UI.Tree?.Culture));
            }
            return (node?.Tag as ITabularNamedObject)?.GetName(UI.Tree?.Culture);
        }

        public override void SetValue(TreeNodeAdv node, object value)
        {
            (node.Tag as ITabularNamedObject).SetName((string)value, UI.Tree?.Culture);
        }

        protected override Control CreateEditor(TreeNodeAdv node)
        {
            var ctr = base.CreateEditor(node) as TextBox;
            if(node?.Tag is Level)
            {
                ctr.Text = (node.Tag as Level).GetName(UI.Tree?.Culture);
            }
            return ctr;
        }
    }
}
