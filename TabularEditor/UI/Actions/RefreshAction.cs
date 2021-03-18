using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Actions
{
    class RefreshAction : ICustomMenuAction, IModelAction
    {
        RefreshType refreshType = RefreshType.Automatic;
        bool scriptToClipboard;
        public Keys Shortcut => Keys.None;

        public RefreshAction(bool scriptToClipboard)
        {
            this.scriptToClipboard = scriptToClipboard;
        }

        public bool HideWhenDisabled
        {
            get
            {
                return true;
            }
        }

        public string ToolTip
        {
            get
            {
                switch(refreshType)
                {
                    case RefreshType.Full: return "Refresh data and recalculate all dependents.";
                    case RefreshType.ClearValues: return "Clear values in this object and all dependents.";
                    case RefreshType.Calculate: return "Recalculate this object and all its dependents, but only if needed. This does not force recalculation, except for volatile formulas.";
                    case RefreshType.DataOnly: return "Refresh data in this object and clear all dependents.";
                    case RefreshType.Automatic: return "If the object needs to be refreshed and recalculated, refresh and recalculate the object and all its dependents. Applies to a partition if its state is anything other than Ready.";
                    case RefreshType.Add: return "Append data to this partition and recalculate all dependents. This command is valid only for regular partitions and not for calculation partitions.";
                    case RefreshType.Defragment: return "Defragment the data in the specified table. As data is added to or removed from a table, the dictionaries of each column can become polluted with values that no longer exist in the actual column values. The defragment option will clean up the values in the dictionaries that are no longer used.";
                }
                return null;
            }
        }

        public RefreshType[] GetValidRefreshTypes(Context currentContext)
        {
            var allValues = (RefreshType[])Enum.GetValues(typeof(RefreshType));

            switch (currentContext)
            {
                case Context.Model:
                    return allValues.Except(new[] { RefreshType.Defragment, RefreshType.Add }).ToArray();
                case Context.Table:
                    return allValues.Except(new[] { RefreshType.Add }).ToArray();
                case Context.Partition:
                    return allValues.Except(new[] { RefreshType.Defragment }).ToArray(); ;
            }
            return new RefreshType[] { };
        }

        public Context ValidContexts
        {
            get
            {
                return Context.Model | Context.Table | Context.Partition;
            }
        }

        public string Name
        {
            get
            {
                return @"Script\Refresh\" + (scriptToClipboard ? "To clipboard" : "To file...");
            }
        }

        public bool Enabled(object arg)
        {
            return UIController.Current.Selection.Context.Has1(ValidContexts);
        }

        public void Execute(object arg)
        {
            var script = 
                TOMWrapper.Utils.Scripter.ScriptRefresh(UIController.Current.Selection.OfType<TabularNamedObject>(), refreshType);

            if (scriptToClipboard) Clipboard.SetText(script);
            else ModelActionManager.SaveScriptToFile(script);
        }

        ToolStripDropDown parent;

        public void InitMenu(ToolStripItem item, Context currentContext)
        {
            parent = item.Owner as ToolStripDropDown;

            // If the first item of the parent is a ToolStripLabel, it means this menu was
            // already populated (likely because we created 2 RefreshActions - one for clipboard
            // and one for script to file.
            if (parent.Items[0] is ToolStripLabel) return;

            var modes = GetValidRefreshTypes(currentContext).ToList();

            if (modes.Count > 1)
            {
                var cmb = new ToolStripComboBox();
                cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                cmb.Items.AddRange(modes.Select(m => m.ToString()).ToArray());
                var currentMode = modes.IndexOf(refreshType);
                if (currentMode < 0) currentMode = modes.IndexOf(RefreshType.Automatic);
                if (currentMode < 0) currentMode = 0;
                refreshType = modes[currentMode];
                cmb.SelectedIndex = currentMode < 0 ? 0 : currentMode;
                cmb.SelectedIndexChanged += (s, e) => {
                    Enum.TryParse(cmb.Text, out refreshType);
                };

                parent.Items.Insert(0, new ToolStripLabel("Refresh mode:"));
                parent.Items.Insert(1, cmb);
                parent.Items.Insert(2, new ToolStripSeparator());
            }
        }
    }
}
