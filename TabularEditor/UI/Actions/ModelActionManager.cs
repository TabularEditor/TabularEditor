using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.Scripting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.UI.Actions
{
    public class ModelActionManager : List<IBaseAction>
    {
        public bool HandleKeyPress(Keys cmdKeys)
        {
            Action act;
            if(ShortcutActions.TryGetValue(cmdKeys, out act))
            {
                if (!act.Enabled(null)) return false;
                act.Execute(null);
                return true;
            }
            return false;
        }

        // TODO: Refactor how adding actions with a shortcut works, to avoid singleton references and so on...
        public Dictionary<Keys, Action> ShortcutActions = new Dictionary<Keys, Action>();

        public void RemoveMacros()
        {
            for(var i = this.Count - 1; i >= 0; i--)
            {
                if (this[i] is MacroAction) this.RemoveAt(i);
            }
        }

        /// <summary>
        /// When an Action is executed, it will set this field.
        /// </summary>
        public Action LastActionExecuted = null;

        public List<ITabularNamedObject> SelectObjects = new List<ITabularNamedObject>();

        public Action Delete { get; private set; }

        public TabularModelHandler Handler { get { return UI.UIController.Current.Handler; } }

        private PowerBIGovernance Governance => Handler.PowerBIGovernance;

        public void CreateStandardActions()
        {
            var csDialog = new CultureSelectDialog();

            // Import Table Wizard...:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Table)), (s, m) => ImportTablesWizard.ShowWizard(m), (s, m) => "Import Tables...", true, Context.Model | Context.Tables));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && m.DataSources.Any(ds => ds.Type == DataSourceType.Provider), (s, m) => ScriptHelper.SchemaCheck(m), (s, m) => "Refresh Table Metadata...", true, Context.Model | Context.Tables));
            // Import Table Wizard...:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && s.DirectCount == 1 && s.DataSource is ProviderDataSource, (s, m) => ImportTablesWizard.ShowWizard(m, s.DataSource as ProviderDataSource), (s, m) => "Import Tables...", true, Context.DataSource));

            // Apply Refresh Policy
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Partition)) && s.DirectCount == 1 && s.Table.EnableRefreshPolicy, (s, m) => s.Table.ApplyRefreshPolicy(), (s, m) => "Apply Refresh Policy", true, Context.Table));

            // Schema check:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && s.DirectCount == 1 && m.DataSources.Any(ds => ds.Type == DataSourceType.Provider) && s.DataSource is ProviderDataSource, (s, m) => ScriptHelper.SchemaCheck(s.DataSource as ProviderDataSource), (s, m) => "Refresh Table Metadata...", true, Context.DataSource));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && s.DirectCount == 1 && m.DataSources.Any(ds => ds.Type == DataSourceType.Provider) && s.Partition.DataSource is ProviderDataSource, (s, m) => ScriptHelper.SchemaCheck(s.Partition), (s, m) => "Refresh Table Metadata...", true, Context.Partition));

            Add(new Separator());

            // "Create New Display Folder"
            Add(new Action((s, m) => Governance.AllowEditProperty(s, TOMWrapper.Properties.DISPLAYFOLDER) && s.Count >= 1, 
                (s, m) => {
                    var orgDF = (s.Direct.FirstOrDefault() as IFolderObject)?.GetDisplayFolder(Handler.Tree.Culture);
                    var newDF = string.IsNullOrWhiteSpace(orgDF) ? "New folder" : (orgDF + @"\New folder"); ;
                    Folder.CreateFolder(s.Table, newDF).Edit().Expand();
                    s.ReplaceFolder(orgDF, newDF, Handler.Tree.Culture);
                }, 
                (s, m) => @"Create New\Display Folder", true, Context.TableObject));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculationItem)) && s.Count == 1, (s, m) => s.CalculationGroup.AddCalculationItem().Edit(), (s, m) => @"Create New\Calculation Item", false, Context.CalculationGroupTable));

            Add(new Separator(@"Create New"));

            // Add measure:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Measure)) && (s.Count == 1 || s.Context.HasX(Context.TableObject)), (s, m) => s.Table.AddMeasure(displayFolder: s.CurrentFolder).Vis().Edit(), (s, m) => @"Create New\Measure", true, Context.CalculationGroupTable | Context.Table | Context.TableObject, Keys.Alt | Keys.D1));

            // Add calc column:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculatedColumn)) && (s.Count == 1 || s.Context.HasX(Context.TableObject)), (s, m) => s.Table.AddCalculatedColumn(displayFolder: s.CurrentFolder).Vis().Edit(), (s, m) => @"Create New\Calculated Column", true, Context.CalculationGroupTable | Context.Table | Context.TableObject, Keys.Alt | Keys.D2));

            // Add calc table column:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculatedTableColumn))
                && Handler.SourceType != ModelSourceType.Database
                && (s.Count == 1 || s.Context.HasX(Context.TableObject))
                && (s.Table is CalculatedTable),
                (s, m) => (s.Table as CalculatedTable).AddCalculatedTableColumn(displayFolder: s.CurrentFolder).Vis().Edit(), (s, m) => @"Create New\Calculated Table Column", true, Context.Table | Context.TableObject));

            // Add hierarchy:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Hierarchy)) && (s.Count == 1 || s.Context.HasX(Context.TableObject)), 
                (s, m) => s.Table.AddHierarchy(displayFolder: s.CurrentFolder, levels: s.Direct.OfType<Column>().ToArray()).Expand().Vis().Edit(), 
                (s, m) => @"Create New\Hierarchy", true, Context.CalculationGroupTable | Context.Table | Context.TableObject, Keys.Alt | Keys.D3));

            // Add data column:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && (s.Count == 1 || s.Context.HasX(Context.TableObject)) && !(s.Table is CalculatedTable), (s, m) => s.Table.AddDataColumn(displayFolder: s.CurrentFolder).Vis().Edit(), (s, m) => @"Create New\Data Column", true, Context.CalculationGroupTable | Context.Table | Context.TableObject, Keys.Alt | Keys.D4));

            // Add calendar:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Calendar)) && (s.Count == 1 || s.Context.HasX(Context.TableObject)), (s, m) => s.Table.AddCalendar().Edit(), (s, m) => @"Create New\Calendar", true, Context.Calendar | Context.CalendarCollection | Context.CalculationGroupTable | Context.Table | Context.TableObject, Keys.Alt | Keys.D7));

            // Add KPI:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(KPI)) && s.Count == 1 && !s.Folders.Any(), (s, m) => s.Measure.AddKPI().Edit(), (s, m) => @"Create New\KPI", true, Context.Measure));

            Add(new Separator(@"Create New"));

            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(Partition)) && s.Count == 1 && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(Partition)), 
                (s, m) => Partition.CreateNew(s.Table).Edit(), 
                (s, m) => @"Create New\Partition" + (Handler.CompatibilityLevel >= 1400 ? " (Legacy)" : ""), true, Context.Table));
            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(MPartition)) && s.Count == 1 && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(MPartition)),
                (s, m) => MPartition.CreateNew(s.Table).Edit(),
                (s, m) => @"Create New\Partition (Power Query)", true, Context.Table));
            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(PolicyRangePartition)) && s.Count == 1 && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(PolicyRangePartition)),
                (s, m) => MPartition.CreateNew(s.Table).Edit(),
                (s, m) => @"Create New\Partition (Policy Range)", true, Context.Table));
            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(EntityPartition)) && s.Count == 1 && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(EntityPartition)),
                (s, m) => MPartition.CreateNew(s.Table).Edit(),
                (s, m) => @"Create New\Partition (DQ over AS)", true, Context.Table));

            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(Partition)) && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(Partition)),
                (s, m) => Partition.CreateNew(s.Table).Edit(),
                (s, m) => @"New Partition" + (Handler.CompatibilityLevel >= 1400 ? " (Legacy)" : ""), true, Context.PartitionCollection | Context.Partition));
            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(MPartition)) && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(MPartition)),
                (s, m) => MPartition.CreateNew(s.Table).Edit(),
                (s, m) => @"New Partition (Power Query)", true, Context.PartitionCollection | Context.Partition));
            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(PolicyRangePartition)) && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(PolicyRangePartition)),
                (s, m) => MPartition.CreateNew(s.Table).Edit(),
                (s, m) => @"New Partition (Policy Range)", true, Context.PartitionCollection | Context.Partition));
            Add(new Action(
                (s, m) => Governance.AllowCreate(typeof(EntityPartition)) && s.Table.Partitions.GetSupportedPartitionTypes().Contains(typeof(EntityPartition)),
                (s, m) => MPartition.CreateNew(s.Table).Edit(),
                (s, m) => @"New Partition (DQ over AS)", true, Context.PartitionCollection | Context.Partition));

            Add(new Action((s, m) => Governance.AllowCreate(typeof(Table)) && (m.DataSources.Any() || Handler.CompatibilityLevel >= 1400), (s, m) => m.AddTable().Vis().Edit(), (s, m) => @"Create New\Table", false, Context.Tables | Context.Model, Keys.Alt | Keys.D5));

            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculatedTable)), (s, m) => m.AddCalculatedTable().Vis().Edit(), (s, m) => @"Create New\Calculated Table", false, Context.Tables | Context.Model, Keys.Alt | Keys.D6));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculationGroupTable)) && Handler.CompatibilityLevel >= 1470, (s, m) => m.AddCalculationGroup().Vis().Edit(), (s, m) => @"Create New\Calculation Group", false, Context.Tables | Context.Model, Keys.Alt | Keys.D8));

            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculationItem)) && s.Count == 1, (s, m) => s.CalculationGroup.AddCalculationItem().Edit(), (s, m) => @"New Calculation Item", false, Context.CalculationItemCollection));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculationItem)), (s, m) => s.CalculationItems.First().CalculationGroupTable.AddCalculationItem().Edit(), (s, m) => @"New Calculation Item", false, Context.CalculationItem));

            Add(new Action((s, m) => Governance.AllowCreate(typeof(ProviderDataSource)), (s, m) => m.AddDataSource().Edit(), (s, m) => @"Create New\Data Source (Legacy)", false, Context.DataSources | Context.Model));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(StructuredDataSource)) && Handler.CompatibilityLevel >= 1400, (s, m) => m.AddStructuredDataSource().Edit(), (s, m) => @"Create New\Data Source (Power Query)", false, Context.DataSources | Context.Model));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Perspective)), (s, m) => m.AddPerspective().Edit(), (s, m) => @"Create New\Perspective", false, Context.Model | Context.Perspectives | Context.Perspective));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Function)), (s, m) => m.AddFunction().Edit(), (s, m) => @"Create New\User-Defined Function", false, Context.Model | Context.Functions | Context.Function));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(NamedExpression)), (s, m) => m.AddExpression().Edit(), (s, m) => @"Create New\Shared Expression", false, Context.Model | Context.Expressions | Context.Expression));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(SingleColumnRelationship)) && m.Tables.Count(t => t.Columns.Any()) >= 2, (s, m) => m.AddRelationship().Edit(), (s, m) => @"Create New\Relationship", false, Context.Relationship | Context.Relationships | Context.Model));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(ModelRole)), (s, m) => m.AddRole().Edit(), (s, m) => @"Create New\Role", false, Context.Model | Context.Roles | Context.Role));


            Add(new Action((s, m) => Governance.AllowCreate(typeof(Culture)), (s, m) => {
                var res = csDialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    m.AddTranslation(csDialog.SelectedCulture.Name).Edit();
                }
            }, (s, m) => @"Create New\Translation", true, Context.Model | Context.Translations | Context.Translation));

            Add(new CreateRelationshipAction(CreateRelationshipDirection.To));
            Add(new CreateRelationshipAction(CreateRelationshipDirection.From));

            // "Add to Hierarchy..."
            Add(new MultiAction((s, m, p) => Governance.AllowEditProperty(ObjectType.Hierarchy, TOMWrapper.Properties.LEVELS) && (
                // Action enabled only when table contains at least one hierarchy:
                ((p == null) && s.Table.Hierarchies.Any()) ||
                // ...and none of the selected columns are already present as levels in the hierarchy:
                ((p != null) && !(p as Hierarchy).Levels.Select(l => l.Column).Intersect(s.Columns).Any())),

                (s, m, p) => (p as Hierarchy).AddLevels(s.Columns),
                (s, m, p) => (p as Hierarchy).Name,
                (s, m) => s.Table.Hierarchies.AsEnumerable(), "Add to Hierarchy...", true, Context.Column));

            Add(new Separator());

            // Relationship actions:
            Add(new Action((s, m) => Governance.AllowEditProperty(ObjectType.Relationship, TOMWrapper.Properties.ISACTIVE) && s.SingleColumnRelationships.Any(o => !o.IsActive), (s, m) => s.SingleColumnRelationships.IsActive = true, (s, m) => "Activate", true, Context.Relationship));
            Add(new Action((s, m) => Governance.AllowEditProperty(ObjectType.Relationship, TOMWrapper.Properties.ISACTIVE) && s.SingleColumnRelationships.Any(o => o.IsActive), (s, m) => s.SingleColumnRelationships.IsActive = false, (s, m) => "Deactivate", true, Context.Relationship));
            // Reverse relationship:
            Add(new Action((s, m) => Governance.AllowEditProperty(ObjectType.Relationship, TOMWrapper.Properties.FROMCOLUMN), (s, m) => s.SingleColumnRelationships.ForEach(r => {
                var fc = r.FromColumn; r.FromColumn = null;
                var tc = r.ToColumn; r.ToColumn = null;
                r.FromColumn = tc;
                r.ToColumn = fc;
            }), (s, m) => "Reverse direction", false, Context.Relationship));

            // Visibility and perspectives
            Add(new Action((s, m) => Governance.AllowEditProperty(s, TOMWrapper.Properties.ISHIDDEN) && s.OfType<IHideableObject>().Any(o => o.IsHidden), (s, m) => s.IsHidden = false, (s, m) => "Make visible", true, Context.DataObjects | Context.Function, Keys.Control | Keys.U));
            Add(new Action((s, m) => Governance.AllowEditProperty(s, TOMWrapper.Properties.ISHIDDEN) && s.OfType<IHideableObject>().Any(o => !o.IsHidden), (s, m) => s.IsHidden = true, (s, m) => "Make invisible", true, Context.DataObjects | Context.Function, Keys.Control | Keys.I));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Perspective)) && s.OfType<ITabularPerspectiveObject>().Any(), (s, m) => s.ShowInAllPerspectives(), (s, m) => @"Show in Perspectives\All Perspectives", true, Context.DataObjects));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Perspective)) && s.OfType<ITabularPerspectiveObject>().Any(), (s, m) => s.HideInAllPerspectives(), (s, m) => @"Hide in Perspectives\All Perspectives", true, Context.DataObjects));

            Add(new Separator(@"Show in Perspectives"));
            Add(new Separator(@"Hide in Perspectives"));

            Add(new MultiAction((s, m, p) => Governance.AllowCreate(typeof(Perspective)) && s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || !obj.InPerspective[p as Perspective]),
                (s, m, p) => s.ShowInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Show in Perspectives", true, Context.DataObjects));

            Add(new MultiAction((s, m, p) => Governance.AllowCreate(typeof(Perspective)) && s.OfType<ITabularPerspectiveObject>().Any(obj => p == null || obj.InPerspective[p as Perspective]),
                (s, m, p) => s.HideInPerspective(p as Perspective),
                (s, m, p) => (p as Perspective).Name,
                (s, m) => m.Perspectives, "Hide in Perspectives", true, Context.DataObjects));

            // Rename Dialogs
            Add(new Separator());

            // "Duplicate Table Object";
            Add(new Action((s, m) => Governance.AllowCreate(s),
                (s, m) => s.ForEach(i =>
                {
                    var obj = (i as IClonableObject).Clone(includeTranslations: i is ITranslatableObject && Preferences.Current.Copy_IncludeTranslations);
                    if (s.Count == 1) obj.Edit(); // Focuses the cloned item in the tree, and lets the user edit its name
                }),
                (s, m) => "Duplicate " + s.Summary(), true, Context.TableObject | Context.Calendar | Context.Partition | Context.CalculationItem));

            // "Duplicate Table";
            Add(new Action((s, m) => Governance.AllowCreate(s) && s.Count == 1, (s, m) => s.Table.Clone().Edit(), (s, m) => "Duplicate Table", true, Context.Table));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(CalculationGroupTable)) && s.Count == 1, (s, m) => s.CalculationGroup.Clone().Edit(), (s, m) => "Duplicate Calculation Group", true, Context.CalculationGroupTable));

            // "Duplicate Translation";
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Culture)) && s.Count == 1,
                (s, m) => s.ForEach(i =>
                {
                    var res = csDialog.ShowDialog();
                    if (res == DialogResult.OK) (i as IClonableObject).Clone(csDialog.SelectedCulture.Name, false).Edit();
                }),
                (s, m) => "Duplicate " + s.Summary(), true, Context.Translation));

            // "Duplicate Role / Perspective / Function":
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Perspective)) && s.Count == 1, (s, m) => s.ForEach(i => (i as IClonableObject).Clone(null, Preferences.Current.Copy_IncludeTranslations).Edit()), (s, m) => "Duplicate Perspective", true, Context.Perspective));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(ModelRole)) && s.Count == 1, (s, m) => s.ForEach(i => (i as IClonableObject).Clone(null, Preferences.Current.Copy_IncludeTranslations).Edit()), (s, m) => "Duplicate Role", true, Context.Role));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Function)) && s.Count == 1, (s, m) => s.ForEach(i => (i as IClonableObject).Clone(null, Preferences.Current.Copy_IncludeTranslations).Edit()), (s, m) => "Duplicate Function", true, Context.Function));

            // Batch Rename
            Add(new Action((s, m) => Governance.AllowEditProperty(s.Direct, TOMWrapper.Properties.NAME) && s.DirectCount > 1, (s, m) =>
            {
                var form = Dialogs.ReplaceForm.Singleton;
                form.Text = "Batch Rename - (" + s.Direct.Summary() + " selected)";
                var res = form.ShowDialog();
                if (res == DialogResult.Cancel) return;
                // TODO: Add options for match case and whole word only
                s.Direct.Rename(form.Pattern, form.ReplaceWith, form.RegEx, form.IncludeTranslations);
            }, (s, m) => "Batch Rename...", true, Context.Function | Context.DataObjects | Context.Level | Context.CalculationItem | Context.Partition, Keys.F2) { ToolTip = "Opens a dialog that lets you rename all the selected objects at once. Folders are not renamed, but objects inside folders are."});

            // Batch Rename Children
            Add(new Action((s, m) => CanBatchRenameChildren(s, m), (s, m) =>
            {
                var form = Dialogs.ReplaceForm.Singleton;
                var sel = new UISelectionList<ITabularNamedObject>(
                    s.Direct.OfType<ITabularObjectContainer>().SelectMany(t => t.GetChildren())
                    .Concat(s.Hierarchies.SelectMany(t => t.Levels))
                    .OfType<ITabularNamedObject>());
                form.Text = "Batch Rename - (" + sel.Summary() + ")";
                var res = form.ShowDialog();
                if (res == DialogResult.Cancel) return;
                // TODO: Add options for match case and whole word only
                sel.Rename(form.Pattern, form.ReplaceWith, form.RegEx, form.IncludeTranslations);
            }, (s, m) => "Batch Rename Children...", true, Context.DataObjects | Context.PartitionCollection, Keys.Shift | Keys.F2)
            { ToolTip = "Opens a dialog that lets you rename all children of the selected objects at once. Folders are not renamed, but objects inside folders are." });

            // Import TMDL
            Add(new Action((s, m) => true, ImportTmdl, (s, m) => "Import TMDL...", true, Context.Model | Context.Table | Context.Functions));

            // Delete Action
            Delete = new Action((s, m) => Governance.AllowDelete(s) && s.Count >= 1, 
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

            // Select Columns (aka. Import Table Wizard) and Refresh Table Metadata:
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && s.DirectCount == 1 && s.Table.Partitions[0].DataSource is ProviderDataSource, (s, m) => ImportTablesWizard.ShowWizard(s.Table), (s, m) => "Select Columns...", true, Context.Table));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(DataColumn)) && s.DirectCount == 1 && m.DataSources.Any(ds => ds.Type == DataSourceType.Provider) && s.Table.Partitions.Count > 0 && s.Table.Partitions[0].DataSource is ProviderDataSource, (s, m) => ScriptHelper.SchemaCheck(s.Table), (s, m) => "Refresh Table Metadata...", true, Context.Table));

            Add(new Separator());
            Add(new Action((s, m) => m.Cultures.Count > 0, (s, m) => UIController.Current.Translations_ExportAll(), (s, m) => "Export translations...", false, Context.Translations | Context.Tool));
            Add(new Action((s, m) => Governance.AllowCreate(typeof(Culture)) && true, (s, m) => UIController.Current.Translations_Import(), (s, m) => "Import translations...", true, Context.Translations | Context.Tool));
            Add(new Action((s, m) => true, (s, m) => UIController.Current.Translations_ExportSelected(), (s, m) => string.Format("Export {0} translation{1}...", s.Count, s.Count == 1 ? "" : "s"), true, Context.Translation));

            // Table actions:
            Add(new Action((s, m) => Governance.AllowEditProperty(ObjectType.Column, TOMWrapper.Properties.ISKEY) && Governance.AllowEditProperty(ObjectType.Column, TOMWrapper.Properties.DATACATEGORY) && s.DirectCount == 1 && s.Table.Columns.Any(c => c.DataType == DataType.DateTime), (s, m) => UIController.Current.MarkAsDateTableDialog.Go(s.Table), (s, m) => "Mark as Date Table...", true, Context.Table));

            // Show dependencies...
            Add(new Action((s, m) => s.DirectCount == 1 && s.Direct.First() is IDaxObject, (s, m) =>
            {
                UIController.Current.ShowDependencies(s.Direct.First() as IDaxObject);
            }, (s, m) => @"Show &dependencies...", true, Context.Calendar | Context.Table | Context.TableObject | Context.CalculationItem | Context.Function, Keys.F3));

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

        private bool CanBatchRenameChildren(UITreeSelection s, Model m)
        {
            return Governance.AllowEditProperty(s.Concat(s.Tables.SelectMany(t => t.GetChildren())), TOMWrapper.Properties.NAME)
                && (s.Context == Context.Table || s.Direct.Any(i => i is Folder) || s.Context == Context.PartitionCollection);
        }

        private void ImportTmdl(UITreeSelection s, Model m)
        {
            var tmdlForm = new TmdlInputForm();
            var mr = tmdlForm.ShowDialog();
            if(mr == DialogResult.Cancel || string.IsNullOrEmpty(tmdlForm.Tmdl)) return;

            m.ImportTmdl(tmdlForm.Tmdl, s.FirstOrDefault() ?? m);
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

        /// <summary>
        /// This method flags to the action, that the TreeView should select the object
        /// immediately after it has been created. Useful for actions that add new objects
        /// to the model tree, when you want to visually highlight the newly created object(s)
        /// to the user.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Select<T>(this T obj) where T: ITabularNamedObject
        {
            UIController.Current.Actions.SelectObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Makes the object visible in the current perspective
        /// </summary>
        public static T Vis<T>(this T obj) where T: ITabularPerspectiveObject
        {
            var currentPerspective = UI.UIController.Current.TreeModel.Perspective;
            if (currentPerspective != null) obj.InPerspective[currentPerspective] = true;
            return obj;
        }
    }
}
