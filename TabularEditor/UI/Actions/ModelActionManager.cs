using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Actions
{
    public class ModelActionManager : List<IBaseAction>
    {
        /// <summary>
        /// When an Action is executed, it will set this field.
        /// </summary>
        public Action LastActionExecuted = null;

        public Action Delete { get; private set; }

        public void CreateStandardActions()
        {
            // Options to add a Measure or a Calculated Column will only be available when the current select consists of exactly
            // 1 item, which is either a Table or a Folder:
            Action.EnabledDelegate calcContext = (s, m) => s.DirectCount == 1 && s.Types.Has1(Types.Table | Types.Folder);

            // "Create New"
            Add(new Action(calcContext, (s, m) => s.Table.AddMeasure(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Measure"));
            Add(new Action(calcContext, (s, m) => s.Table.AddCalculatedColumn(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Calculated Column"));
            Add(new Action((s, m) => calcContext(s,m) || s.Direct.OfType<Column>().Any(), 
                (s, m) => s.Table.AddHierarchy(displayFolder: s.CurrentFolder, levels: s.Direct.OfType<Column>().ToArray()).Expand().Edit(), 
                (s, m) => @"Create New\Hierarchy"));

            // "Duplicate";
            Add(new Action((s, m) => s.Types.HasX(Types.CalculatedColumn | Types.Measure) && s.Types.Has0(Types.DataColumn | Types.Hierarchy),
                (s, m) => s.ForEach(i => i.Clone(null, true)),
                (s, m) => "Duplicate " + s.Summary(), true));

            // "Add to Hierarchy..."
            Add(new Separator());
            Add(new MultiAction((s, m, p) => p == null ? s.Direct.OfType<Column>().Any() : !(p as Hierarchy).Levels.Select(l => l.Column).Intersect(s.Direct.OfType<Column>()).Any(),
                (s, m, p) => (p as Hierarchy).AddLevels(s.Direct.OfType<Column>()),
                (s, m, p) => (p as Hierarchy).Name,
                (s, m) => s.Table.Hierarchies.AsEnumerable(), "Add to Hierarchy...", true));

            // Visibility and perspectives
            Add(new Action((s, m) => s.OfType<IHideableObject>().Any(o => o.IsHidden), (s, m) => s.IsHidden = false, (s, m) => "Make visible"));
            Add(new Action((s, m) => s.OfType<IHideableObject>().Any(o => !o.IsHidden), (s, m) => s.IsHidden = true, (s, m) => "Make invisible"));
            Add(new Action((s, m) => s.OfType<ITabularPerspectiveObject>().Any(), (s, m) => s.ShowInAllPerspectives(), (s, m) => @"Show in Perspectives\All Perspectives", true));
            Add(new Action((s, m) => s.OfType<ITabularPerspectiveObject>().Any(), (s, m) => s.HideInAllPerspectives(), (s, m) => @"Hide in Perspectives\All Perspectives", true));

            Add(new Separator(@"Show in Perspectives"));
            Add(new Separator(@"Hide in Perspectives"));

            Add(new MultiAction((s, m, p) => s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || !obj.InPerspective[p as Perspective]),
                (s, m, p) => s.ShowInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Show in Perspectives", true));

            Add(new MultiAction((s, m, p) => s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || obj.InPerspective[p as Perspective]),
                (s, m, p) => s.HideInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Hide in Perspectives", true));

            // Rename Dialogs
            Add(new Separator());
            Add(new Action((s, m) => s.Count > 1, (s, m) =>
            {
                var form = Dialogs.ReplaceForm.Singleton;
                form.Text = "Batch Rename - (" + s.Summary() + " selected)";
                var res = form.ShowDialog();
                if (res == DialogResult.Cancel) return;
                // TODO: Add options for match case and whole word only
                s.Rename(form.Pattern, form.ReplaceWith, form.RegEx, form.IncludeTranslations);
            }, (s, m) => "Batch Rename...", true) { ToolTip = "Opens a dialog that lets you rename all the selected objects at once. Folders are not renamed, but objects inside folders are."});

            Add(new Action((s, m) => s.Types.Has1(Types.Table | Types.Hierarchy), (s, m) =>
            {
                var form = Dialogs.ReplaceForm.Singleton;
                var sel = new UISelectionList<ITabularNamedObject>(
                    s.Tables.SelectMany(t => t.GetChildren())
                    .Concat(s.Hierarchies.SelectMany(t => t.Levels)));
                form.Text = "Batch Rename - (" + sel.Summary() + ")";
                var res = form.ShowDialog();
                if (res == DialogResult.Cancel) return;
                // TODO: Add options for match case and whole word only
                sel.Rename(form.Pattern, form.ReplaceWith, form.RegEx, form.IncludeTranslations);
            }, (s, m) => "Batch Rename Children...", true)
            { ToolTip = "Opens a dialog that lets you rename all children of the selected objects at once. Folders are not renamed, but objects inside folders are." });

            // Delete Action
            Delete = new Action((s, m) => s.DirectCount >= 1 && !s.Types.HasFlag(Types.Model), 
                (s, m) => {
                    if (MessageBox.Show("Are you sure you want to delete " + s.Summary() + "?", "Confirm deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;
                    s.Delete();
                }, (s, m) => "Delete " + s.Summary() + "...");
            Add(Delete);
        }
    }

    public static class TabularObjectTreeHelper
    {
        /// <summary>
        /// This method flags to the action, that the TreeView should expand the object
        /// after it has been created. For example, when a new hierarchy with predefined
        /// levels have been added, we would like to show the levels immediately after
        /// creation.
        /// </summary>
        public static T Expand<T>(this T obj) where T: TabularNamedObject
        {
            UIController.Current.Actions.LastActionExecuted.ExpandObject = obj;
            return obj;
        }
        /// <summary>
        /// This method flags to the action, that the TreeView should edit the object name
        /// immediately after it has been created. Useful for actions that add new objects
        /// to the model tree, when you want end users to be able to provide a name for
        /// the new object immediately after creation.
        /// </summary>
        public static T Edit<T>(this T obj) where T: TabularNamedObject
        {
            UIController.Current.Actions.LastActionExecuted.EditObjectName = obj;
            return obj;
        }
    }
}
