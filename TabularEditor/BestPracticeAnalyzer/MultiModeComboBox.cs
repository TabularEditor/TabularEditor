using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.BestPracticeAnalyzer
{
    public class MultiModeComboBox : ComboBox
    {
        public MultiModeComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            Items.AddRange(new[] { "all", "any", "none", "not all" });
            Width = 60;
            SelectedIndex = 0;
            SelectionChangeCommitted += MultiModeCombobox_SelectionChangeCommitted;
        }

        private void MultiModeCombobox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (multiNode != null)
            {
                if (SelectedIndex == 0) { MultiNode.Mode = MultiNode.Operator.All; MultiNode.Not = false; }
                else if (SelectedIndex == 1) { MultiNode.Mode = MultiNode.Operator.Any; MultiNode.Not = false; }
                else if (SelectedIndex == 2) { MultiNode.Mode = MultiNode.Operator.Any; MultiNode.Not = true; }
                else { MultiNode.Mode = MultiNode.Operator.All; MultiNode.Not = true; }
            }
        }

        private MultiNode multiNode;

        public MultiNode MultiNode
        {
            get
            {
                return multiNode;
            }

            set
            {
                multiNode = value;
                if (multiNode != null)
                {
                    if (MultiNode.Mode == MultiNode.Operator.All && !MultiNode.Not) SelectedIndex = 0;
                    else if (multiNode.Mode == MultiNode.Operator.Any && !multiNode.Not) SelectedIndex = 1;
                    else if (multiNode.Mode == MultiNode.Operator.Any && multiNode.Not) SelectedIndex = 2;
                    else if (multiNode.Mode == MultiNode.Operator.All && multiNode.Not) SelectedIndex = 3;
                }
            }
        }
    }

}
