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
                && !(node.Tag is Relationship)
                && !(node.Tag is TablePermission)
                && !(node.Tag is KPI);
        }

        public TabularNodeTextBox(UIController UI)
        {
            this.DrawText += TabularNodeTextBox_DrawText;
            this.UI = UI;
        }

        private void TabularNodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            // toneBlue is set to 'true' if the current object has a translation applied. This menas that its text should be
            // rendered using a blue color to indicate that a translation exists on the object:
            bool toneBlue = ((e.Node.Tag as ITranslatableObject)?.TranslatedNames?.TranslatedCount > 0);

            // toneDown is set to 'true' if the current object is hidden or disabled (only applies to hideable objects or
            // relationships respectively):
            bool toneDown = (e.Node.Tag as TablePermission)?.NoEffect ?? (e.Node.Tag as IHideableObject)?.IsHidden ?? !((e.Node.Tag as Relationship)?.IsActive ?? true);

            // toneSelect is set to 'true' if the current object has been selected in the tree (and thus will have a blue
            // background applied):
            bool toneSelect = e.Context.DrawSelection == DrawSelectionMode.Active || e.Context.DrawSelection == DrawSelectionMode.FullRowSelect;

            // Key columns should be rendered using a bold font:
            if (e.Node.Tag is Column c && c.IsKey == true) e.Font = BoldFont;

            e.TextColor =
                toneSelect ?
                    (toneBlue ?
                        (toneDown ? Color.FromArgb(161,161,211) : Color.FromArgb(211, 211, 255)) :
                        (toneDown ? Color.Silver : SystemColors.HighlightText)) :
                    (toneBlue ? 
                        (toneDown ? Color.FromArgb(111,111,191) : Color.FromArgb(0,0,161)) : 
                        (toneDown ? Color.Gray : SystemColors.ControlText));
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
            if(level != null && level.Column != null)
            {
                return string.Format("{0} ({1})", level.GetName(UI.TreeModel?.Culture), level.Column.GetName(UI.TreeModel?.Culture));
            }
            return (node?.Tag as ITabularNamedObject)?.GetName(UI.TreeModel?.Culture);
        }

        public override void SetValue(TreeNodeAdv node, object value)
        {
            (node.Tag as ITabularNamedObject).SetName((string)value, UI.TreeModel?.Culture);
        }

        protected override Control CreateEditor(TreeNodeAdv node)
        {
            var ctr = base.CreateEditor(node) as TextBox;
            if(node?.Tag is Level)
            {
                ctr.Text = (node.Tag as Level).GetName(UI.TreeModel?.Culture);
            }
            return ctr;
        }
    }
}
