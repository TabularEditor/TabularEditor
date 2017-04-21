using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.UI;

namespace TabularEditor.BestPracticeAnalyzer
{
    public class MultiNodePanel : BaseNodePanel
    {
        public new MultiNode Node {
            get {
                return base.Node as MultiNode;
            }
            set
            {
                base.Node = value;
            }
        }

        public MultiNodePanel(Panel parent, MultiNode node) : base(parent, node)
        {
            Height = PanelHeight + 6;

            ChildPanel = new Panel();
            ChildPanel.BorderStyle = BorderStyle.FixedSingle;
            ChildPanel.Height = 1;
            ChildPanel.Width = ClientSize.Width - 35;
            ChildPanel.Left = 30;
            ChildPanel.Top = PanelHeight;
            ChildPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(ChildPanel);

            var cmb = new MultiModeComboBox();
            cmb.MultiNode = node;
            cmb.Top = MarginTop;
            cmb.Left = MarginLeft;
            cmb.SelectionChangeCommitted += (s, e) => UpdateLabels();
            Controls.Add(cmb);

            var lbl = new Label();
            lbl.Text = "of the following conditions apply:";
            lbl.AutoSize = true;
            lbl.Top = MarginTop + 3;
            lbl.Left = 70;
            Controls.Add(lbl);
        }

        List<Label> boolLabels = new List<Label>();
        public void UpdateLabels(bool recursive = false)
        {
            foreach (var lbl in boolLabels)
            {
                Controls.Remove(lbl);
                lbl.Dispose();
            }
            boolLabels.Clear();

            var h = PanelHeight - 6;
            for (var i = 1; i < ChildPanel.Controls.Count; i++)
            {
                h += ChildPanel.Controls[ChildPanel.Controls.Count - i].Height;
                var lbl = new Label()
                {
                    Left = Node.Mode == MultiNode.Operator.All ? 2 : 7,
                    Top = h,
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Text = Node.Mode == MultiNode.Operator.All ? "AND" : "OR"
                };
                Controls.Add(lbl);
                boolLabels.Add(lbl);
            }

            if(recursive)
            {
                foreach (var p in ChildPanel.Controls.Cast<Panel>().OfType<MultiNodePanel>()) p.UpdateLabels(true);
            }
        }

        public void RemoveChild(BaseNodePanel panel)
        {
            IncreaseHeight(-panel.Height);
            ChildPanel.Controls.Remove(panel);

            // TODO: Update the Expression Node tree
            if(ChildPanel.Controls.Count == 1)
            {
                (ChildPanel.Controls[0] as BaseNodePanel).MoveUp();
            }

            if (ChildPanel.Controls.Count == 0)
            {
                Delete();
                return;
            }

            UpdateLabels();
        }

        public void AddChild(BaseNodePanel panel, int index = 0)
        {
            ControlHelper.SuspendDrawing(this);

            ChildPanel.Controls.Add(panel);
            ChildPanel.Controls.SetChildIndex(panel, index);
            IncreaseHeight(panel is MultiNodePanel ? PanelHeight + 6 : PanelHeight);

            ControlHelper.ResumeDrawing(this);
        }

        private void IncreaseHeight(int amount)
        {
            Height += amount;
            (Parent?.Parent as MultiNodePanel)?.IncreaseHeight(amount);
        }

        Panel ChildPanel;
    }
}
