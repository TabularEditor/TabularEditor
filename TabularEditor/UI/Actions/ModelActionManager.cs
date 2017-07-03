using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Dialogs;

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
            var csDialog = new CultureSelectDialog();
            
            // Options to add a Measure or a Calculated Column will only be available when the current select consists of exactly
            // 1 item, which is either a Table or a Folder:
            Action.EnabledDelegate calcContext = (s, m) => s.DirectCount == 1 && ( s.Types == Types.Folder);

            Add(new CreateRelationshipAction(CreateRelationshipDirection.To));
            Add(new CreateRelationshipAction(CreateRelationshipDirection.From));

            // "Create New"
            Add(new Action((s, m) => s.Count == 1 && s.Types == Types.Measure, (s, m) => s.Measure.AddKPI().Edit(), (s, m) => @"Create New\KPI", true, Context.DataObjects));
            Add(new Action((s, m) => true, (s, m) => s.Table.AddMeasure(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Measure", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => true, (s, m) => s.Table.AddCalculatedColumn(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Calculated Column", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => true, (s, m) => s.Table.AddDataColumn(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Data Column", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => true || s.Direct.OfType<Column>().Any(), 
                (s, m) => s.Table.AddHierarchy(displayFolder: s.CurrentFolder, levels: s.Direct.OfType<Column>().ToArray()).Expand().Edit(), 
                (s, m) => @"Create New\Hierarchy", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => true, (s, m) => m.AddDataSource().Edit(), (s, m) => @"Create New\Data Source", false, Context.DataSources));
            Add(new Action((s, m) => true, (s, m) => m.AddCalculatedTable().Edit(), (s, m) => @"Create New\Calculated Table", false, Context.Tables));
            Add(new Action((s, m) => m.DataSources.Any(), (s, m) => m.AddTable().Edit(), (s, m) => @"Create New\Table", false, Context.Tables));
            Add(new Action((s, m) => m.Tables.Count(t => t.Columns.Any()) >= 2, (s, m) => m.AddRelationship().Edit(), (s, m) => @"Create New\Relationship", false, Context.Relationship | Context.Relationships));
            Add(new Action((s, m) => true, (s, m) => m.AddPerspective().Edit(), (s, m) => @"Create New\Perspective", false, Context.Model | Context.Perspectives | Context.Perspective));
            Add(new Action((s, m) => true, (s, m) => m.AddRole().Edit(), (s, m) => @"Create New\Role", false, Context.Model | Context.Roles | Context.Role));
            Add(new Action((s, m) => true, (s, m) => {
                var res = csDialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    m.AddTranslation(csDialog.SelectedCulture.Name).Edit();
                }
            }, (s, m) => @"Create New\Translation", true, Context.Model | Context.Translations | Context.Translation));

            // "Duplicate Table Object";
            Add(new Action((s, m) => true,
                (s, m) => s.ForEach(i =>
                {
                    var obj = (i as IClonableObject).Clone(includeTranslations: i is ITranslatableObject);
                    if (s.Count == 1) obj.Edit(); // Focuses the cloned item in the tree, and lets the user edit its name
                }),
                (s, m) => "Duplicate " + s.Summary(), true, Context.TableObject | Context.Partition ));

            Add(new Action((s, m) => s.DirectCount == 1 && s.Direct.First() is IDaxObject, (s, m) =>
            {
                UIController.Current.ShowDependencies(s.Direct.First() as IDaxObject);
            }, (s, m) => @"Show dependencies...", true, Context.Table | Context.TableObject));

            // "Duplicate Table";
            Add(new Action((s, m) => s.Count == 1, (s, m) => s.Table.Clone().Edit(), (s, m) => "Duplicate Table", true, Context.Table));

            // "Duplicate Translation";
            Add(new Action((s, m) => s.Count == 1,
                (s, m) => s.ForEach(i =>
                {
                        var res = csDialog.ShowDialog();
                        if (res == DialogResult.OK) (i as IClonableObject).Clone(csDialog.SelectedCulture.Name, false).Edit();
                }),
                (s, m) => "Duplicate " + s.Summary(), true, Context.Translation));

            // "Duplicate Role / Perspective":
            Add(new Action((s, m) => s.Count == 1, (s, m) => s.ForEach(i => (i as IClonableObject).Clone(null, true).Edit()), (s, m) => "Duplicate " + s.Summary(), true, Context.Role | Context.Perspective));

            // "Add to Hierarchy..."
            Add(new Separator());
            Add(new MultiAction((s, m, p) => p == null ? s.Direct.OfType<Column>().Any() : !(p as Hierarchy).Levels.Select(l => l.Column).Intersect(s.Direct.OfType<Column>()).Any(),
                (s, m, p) => (p as Hierarchy).AddLevels(s.Direct.OfType<Column>()),
                (s, m, p) => (p as Hierarchy).Name,
                (s, m) => s.Table.Hierarchies.AsEnumerable(), "Add to Hierarchy...", true, Context.TableObject | Context.Level));

            // Visibility and perspectives
            Add(new Action((s, m) => s.OfType<IHideableObject>().Any(o => o.IsHidden), (s, m) => s.IsHidden = false, (s, m) => "Make visible", true, Context.TableObject | Context.Table));
            Add(new Action((s, m) => s.OfType<IHideableObject>().Any(o => !o.IsHidden), (s, m) => s.IsHidden = true, (s, m) => "Make invisible", true, Context.TableObject | Context.Table));
            Add(new Action((s, m) => s.OfType<ITabularPerspectiveObject>().Any(), (s, m) => s.ShowInAllPerspectives(), (s, m) => @"Show in Perspectives\All Perspectives", true, Context.TableObject | Context.Table));
            Add(new Action((s, m) => s.OfType<ITabularPerspectiveObject>().Any(), (s, m) => s.HideInAllPerspectives(), (s, m) => @"Hide in Perspectives\All Perspectives", true, Context.TableObject | Context.Table));

            Add(new Separator(@"Show in Perspectives"));
            Add(new Separator(@"Hide in Perspectives"));

            Add(new MultiAction((s, m, p) => s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || !obj.InPerspective[p as Perspective]),
                (s, m, p) => s.ShowInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Show in Perspectives", true, Context.TableObject | Context.Table));

            Add(new MultiAction((s, m, p) => s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || obj.InPerspective[p as Perspective]),
                (s, m, p) => s.HideInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Hide in Perspectives", true, Context.TableObject | Context.Table));

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
            }, (s, m) => "Batch Rename...", true, Context.Table | Context.TableObject | Context.Level) { ToolTip = "Opens a dialog that lets you rename all the selected objects at once. Folders are not renamed, but objects inside folders are."});

            Add(new Action((s, m) => true, (s, m) =>
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
            }, (s, m) => "Batch Rename Children...", true, Context.Table)
            { ToolTip = "Opens a dialog that lets you rename all children of the selected objects at once. Folders are not renamed, but objects inside folders are." });

            // Delete Action
            // TODO: Would be nice to have an IDeletable interface...
            Delete = new Action((s, m) => s.All(obj => obj is IDeletableObject), 
                (s, m) => {
                    string refs = "";
                    if (s.Count == 1)
                    {
                        if(s.FirstOrDefault() is Column)
                        {
                            var hCount = (s.FirstOrDefault() as Column).UsedInHierarchies.Count();
                            if(hCount > 0)
                            {
                                refs += string.Format("\n\nThis column is used in {0} hierarch{1}. The corresponding level{2} will be removed from the hierarch{1}.", hCount, hCount == 1 ? "y" : "ies", hCount == 1 ? "" : "s");
                            }
                        }

                        var d = (s.FirstOrDefault() as IDaxObject);
                        if (d != null && d.Dependants.Count > 0)
                        {
                            refs += "\n\nThis object is directly referenced in the DAX expression on " + d.Dependants.First().DaxObjectFullName;
                            if (d.Dependants.Count > 1) refs += string.Format(" and {0} other object{1}.", d.Dependants.Count - 1, d.Dependants.Count == 2 ? "" : "s");
                        }
                        else if (!(s.FirstOrDefault() is KPI))
                        {
                            refs += "\n\nThis object does not appear to be referenced in DAX expressions on other objects.";
                        }
                    }

                    if (MessageBox.Show(string.Format("Are you sure you want to delete {0}?{1}", s.Name, refs), "Confirm deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;
                    s.Delete();
                }, (s, m) => "Delete " + s.Summary() + "...", true, Context.SingularObjects);
            Add(Delete);

            Add(new Separator());
            Add(new Action((s, m) => m.Cultures.Count > 0, (s, m) => UIController.Current.Translations_ExportAll(), (s, m) => "Export translations...", false, Context.Translations));
            Add(new Action((s, m) => true, (s, m) => UIController.Current.Translations_Import(), (s, m) => "Import translations...", true, Context.Translations));
            Add(new Action((s, m) => true, (s, m) => UIController.Current.Translations_ExportSelected(), (s, m) => string.Format("Export {0} translation{1}...", s.Count, s.Count == 1 ? "" : "s"), true, Context.Translation));

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
        public static T Expand<T>(this T obj) where T: ITabularNamedObject
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
        public static T Edit<T>(this T obj) where T: ITabularNamedObject
        {
            UIController.Current.Actions.LastActionExecuted.EditObjectName = obj;
            return obj;
        }
    }
}
