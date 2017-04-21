using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.BestPracticeAnalyzer
{
    static public class PropCache
    {
        static Dictionary<Type, Dictionary<string, Type>> cache = new Dictionary<Type, Dictionary<string, Type>>();

        static public Dictionary<string, Type> Get(Type type)
        {
            Dictionary<string, Type> res;
            if(!cache.TryGetValue(type, out res))
            {
                res = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => pi.GetIndexParameters().Length == 0 && pi.GetMethod != null && pi.GetCustomAttribute<TabularEditor.TOMWrapper.IntelliSenseAttribute>() != null)
                    .ToDictionary(pi => pi.Name, pi => pi.PropertyType);
                cache.Add(type, res);
            }
            return res;
        }
    }

    public class CriteriaNodePanel : BaseNodePanel
    {
        private int SuggestedOperatorComboBoxWidth()
        {
            if (Node.PropertyPath.Count == 0 || Node.PropertyPath.Last().Member == null) return 40;
            if (Node.LastType == typeof(string)) return 90;
            if (Node.LastType == typeof(bool)) return 40;
            if (Node.LastType.IsEnum) return 20;
            if (Node.LastType.IsPrimitive) return 20;
            else return 40;
        }

        public new CriteriaNode Node
        {
            get
            {
                return base.Node as CriteriaNode;
            }
            set
            {
                base.Node = value;
            }
        }
        ComboBox operatorCombo;
        TextBox valueTextBox;
        CheckBox valueCaseSensitivity;
        ComboBox valueComboBox;

        private int CalcWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 60;

            var textWidth = TextRenderer.MeasureText(text, Font).Width;

            // Round up to 10:
            return textWidth + (9 - textWidth % 10) + 18;
        }

        FlowLayoutPanel flow;
        static ToolTip combtt = new ToolTip();

        public new void Focus()
        {
            flow.Controls[0].Focus();
        }

        Stack<PropertySelector> PropertySelectorStack = new Stack<PropertySelector>();
        private class PropertySelector
        {
            public ComboBox combo;
            public Button expand;
            public Type objectType;

            public Dictionary<string, Type> props { get { return PropCache.Get(objectType); } }

            /// <summary>
            /// Returns true if the current selection in the combobox is a valid property of the object
            /// </summary>
            public bool isProp { get { return selectedValue != null && props.ContainsKey(selectedValue); } }

            /// <summary>
            /// Returns true if the current selection in the combobox is a custom text item
            /// </summary>
            public bool isCustom { get { return !string.IsNullOrEmpty(selectedValue) && !props.ContainsKey(selectedValue); } }

            /// <summary>
            /// Gets the current (textual) value of the combobox
            /// </summary>
            public string selectedValue { get { return combo.SelectedItem?.ToString(); } }

            /// <summary>
            /// Gets the property associated with the current selection
            /// </summary>
            public Type selectedProp { get { return selectedValue != null && props.ContainsKey(selectedValue) ? props[selectedValue] : null; } }

            /// <summary>
            /// Returns true if the currently selected property is an object (not a string)
            /// Drill-down to nested properties should be possible when this is true.
            /// </summary>
            public bool isObject { get { return selectedProp != null && selectedProp.IsClass && selectedProp != typeof(string); } }

            public void UpdateTooltip()
            {
                if (string.IsNullOrEmpty(selectedValue))
                {
                    combtt.SetToolTip(combo, "Choose an object property");
                }
                else
                {
                    if (selectedProp == null)
                    {
                        combtt.SetToolTip(combo, "Custom expression");
                    }
                    else {
                        combtt.SetToolTip(combo, string.Format("{0} property of {1}{2}",
                            selectedProp.Name,
                            objectType.Name,
                            isObject ? " (click \u2022 to expand/collapse)" : ""));
                    }
                }
            }

            public void Delete()
            {
                combo.Parent.Controls.Remove(combo);
                expand.Parent.Controls.Remove(combo);
                combo.Dispose();
                expand.Dispose();
            }
        }

        private void AddPropCombo(Control parent, string selectedValue, Type objectType)
        {
            var pc = new PropertySelector { objectType = objectType };
            var props = PropCache.Get(objectType);

            PropertySelectorStack.Push(pc);

            pc.combo = new ComboBox()
            {
                Margin = new Padding(2, MarginTop, 2, 0),
                Text = selectedValue,
                Width = CalcWidth(selectedValue),
                DropDownWidth = 150,
                Tag = pc
            };
            pc.combo.Items.AddRange(props.Keys.OrderBy(n => n).ToArray());
            pc.combo.SelectedItem = selectedValue;
            pc.combo.SelectionChangeCommitted += PropComboSelectionChanged;
            pc.UpdateTooltip();

            pc.expand = new Button()
            {
                Margin = new Padding(-2, MarginTop - 1, -2, MarginTop),
                Text = "\u2022",
                Width = 18,
                Tag = pc,
                Visible = selectedValue != null && props.ContainsKey(selectedValue) && props[selectedValue].IsClass && props[selectedValue] != typeof(string)
            };

            pc.expand.Click += Expand_Click;

            var operatorIndex = parent.Controls.IndexOf(operatorCombo);
            parent.Controls.Add(pc.combo);
            if(operatorIndex >= 0) parent.Controls.SetChildIndex(pc.combo, operatorIndex);
            parent.Controls.Add(pc.expand);
            if (operatorIndex >= 0) parent.Controls.SetChildIndex(pc.expand, operatorIndex + 1);
        }

        private void Expand_Click(object sender, EventArgs e)
        {
            var pc = (sender as Control).Tag as PropertySelector;

            if (!pc.isObject) throw new NotSupportedException(); // Can't expand non-objects

            if(PropertySelectorStack.Peek() == pc)
            {
                // Last-most item - clicking will expand:
                Node.PropertyPath.Push(new CriteriaNode.PropertyPathItem());
                AddPropCombo(pc.expand.Parent, null, pc.selectedProp);
                PropertySelectorStack.Peek().combo.Focus();
            }
            else
            {
                // Not the last-most item - clicking should collapse all following items:
                while (PropertySelectorStack.Peek() != pc) PropertySelectorStack.Pop().Delete();
                while (Node.PropertyPath.Count > PropertySelectorStack.Count) Node.PropertyPath.Pop();
                PropertySelectorStack.Peek().combo.Focus();
            }

            UpdateOperatorAndValue();
        }

        private void UpdateOperatorAndValue()
        {
            if(PropertySelectorStack.Count == 0)
            {
                operatorCombo.Visible = false;
                valueComboBox.Visible = false;
                valueTextBox.Visible = false;
                return;
            }
            var pc = PropertySelectorStack.Peek();

            operatorCombo.Visible = pc.isProp || pc.isCustom;
            valueComboBox.Visible = pc.isProp && pc.selectedProp.IsEnum;
            var vtVisible = (pc.isProp && !pc.isObject && !pc.selectedProp.IsEnum && pc.selectedProp != typeof(bool)) || pc.isCustom;

            operatorCombo.Items.Clear();

            Node.PropertyPath.Peek().Member = pc.isProp ? pc.objectType.GetProperty(pc.selectedValue) : null;

            if (!Node.IsBlank)
            {
                operatorCombo.Items.AddRange(Node.ValidOperators.AsNames());

                operatorCombo.SelectedItem = Node.Op.GetName();
                if (operatorCombo.SelectedItem == null) operatorCombo.SelectedItem = operatorCombo.Items[0];

                Node.Op = OperatorHelper.Parse(operatorCombo.SelectedItem.ToString());
                vtVisible = vtVisible && !Node.Op.IsUnary();

                operatorCombo.Width = CalcWidth(operatorCombo.SelectedItem.ToString());
                operatorCombo.DropDownWidth = SuggestedOperatorComboBoxWidth();

                if(Node.CriteriaType.IsEnum)
                {
                    valueComboBox.Items.Clear();
                    if (Node.Value == null || !(Node.Value is Enum)) Node.Value = Enum.GetValues(Node.CriteriaType).GetValue(0);
                    valueComboBox.Width = CalcWidth(Enum.GetName(Node.CriteriaType, Node.Value));
                    valueComboBox.Items.AddRange(Enum.GetNames(Node.CriteriaType));
                    valueComboBox.SelectedItem = Enum.GetName(Node.CriteriaType, Node.Value);
                }
            }

            valueCaseSensitivity.Visible = pc.isProp && pc.selectedProp == typeof(string) && Node.Op.CaseSensitivity();
            valueTextBox.Visible = vtVisible;

            pc.combo.Width = CalcWidth(pc.selectedValue);
        }

        private void PropComboSelectionChanged(object sender, EventArgs e)
        {
            var pc = (sender as Control).Tag as PropertySelector;

            pc.expand.Visible = pc.isObject;
            pc.UpdateTooltip();

            while(PropertySelectorStack.Peek() != pc) PropertySelectorStack.Pop().Delete();
            while (Node.PropertyPath.Count > PropertySelectorStack.Count) Node.PropertyPath.Pop();

            if (pc.combo.SelectedItem != null)
                Node.PropertyPath.Peek().Member = pc.objectType.GetProperty(pc.combo.SelectedItem.ToString());
            else
                Node.PropertyPath.Peek().Member = null;

            UpdateOperatorAndValue();
        }

        public CriteriaNodePanel(Type baseType, Panel parent, CriteriaNode node) : base(parent, node)
        {
            Node = node;
            flow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, ContextMenuStrip = this.ContextMenuStrip };
            
            // Operator:
            operatorCombo = new ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(2, MarginTop, 2, 0),
                Width = CalcWidth(node.Op.GetName()),
                DropDownWidth = SuggestedOperatorComboBoxWidth()
            };
            operatorCombo.SelectionChangeCommitted += OperatorChanged;

            // Value (combobox for enum object types):
            valueComboBox = new ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(2, MarginTop, 2, 0),
                DropDownWidth = 120
            };
            valueComboBox.SelectionChangeCommitted += (s, e) => node.Value = valueComboBox.SelectedItem;

            // Value (textbox for non-enum object types):
            valueTextBox = new TextBox()
            {
                Margin = new Padding(2, MarginTop + 1, 2, 0),
                Width = 100
            };
            valueTextBox.LostFocus += (s, e) => { node.Value = valueTextBox.Text; };
            if (node.Value != null) valueTextBox.Text = node.Value.ToString();

            // Checkbox for case sensitivity for string criteria:
            valueCaseSensitivity = new CheckBox()
            {
                Text = "Case sensitive",
                Margin = new Padding(2, MarginTop + 3, 2, 0),
                AutoSize = true,
                Checked = node.CaseSensitive
            };
            valueCaseSensitivity.CheckedChanged += (s, e) => { node.CaseSensitive = valueCaseSensitivity.Checked; };

            // First property combobox which always appears, based on baseType (last item in stack):
            var firstPropValue = node.IsBlank ? null : node.PropertyPath.Last().Member.Name;
            AddPropCombo(flow, firstPropValue, baseType);

            if (firstPropValue != null)
            {

                // Sequence of sub-properties (may appear 0..x times based on how deep we go):
                if (node.PropertyPath.Count > 1)
                {
                    var i = 1;
                    foreach (var p in node.PropertyPath.Reverse().Skip(1))
                    {
                        AddPropCombo(flow, p.Member.Name, p.Member.ReflectedType);
                        i++;
                    }
                }
            }

            flow.Controls.Add(operatorCombo);
            flow.Controls.Add(valueComboBox);
            flow.Controls.Add(valueTextBox);
            flow.Controls.Add(valueCaseSensitivity);

            Controls.Add(flow);

            if (Node.PropertyPath.Count == 0) Node.PropertyPath.Push(new CriteriaNode.PropertyPathItem());
            UpdateOperatorAndValue();
            flow.Paint += Flow_Paint;
        }

        public override void Refresh()
        {
            base.Refresh();
            flow.Refresh();
        }

        private void Flow_Paint(object sender, PaintEventArgs e)
        {
            var h = ClientSize.Height - 1;
            var w = ClientSize.Width - 1;

            e.Graphics.DrawLine(Pens.LightGray, 0, h, w, h);

            if (Highlight)
            {
                var pen = new Pen(Color.Blue, 2);
                var rect = e.ClipRectangle;
                e.Graphics.DrawRectangle(pen, 1, 1, w, h - 2);
            }
        }

        private void OperatorChanged(object sender, EventArgs e)
        {
            var pc = PropertySelectorStack.Peek();

            Node.Op = OperatorHelper.Parse(operatorCombo.SelectedItem.ToString());
            operatorCombo.Width = CalcWidth(operatorCombo.SelectedItem.ToString());
            valueTextBox.Visible = !Node.Op.IsUnary();
            valueCaseSensitivity.Visible = pc.selectedProp == typeof(string) && Node.Op.CaseSensitivity();
            if(valueTextBox.Visible) valueTextBox.Focus();
        }
    }

}
