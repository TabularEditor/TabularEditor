using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI.Dialogs;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.UI.Actions
{
    public class ModelActionManager : List<IBaseAction>
    {
        public void RemoveCustomActions()
        {
            for(var i = this.Count - 1; i >= 0; i--)
            {
                if (this[i] is CustomAction) this.RemoveAt(i);
            }
        }

        /// <summary>
        /// When an Action is executed, it will set this field.
        /// </summary>
        public Action LastActionExecuted = null;

        public Action Delete { get; private set; }

        public TabularModelHandler Handler { get { return UI.UIController.Current.Handler; } }

        public void CreateStandardActions()
        {
            var csDialog = new CultureSelectDialog();

            // "Create New"
            Add(new Action((s, m) => s.Count >= 1 && !Handler.UsePowerBIGovernance, 
                (s, m) => {
                var disp = (s.FirstOrDefault() as IDetailObject)?.DisplayFolder;
                disp = string.IsNullOrWhiteSpace(disp) ? "New folder" : (disp + @"\New folder"); ;
                Folder.CreateFolder(s.Table, disp).Edit();
                s.DisplayFolder = disp;
                
            }, (s, m) => @"Create New\Display Folder", true, Context.TableObject));
            Add(new Action((s, m) => s.Count == 1 && s.Types == Types.Measure, (s, m) => s.Measure.AddKPI().Edit(), (s, m) => @"Create New\KPI", true, Context.DataObjects));
            Add(new Action((s, m) => true, (s, m) => s.Table.AddMeasure(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Measure", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => true, (s, m) => s.Table.AddCalculatedColumn(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Calculated Column", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => !Handler.UsePowerBIGovernance, (s, m) => s.Table.AddDataColumn(displayFolder: s.CurrentFolder).Edit(), (s, m) => @"Create New\Data Column", true, Context.Table | Context.TableObject));
            Add(new Action((s, m) => true || s.Direct.OfType<Column>().Any(), 
                (s, m) => s.Table.AddHierarchy(displayFolder: s.CurrentFolder, levels: s.Direct.OfType<Column>().ToArray()).Expand().Edit(), 
                (s, m) => @"Create New\Hierarchy", true, Context.Table | Context.TableObject));
            Add(new Separator(@"Create New"));
            Add(new Action(
                (s, m) => (s.Context == Context.Partition || (s.Context == Context.Table && s.Count == 1)) && !Handler.UsePowerBIGovernance, 
                (s, m) => Partition.CreateNew(s.Context == Context.Partition ? s.Partitions.First().Table : s.Table).Edit(), 
                (s, m) => @"Create New\Partition", true, Context.Table | Context.Partition));

            Add(new Action((s, m) => !Handler.UsePowerBIGovernance, (s, m) => m.AddDataSource().Edit(), (s, m) => @"Create New\Data Source", false, Context.DataSources | Context.Model));
            Add(new Action((s, m) => Handler.CompatibilityLevel >= 1400 && !Handler.UsePowerBIGovernance, (s, m) => m.AddStructuredDataSource().Edit(), (s, m) => @"Create New\Structured Data Source", false, Context.DataSources | Context.Model));
            Add(new Action((s, m) => !Handler.UsePowerBIGovernance, (s, m) => m.AddPerspective().Edit(), (s, m) => @"Create New\Perspective", false, Context.Model | Context.Perspectives | Context.Perspective));
            Add(new Action((s, m) => !Handler.UsePowerBIGovernance, (s, m) => m.AddExpression().Edit(), (s, m) => @"Create New\Shared Expression", false, Context.Model | Context.Expressions | Context.Expression));
            Add(new Action((s, m) => m.Tables.Count(t => t.Columns.Any()) >= 2, (s, m) => m.AddRelationship().Edit(), (s, m) => @"Create New\Relationship", false, Context.Relationship | Context.Relationships | Context.Model));
            Add(new Action((s, m) => true, (s, m) => m.AddRole().Edit(), (s, m) => @"Create New\Role", false, Context.Model | Context.Roles | Context.Role));
            Add(new Action((s, m) => m.DataSources.Any() && !Handler.UsePowerBIGovernance, (s, m) => m.AddTable().Edit(), (s, m) => @"Create New\Table", false, Context.Tables | Context.Model));
            Add(new Action((s, m) => true, (s, m) => m.AddCalculatedTable().Edit(), (s, m) => @"Create New\Calculated Table", false, Context.Tables | Context.Model));
            Add(new Action((s, m) => !Handler.UsePowerBIGovernance, (s, m) => {
                var res = csDialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    m.AddTranslation(csDialog.SelectedCulture.Name).Edit();
                }
            }, (s, m) => @"Create New\Translation", true, Context.Model | Context.Translations | Context.Translation));

            Add(new CreateRelationshipAction(CreateRelationshipDirection.To));
            Add(new CreateRelationshipAction(CreateRelationshipDirection.From));

            // "Add to Hierarchy..."
            Add(new MultiAction((s, m, p) =>
                // Action enabled only when table contains at least one hierarchy:
                ((p == null) && s.Table.Hierarchies.Any()) ||
                // ...and none of the selected columns are already present as levels in the hierarchy:
                ((p != null) && !(p as Hierarchy).Levels.Select(l => l.Column).Intersect(s.Columns).Any()),

                (s, m, p) => (p as Hierarchy).AddLevels(s.Columns),
                (s, m, p) => (p as Hierarchy).Name,
                (s, m) => s.Table.Hierarchies.AsEnumerable(), "Add to Hierarchy...", true, Context.Column));

            Add(new Separator());

            // Visibility and perspectives
            Add(new Action((s, m) => s.OfType<IHideableObject>().Any(o => o.IsHidden), (s, m) => s.IsHidden = false, (s, m) => "Make visible", true, Context.TableObject | Context.Table));
            Add(new Action((s, m) => s.OfType<IHideableObject>().Any(o => !o.IsHidden), (s, m) => s.IsHidden = true, (s, m) => "Make invisible", true, Context.TableObject | Context.Table));
            Add(new Action((s, m) => s.OfType<ITabularPerspectiveObject>().Any() && !Handler.UsePowerBIGovernance, (s, m) => s.ShowInAllPerspectives(), (s, m) => @"Show in Perspectives\All Perspectives", true, Context.TableObject | Context.Table));
            Add(new Action((s, m) => s.OfType<ITabularPerspectiveObject>().Any() && !Handler.UsePowerBIGovernance, (s, m) => s.HideInAllPerspectives(), (s, m) => @"Hide in Perspectives\All Perspectives", true, Context.TableObject | Context.Table));

            Add(new Separator(@"Show in Perspectives"));
            Add(new Separator(@"Hide in Perspectives"));

            Add(new MultiAction((s, m, p) => s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || !obj.InPerspective[p as Perspective]) && !Handler.UsePowerBIGovernance,
                (s, m, p) => s.ShowInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Show in Perspectives", true, Context.TableObject | Context.Table));

            Add(new MultiAction((s, m, p) => s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || obj.InPerspective[p as Perspective]) && !Handler.UsePowerBIGovernance,
                (s, m, p) => s.HideInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Hide in Perspectives", true, Context.TableObject | Context.Table));

            // Rename Dialogs
            Add(new Separator());

            // "Duplicate Table Object";
            Add(new Action((s, m) => true,
                (s, m) => s.ForEach(i =>
                {
                    var obj = (i as IClonableObject).Clone(includeTranslations: i is ITranslatableObject);
                    if (s.Count == 1) obj.Edit(); // Focuses the cloned item in the tree, and lets the user edit its name
                }),
                (s, m) => "Duplicate " + s.Summary(), true, Context.TableObject | Context.Partition));

            // "Duplicate Table";
            Add(new Action((s, m) => s.Count == 1 && !Handler.UsePowerBIGovernance, (s, m) => s.Table.Clone().Edit(), (s, m) => "Duplicate Table", true, Context.Table));

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

            // Batch Rename
            Add(new Action((s, m) => s.Count > 1, (s, m) =>
            {
                var form = Dialogs.ReplaceForm.Singleton;
                form.Text = "Batch Rename - (" + s.Summary() + " selected)";
                var res = form.ShowDialog();
                if (res == DialogResult.Cancel) return;
                // TODO: Add options for match case and whole word only
                s.Rename(form.Pattern, form.ReplaceWith, form.RegEx, form.IncludeTranslations);
            }, (s, m) => "Batch Rename...", true, Context.Table | Context.TableObject | Context.Level) { ToolTip = "Opens a dialog that lets you rename all the selected objects at once. Folders are not renamed, but objects inside folders are."});

            // Batch Rename Children
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
            Delete = new Action((s, m) => s.Count >= 1, 
                (s, m) => {
                    if (s.Count == 1)
                    {
                        // Handle single-object deletion:
                        string message;
                        if (!s.First().CanDelete(out message))
                        {
                            MessageBox.Show(message, s.Summary() + " cannot be deleted.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        else if (!string.IsNullOrEmpty(message))
                        {
                            var mr = MessageBox.Show(message, "Confirm deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (mr == DialogResult.Cancel) return;
                        }
                    }
                    else
                    {
                        // Handle multi-object deletion:
                        string message;
                        if(s.Any(o => !o.CanDelete(out message)))
                        {
                            var mr = MessageBox.Show("One or more of the selected objects cannot be deleted. Proceed?", "Unable to delete one or more objects.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (mr == DialogResult.Cancel) return;
                        } else
                        {
                            // Confirm deletion of multiple objects just in case...
                            var mr = MessageBox.Show("Are you sure you want to delete the selected objects?", s.Summary(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (mr == DialogResult.Cancel) return;
                        }
                    }

                    s.Delete();
                }, (s, m) => "Delete " + s.Summary() + "...", true, Context.SingularObjects ^ Context.Model);
            Add(Delete);

            Add(new Separator());
            Add(new Action((s, m) => m.Cultures.Count > 0, (s, m) => UIController.Current.Translations_ExportAll(), (s, m) => "Export translations...", false, Context.Translations | Context.Tool));
            Add(new Action((s, m) => true, (s, m) => UIController.Current.Translations_Import(), (s, m) => "Import translations...", true, Context.Translations | Context.Tool));
            Add(new Action((s, m) => true, (s, m) => UIController.Current.Translations_ExportSelected(), (s, m) => string.Format("Export {0} translation{1}...", s.Count, s.Count == 1 ? "" : "s"), true, Context.Translation));

            // Show dependencies...
            Add(new Action((s, m) => s.DirectCount == 1 && s.Direct.First() is IDaxObject, (s, m) =>
            {
                UIController.Current.ShowDependencies(s.Direct.First() as IDaxObject);
            }, (s, m) => @"Show dependencies...", true, Context.Table | Context.TableObject));

            // Reverse relationship:
            Add(new Action((s, m) => true, (s, m) => s.SingleColumnRelationships.ForEach(r => {
                var fc = r.FromColumn; r.FromColumn = null;
                var tc = r.ToColumn; r.ToColumn = null;
                r.FromColumn = tc;
                r.ToColumn = fc;
            }), (s, m) => "Reverse direction", false, Context.Relationship));

            // Script actions:
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => Clipboard.SetText(Scripter.ScriptCreateOrReplace(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Create or Replace\To clipboard", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => Clipboard.SetText(Scripter.ScriptCreate(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Create\To clipboard", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => Clipboard.SetText(Scripter.ScriptAlter(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Alter\To clipboard", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => Clipboard.SetText(Scripter.ScriptDelete(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Delete\To clipboard", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => SaveScriptToFile(Scripter.ScriptCreateOrReplace(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Create or Replace\To file...", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => SaveScriptToFile(Scripter.ScriptCreate(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Create\To file...", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => SaveScriptToFile(Scripter.ScriptAlter(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Alter\To file...", true, Context.Scriptable));
            Add(new Action((s, m) => s.DirectCount == 1, (s, m) => SaveScriptToFile(Scripter.ScriptDelete(s.OfType<TabularNamedObject>().FirstOrDefault())), (s, m) => @"Script\Delete\To file...", true, Context.Scriptable));

            Add(new Action((s, m) => s.DirectCount > 1, (s, m) => Clipboard.SetText(Scripter.ScriptMergePartitions(s.OfType<Partition>().ToList())), (s, m) => @"Script\Merge Partitions\To clipboard", true, Context.Partition));
            Add(new Action((s, m) => s.DirectCount > 1, (s, m) => SaveScriptToFile(Scripter.ScriptMergePartitions(s.OfType<Partition>().ToList())), (s, m) => @"Script\Merge Partitions\To file...", true, Context.Partition));

            Add(new RefreshAction(true));
            Add(new RefreshAction(false));

        }

        public static void SaveScriptToFile(string script)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "Tabular Model Scripting Language|*.tmsl|All files|*.*",
                DefaultExt = "*.tmsl",
                FileName = "script.tmsl",
                Title = "Save script"
            };
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(sfd.FileName, script);
            }
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
