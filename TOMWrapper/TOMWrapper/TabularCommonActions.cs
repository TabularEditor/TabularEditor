using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Provides convenient methods for common actions on a Tabular Model, that often involve changing multiple objects at once.
    /// For example, these methods may be used to easily perform UI drag and drop operations that will change hierarchy levels,
    /// display folders, etc.
    /// </summary>
    public sealed class TabularCommonActions
    {
        public TabularModelHandler Handler { get; private set; }
        internal TabularCommonActions(TabularModelHandler handler)
        {
            Handler = handler;
        }
        public void ReorderLevels(IEnumerable<Level> levels, int firstOrdinal)
        {
            if (levels.Count() == 0) return;
            var hier = levels.First().Hierarchy;
            if (hier == null || levels.Select(l => l.Hierarchy).Distinct().Count() != 1) throw new ArgumentException("When reordering levels, all levels must belong to the same hierarchy.");

            if (firstOrdinal < 0) firstOrdinal = 0;
            if (firstOrdinal > hier.Levels.Count) firstOrdinal = hier.Levels.Count;

            Handler.BeginUpdate("level reorder");
            hier.Reordering = true;

            var aboveMoved = hier.Levels.OrderBy(l => l.Ordinal).Take(firstOrdinal).Except(levels);
            var others = hier.Levels.OrderBy(l => l.Ordinal).Skip(firstOrdinal).Except(levels);

            var list = aboveMoved.Concat(levels).Concat(others).ToList();

            hier.SetLevelOrder(list);

            hier.Reordering = false;
            Handler.EndUpdate();
        }

        public void ReorderCalculationItems(IEnumerable<CalculationItem> items, int firstOrdinal)
        {
            if (items.Count() == 0) return;
            var calcGroup = items.First().CalculationGroupTable;
            if (calcGroup == null || items.Select(l => l.CalculationGroupTable).Distinct().Count() != 1) throw new ArgumentException("When reordering calculation items, all items must belong to the same calculation group.");

            if (firstOrdinal < 0) firstOrdinal = 0;
            if (firstOrdinal > calcGroup.CalculationItems.Count) firstOrdinal = calcGroup.CalculationItems.Count;

            Handler.BeginUpdate("level reorder");
            calcGroup.Reordering = true;

            var aboveMoved = calcGroup.CalculationItems.OrderBy(l => l.Ordinal).Take(firstOrdinal).Except(items);
            var others = calcGroup.CalculationItems.OrderBy(l => l.Ordinal).Skip(firstOrdinal).Except(items);

            var list = aboveMoved.Concat(items).Concat(others).ToList();

            calcGroup.SetLevelOrder(list);

            calcGroup.Reordering = false;
            Handler.EndUpdate();
        }

        public void AddColumnsToHierarchy(IEnumerable<Column> columns, Hierarchy hierarchy, int firstOrdinal = -1)
        {
            hierarchy.AddLevels(columns, firstOrdinal);
        }

        public void AddCalcItemsToCalcGroup(IEnumerable<CalculationItem> items, CalculationGroupTable destination, int firstOrdinal = -1)
        {

        }

        public Level AddColumnToHierarchy(Column column, Hierarchy hierarchy, int ordinal = -1)
        {
            return hierarchy.AddLevel(column, ordinal: ordinal);
        }

        public void SetContainer(IEnumerable<IFolderObject> objects, IFolder newContainer, Culture culture)
        {
            if (objects == null) throw new ArgumentNullException("objects");
            if (newContainer == null) throw new ArgumentNullException("newContainer");
            if (objects.Count() == 0) return;
            if (objects.Any(o => o.Table != newContainer.ParentTable)) throw new ArgumentException("Cannot move objects between tables. To move a measure from one table to another, use method \"MoveObjects\".");

            Handler.BeginUpdate("folder structure change");

            foreach (var obj in objects)
            {
                obj.SetDisplayFolder((newContainer as Folder)?.Path ?? "", culture);
            }

            Handler.EndUpdate();
        }

        public string NewMeasureName(string prefix, Table table)
        {
            return table.Measures.GetNewName(prefix);
        }
        
        public string NewColumnName(string prefix, Table table)
        {
            return table.Columns.GetNewName(prefix);
        }

        /// <summary>
        /// Inserts the specified list of objects into the model, at the optional destination. Objects that cannot
        /// be meaningfully inserted in the destination, will be inserted at the destination parent (recursively).
        /// If no suitable destination can be found, insertion will be ignored.
        /// Useful for drag-and-drop or copy-paste operations.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="destination"></param>
        public List<TabularObject> InsertObjects(ObjectJsonContainer objectContainer, ITabularNamedObject destination = null)
        {
            // Possible destinations:
            var destHier = (destination as Level)?.Hierarchy ?? (destination as Hierarchy);
            var destTable = (destination as Folder)?.Table ?? (destination as Partition)?.Table ?? destHier?.Table ?? (destination as IFolderObject)?.Table ?? (destination as Table);
            var folder = (destination as Folder)?.Path;

            bool replaceTable = false;
            string replaceTableName = "";
            JObject replaceTableJobj = null;
            bool replaceTableIsCalculated = false;

            // If the object container only holds a single object, and that object is a table, let's ask the user if they want to
            // replace the destination table with the table in the clipboard (unless of course it's the same table).
            if (destTable != null && objectContainer.Count == 1 && (objectContainer.Get<CalculatedTable>().Count() == 1 || objectContainer.Get<Table>().Count() == 1))
            {
                replaceTableIsCalculated = objectContainer.Get<CalculatedTable>().Any();
                replaceTableJobj = objectContainer.Get<CalculatedTable>().FirstOrDefault() ?? objectContainer.Get<Table>().FirstOrDefault();
                replaceTableName = replaceTableJobj["name"].ToString();

                // Check that the object came from a different instance, or that its another table:
                if (objectContainer.InstanceID != Handler.InstanceID || !destTable.Name.StartsWith(replaceTableName))
                {
                    var result = MessageBox.Show($"Do you want to replace table '{destTable.Name}' with table '{replaceTableName}' from the clipboard?\n\nExisting relationships will be kept, provided participating columns have the same name and data types in both the replaced and the inserted table.", "Replace existing table?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Cancel) return new List<TabularObject>();
                    if (result == DialogResult.Yes) replaceTable = true;
                }
            }

            Handler.BeginUpdate("Paste objects");

            var inserted = new List<TabularObject>();

            if (destHier != null)
            {
                // Levels can only be deserialized on a Hierarchy destination:
                foreach (var obj in objectContainer.Get<Level>()) inserted.Add(Serializer.DeserializeLevel(obj, destHier));
                destHier.CompactLevelOrdinals();
            }

            if (destTable?.GetType() == typeof(Table))
            {
                // DataColumns and Partitions can only be deserialized onto a Table destination (not CalculatedTable):
                foreach (var obj in objectContainer.Get<DataColumn>().OrderBy(obj => obj["sortByColumn"] == null ? 0 : 1)) inserted.Add(Serializer.DeserializeDataColumn(obj, destTable));
                foreach (var obj in objectContainer.Get<Partition>()) inserted.Add(Serializer.DeserializePartition(obj, destTable));
                foreach (var obj in objectContainer.Get<MPartition>()) inserted.Add(Serializer.DeserializeMPartition(obj, destTable));
                foreach (var obj in objectContainer.Get<PolicyRangePartition>()) inserted.Add(Serializer.DeserializePolicyRangePartition(obj, destTable));
                foreach (var obj in objectContainer.Get<EntityPartition>()) inserted.Add(Serializer.DeserializeEntityPartition(obj, destTable));
            }

            if (destTable is Table)
            {
                // Measures, Hierarchies and CalculatedColumns can be deserialized onto a Table (or Table derived) destination:
                foreach (var obj in objectContainer.Get<CalculatedColumn>().OrderBy(obj => obj["sortByColumn"] == null ? 0 : 1)) inserted.Add(Serializer.DeserializeCalculatedColumn(obj, destTable));
                foreach (var obj in objectContainer.Get<Hierarchy>()) inserted.Add(Serializer.DeserializeHierarchy(obj, destTable));
                foreach (var obj in objectContainer.Get<Measure>()) inserted.Add(Serializer.DeserializeMeasure(obj, destTable));
            }

            // Replace an existing table with the one from the clipboard:
            if (replaceTable)
            {
                if (destTable.Name == replaceTableName)
                {
                    // Similarly named tables - disable formula fixup:
                    var fixupSetting = Handler.Settings.AutoFixup;
                    Handler.Settings.AutoFixup = false;
                    destTable.Name = destTable.Name + '-' + Guid.NewGuid().ToString();
                    Handler.Settings.AutoFixup = true;
                }
                else
                {
                    // Differently named tables:
                    // First, let's rename the destTable to match the new table (fix-up will handle DAX references):
                    if (destTable.Name != replaceTableName) destTable.Name = replaceTableName;

                    // Secondly, let's rename the table to be inserted (to avoid naming conflicts):
                    replaceTableJobj["name"] = replaceTableName + '-' + Guid.NewGuid().ToString();
                }

                // Insert the table:
                var newTable = replaceTableIsCalculated ?
                    Serializer.DeserializeCalculatedTable(replaceTableJobj, Handler.Model) :
                    Serializer.DeserializeTable(replaceTableJobj, Handler.Model);
                inserted.Add(newTable);

                // Update relationships to point to the inserted table:
                foreach(var rel in destTable.UsedInRelationships.ToList())
                {
                    if (rel.FromTable == destTable && newTable.Columns.Contains(rel.FromColumn.Name) && rel.FromColumn.DataType == newTable.Columns[rel.FromColumn.Name].DataType) { rel.FromColumn = newTable.Columns[rel.FromColumn.Name]; }
                    if (rel.ToTable == destTable && newTable.Columns.Contains(rel.ToColumn.Name) && rel.ToColumn.DataType == newTable.Columns[rel.ToColumn.Name].DataType) { rel.ToColumn = newTable.Columns[rel.ToColumn.Name]; }
                }

                // Delete original table:
                destTable.Delete();

                // Rename inserted table:
                newTable.Name = replaceTableName;
            }
            else
            {
                foreach (var obj in objectContainer.Get<CalculatedTable>()) inserted.Add(Serializer.DeserializeCalculatedTable(obj, Handler.Model));
                foreach (var obj in objectContainer.Get<Table>()) inserted.Add(Serializer.DeserializeTable(obj, Handler.Model));
            }

            foreach (var obj in objectContainer.Get<ModelRole>()) inserted.Add(Serializer.DeserializeModelRole(obj, Handler.Model));
            foreach (var obj in objectContainer.Get<ProviderDataSource>()) inserted.Add(Serializer.DeserializeProviderDataSource(obj, Handler.Model));
            foreach (var obj in objectContainer.Get<SingleColumnRelationship>()) inserted.Add(Serializer.DeserializeSingleColumnRelationship(obj, Handler.Model));
            foreach (var obj in objectContainer.Get<Perspective>()) inserted.Add(Serializer.DeserializePerspective(obj, Handler.Model));
            foreach (var obj in objectContainer.Get<Culture>()) inserted.Add(Serializer.DeserializeCulture(obj, Handler.Model));

            if(Handler.CompatibilityLevel >= 1400)
            {
                foreach (var obj in objectContainer.Get<NamedExpression>()) inserted.Add(Serializer.DeserializeNamedExpression(obj, Handler.Model));
                foreach (var obj in objectContainer.Get<StructuredDataSource>()) inserted.Add(Serializer.DeserializeStructuredDataSource(obj, Handler.Model));
            }

            if(Handler.CompatibilityLevel >= 1470)
            {
                CalculationGroupTable destCalcGroup = destination is CalculationGroupTable cgt ? cgt : 
                    (destination is CalculationItem ci ? ci.CalculationGroupTable : null);
                if (destCalcGroup != null)
                {
                    foreach (var obj in objectContainer.Get<CalculationItem>()) inserted.Add(Serializer.DeserializeCalculationItem(obj, destCalcGroup));
                }
                foreach (var obj in objectContainer.Get<CalculationGroupTable>()) inserted.Add(Serializer.DeserializeCalculationGroupTable(obj, Handler.Model));
            }

            foreach (var obj in inserted)
            {
                var tableObj = obj as Table;

                (obj as IInternalTranslatableObject)?.LoadTranslations(true);
                (obj as IInternalTabularPerspectiveObject)?.LoadPerspectives(true);
                if (tableObj != null)
                {
                    tableObj.LoadRLS();
                    Handler.Tree.RebuildFolderCacheForTable(tableObj);
                }

                if (!string.IsNullOrEmpty(folder) && obj is IFolderObject)
                {
                    (obj as IFolderObject).DisplayFolder = folder;
                }

                if (Handler.CompatibilityLevel >= 1400)
                {
                    if (tableObj != null) tableObj.LoadOLS(true);
                    (obj as Column)?.LoadOLS();
                }

                (obj as IInternalAnnotationObject)?.ClearTabularEditorAnnotations();
            }

            Handler.EndUpdate();
            FormulaFixup.BuildDependencyTree();

            return inserted;
        }

        public enum CheckResult
        {
            Ok,
            CantMove,
            ConfirmOverwrite
        }

        public CheckResult CheckMoveObjects(IEnumerable<IFolderObject> objects, Table newDestinationTable)
        {
            if (newDestinationTable == null || newDestinationTable is CalculationGroupTable) return CheckResult.CantMove;
            var result = CheckResult.Ok;
            foreach(var obj in objects)
            {
                if (obj == null) return CheckResult.CantMove;
                if (newDestinationTable.Columns.Contains(obj.Name)
                    || newDestinationTable.Measures.Contains(obj.Name)) result = CheckResult.ConfirmOverwrite;
                if (!(obj is Measure || obj is CalculatedColumn)) return CheckResult.CantMove;
            }
            return result;
        }

        public void MoveCalculationItem(CalculationItem item, CalculationGroupTable newCgt)
        {
            var name = item.Name;
            item.Delete();
            item.RenewMetadataObject();
            item.MetadataObject.Name = newCgt.CalculationItems.GetNewName(name);
            newCgt.CalculationItems.Add(item);
        }

        public void MoveObject(IFolderObject sourceObject, Table newDestinationTable, bool allowOverwrite)
        {
            if (sourceObject is Measure m)
            {
                var kpi = m.KPI;
                m.Delete();
                m.RenewMetadataObject();
                if (kpi != null) { kpi.RenewMetadataObject(); m.Reinit(); }
                newDestinationTable.Measures.Add(m);
            }
            if (sourceObject is CalculatedColumn c)
            {
                var name = c.Name;
                c.Delete();
                c.RenewMetadataObject();
                c.MetadataObject.Name = newDestinationTable.Columns.GetNewName(name);
                newDestinationTable.Columns.Add(c);
            }
        }
    }
}
