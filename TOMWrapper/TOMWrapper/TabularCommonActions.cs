using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Provides convenient methods for common actions on a Tabular Model, that often involve changing multiple objects at once.
    /// For example, these methods may be used to easily perform UI drag and drop operations that will change hierarchy levels,
    /// display folders, etc.
    /// </summary>
    public class TabularCommonActions
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
            // Loop through all tables, as measures must be uniquely named across the model:
            return Handler.Model.Tables.Select(t => t.MetadataObject.Measures.GetNewName(prefix)).OrderByDescending(p => p.Length).First();
        }
        public string NewColumnName(string prefix, Table table)
        {
            return table.Columns.MetadataObjectCollection.GetNewName(prefix);
        }

        public void MoveObjects(IEnumerable<IDetailObject> objects, Table newTable, Culture culture)
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
                        newTable.Measures.Add(obj);
                        if (setNewName) obj.Name = NewMeasureName(name);
                    }
                    if (obj is CalculatedColumn)
                    {
                        if (newTable.Columns.Contains(name))
                        {
                            if (res == System.Windows.Forms.DialogResult.Yes) newTable.Columns[obj.Name].Delete();
                            else setNewName = true;
                        }
                        newTable.Columns.Add(obj);
                        if (setNewName) obj.Name = NewColumnName(name, newTable);
                    }
                }

                //obj.SetDisplayFolder((newContainer as Folder)?.Path ?? "", culture);
            }

            Handler.EndUpdate();
        }
    }
}
