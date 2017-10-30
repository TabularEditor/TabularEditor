using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

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

        public void AddColumnsToHierarchy(IEnumerable<Column> columns, Hierarchy hierarchy, int firstOrdinal = -1)
        {
            hierarchy.AddLevels(columns, firstOrdinal);
        }

        public Level AddColumnToHierarchy(Column column, Hierarchy hierarchy, int ordinal = -1)
        {
            return hierarchy.AddLevel(column, ordinal: ordinal);
        }

        public void SetContainer(IEnumerable<IDetailObject> objects, IDetailObjectContainer newContainer, Culture culture)
        {
            if (objects == null) throw new ArgumentNullException("objects");
            if (newContainer == null) throw new ArgumentNullException("newContainer");
            if (objects.Count() == 0) return;
            if (objects.Any(o => o.Table != newContainer.ParentTable)) throw new ArgumentException("Cannot move objects between tables. To move a measure from one table to another, use method \"MoveObjects\".");

            Handler.BeginUpdate("folder structure change");

            foreach(var obj in objects)
            {
                obj.SetDisplayFolder((newContainer as Folder)?.Path ?? "", culture);
            }

            Handler.EndUpdate();
        }

        public string NewMeasureName(string prefix)
        {
            // TODO: Refactor this (similar code in Measure.cs)
            // Loop through all tables, as measures must be uniquely named across the model:
            return Handler.Model.Tables.Select(t => t.MetadataObject.Measures.GetNewName(prefix)).OrderByDescending(p => p.Length).First();
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
            Handler.BeginUpdate("Paste objects");

            var inserted = new List<TabularObject>();

            // Possible destinations:
            var destHier = (destination as Level)?.Hierarchy ?? (destination as Hierarchy);
            var destTable = (destination as Folder)?.Table ?? (destination as Partition)?.Table ?? (destination as PartitionViewTable)?.Table ?? destHier?.Table ?? (destination as IDetailObject)?.Table ?? (destination as Table);
            var folder = (destination as Folder)?.Path;

            if (destHier != null)
            {
                // Levels can only be deserialized on a Hierarchy destination:
                foreach (var obj in objectContainer[typeof(Level)]) inserted.Add(Serializer.DeserializeLevel(obj, destHier));
                destHier.CompactLevelOrdinals();
            }

            if (destTable?.GetType() == typeof(Table))
            {
                // DataColumns and Partitions can only be deserialized onto a Table destination (not CalculatedTable):
                foreach (var obj in objectContainer[typeof(DataColumn)]) inserted.Add(Serializer.DeserializeDataColumn(obj, destTable));
                foreach (var obj in objectContainer[typeof(Partition)]) inserted.Add(Serializer.DeserializePartition(obj, destTable));
            }
            if (destTable is Table)
            {
                // Measures, Hierarchies and CalculatedColumns can be deserialized onto a Table (or Table derived) destinated:
                foreach (var obj in objectContainer[typeof(CalculatedColumn)]) inserted.Add(Serializer.DeserializeCalculatedColumn(obj, destTable));
                foreach (var obj in objectContainer[typeof(Hierarchy)]) inserted.Add(Serializer.DeserializeHierarchy(obj, destTable));
                foreach (var obj in objectContainer[typeof(Measure)]) inserted.Add(Serializer.DeserializeMeasure(obj, destTable));
            }

            foreach (var obj in objectContainer[typeof(CalculatedTable)]) inserted.Add(Serializer.DeserializeCalculatedTable(obj, Handler.Model));
            foreach (var obj in objectContainer[typeof(Table)]) inserted.Add(Serializer.DeserializeTable(obj, Handler.Model));

            foreach (var obj in objectContainer[typeof(ModelRole)]) inserted.Add(Serializer.DeserializeModelRole(obj, Handler.Model));
            foreach (var obj in objectContainer[typeof(ProviderDataSource)]) inserted.Add(Serializer.DeserializeProviderDataSource(obj, Handler.Model));
            foreach (var obj in objectContainer[typeof(SingleColumnRelationship)]) inserted.Add(Serializer.DeserializeSingleColumnRelationship(obj, Handler.Model));
            foreach (var obj in objectContainer[typeof(Perspective)]) inserted.Add(Serializer.DeserializePerspective(obj, Handler.Model));
            foreach (var obj in objectContainer[typeof(Culture)]) inserted.Add(Serializer.DeserializeCulture(obj, Handler.Model));

            if(Handler.CompatibilityLevel >= 1400)
            {
                foreach (var obj in objectContainer[typeof(NamedExpression)]) inserted.Add(Serializer.DeserializeNamedExpression(obj, Handler.Model));
                foreach (var obj in objectContainer[typeof(StructuredDataSource)]) inserted.Add(Serializer.DeserializeStructuredDataSource(obj, Handler.Model));
            }

            foreach (var obj in inserted)
            {
                (obj as ITranslatableObject)?.LoadTranslations(true);
                (obj as ITabularPerspectiveObject)?.LoadPerspectives(true);
                (obj as Table)?.LoadRLS();

                if (!string.IsNullOrEmpty(folder) && obj is IDetailObject)
                {
                    (obj as IDetailObject).DisplayFolder = folder;
                }

                if (Handler.CompatibilityLevel >= 1400)
                {
                    (obj as Table)?.LoadOLS(true);
                    (obj as Column)?.LoadOLS();
                }

                (obj as IAnnotationObject)?.ClearTabularEditorAnnotations();
            }

            Handler.EndUpdate();
            FormulaFixup.BuildDependencyTree();

            return inserted;
        }

        public void MoveObjects(IEnumerable<IDetailObject> objects, Table newTable)
        {
            if (objects == null) throw new ArgumentNullException("objects");
            if (newTable == null) throw new ArgumentNullException("newContainer");
            if (objects.Count() == 0) return;
            if (!objects.All(obj => obj is Measure || obj is CalculatedColumn)) throw new ArgumentException("Only Measures and Calculated Columns can be moved between tables.");

            var res = System.Windows.Forms.DialogResult.Yes;

            // Check if an object with the given name already exists:
            if (objects.OfType<Measure>().Any(obj => obj.Table == null && NewMeasureName(obj.Name) != obj.Name) ||
                objects.OfType<CalculatedColumn>().Any(obj => obj.Table == null && NewColumnName(obj.Name, newTable) != obj.Name)) {
                res = System.Windows.Forms.MessageBox.Show("One or more objects with the given name already exists in the destination. Do you want to overwrite the destination objects?", "Overwrite existing objects?", System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Exclamation);
                if (res == System.Windows.Forms.DialogResult.Cancel) return;
            }

            Handler.BeginUpdate("move objects");
            foreach (var obj in objects)
            {
                // TODO: When dragging between two tables, the original object (measure or calc column) is actually being deleted
                // and a copy is created in the destination table. This means that the Tree Explorer cannot re-select the node
                // corresponding to the created copy, as it searches for a node corresponding to an object that no longer exists
                // in the logical TOM. This happens in the _model_StructureChanged method of TreeViewAdv.cs.
                // In order to fix this, we would need a lookup mechanism that relies on the names of the objects within the TOM.

                // Objects moved between two tables:
                if(obj.Table != null || (obj as TabularNamedObject).MetadataObject.IsRemoved)
                {
                    var name = obj.Name;
                    TabularNamedObject newObj = null;
                    if (obj is Measure) { newObj = (obj as Measure).Clone(newParent: newTable); }
                    if (obj is CalculatedColumn) { newObj = (obj as CalculatedColumn).Clone(newParent: newTable); }

                    (obj as TabularNamedObject).Delete();
                    newObj.Name = name;
                }
                else
                // Objects moved between two instances of Tabular Editor (source Table will be null in this case):
                {
                    var name = obj.Name;
                    bool setNewName = false;
                    if (res == System.Windows.Forms.DialogResult.No) (obj as TabularNamedObject).MetadataObject.Name = "_temp_" + Guid.NewGuid().ToString();

                    if (obj is Measure)
                    {
                        var existingMeasureTable = Handler.Model.Tables.FirstOrDefault(t => t.Measures.Contains(name));

                        if(existingMeasureTable != null)
                        {
                            if (res == System.Windows.Forms.DialogResult.Yes) existingMeasureTable.Measures[obj.Name].Delete();
                            else setNewName = true;
                        }
                        newTable.Measures.Add(obj as Measure);
                        if (setNewName) obj.Name = NewMeasureName(name);
                    }
                    if (obj is CalculatedColumn)
                    {
                        if (newTable.Columns.Contains(name))
                        {
                            if (res == System.Windows.Forms.DialogResult.Yes) newTable.Columns[obj.Name].Delete();
                            else setNewName = true;
                        }
                        newTable.Columns.Add(obj as CalculatedColumn);
                        if (setNewName) obj.Name = NewColumnName(name, newTable);
                    }
                }

                //obj.SetDisplayFolder((newContainer as Folder)?.Path ?? "", culture);
            }

            Handler.EndUpdate();
        }
    }
}
