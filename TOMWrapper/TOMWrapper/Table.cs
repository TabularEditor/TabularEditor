using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TextServices;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    partial class Table: ITabularObjectContainer, IDetailObjectContainer, ITabularPerspectiveObject, IDaxObject,
        IErrorMessageObject
    {
        [Browsable(false)]
        public HashSet<IExpressionObject> Dependants { get; private set; } = new HashSet<IExpressionObject>();

        #region Convenient methods
        [IntelliSense("Adds a new measure to the table.")]
        public Measure AddMeasure(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add measure");
            var measure = new Measure(this, name);
            if (!string.IsNullOrEmpty(expression)) measure.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) measure.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return measure;
        }

        [IntelliSense("Adds a new calculated column to the table.")]
        public CalculatedColumn AddCalculatedColumn(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add calculated column");
            var column = new CalculatedColumn(this, name);
            if (!string.IsNullOrEmpty(expression)) column.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }

        [IntelliSense("Adds a new Data column to the table.")]
        public DataColumn AddDataColumn(string name = null, string sourceColumn = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add Data column");
            var column = new DataColumn(this, name);
            column.DataType = TOM.DataType.String;
            if (!string.IsNullOrEmpty(sourceColumn)) column.SourceColumn = sourceColumn;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }


        [IntelliSense("Adds a new hierarchy to the table.")]
        public Hierarchy AddHierarchy(string name = null, string displayFolder = null, params Column[] levels)
        {
            Handler.BeginUpdate("add hierarchy");
            var hierarchy = new Hierarchy(this, name);
            if (!string.IsNullOrEmpty(displayFolder)) hierarchy.DisplayFolder = displayFolder;
            for(var i = 0; i < levels.Length; i++)
            {
                hierarchy.AddLevel(levels[i], ordinal: i);
            }
            Handler.EndUpdate();
            return hierarchy;
        }
        
        public Hierarchy AddHierarchy(string name, string displayFolder = null, params string[] levels)
        {
            return AddHierarchy(name, displayFolder, levels.Select(s => Columns[s]).ToArray());
        }

        #endregion
        #region Convenient Collections
        /// <summary>
        /// Enumerates all levels across all hierarchies on this table.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Level> AllLevels { get { return Hierarchies.SelectMany(h => h.Levels); } }
        /// <summary>
        /// Enumerates all relationships in which this table participates.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Relationship> UsedInRelationships { get { return Model.Relationships.Where(r => r.FromTable == this || r.ToTable == this); } }
        #endregion

        /*public override TabularNamedObject Clone(string newName = null, bool includeTranslations = false)
        {
            Handler.BeginUpdate("duplicate table");

            var mt = MetadataObject.Clone();
            mt.Name = !string.IsNullOrEmpty(newName) ? newName : Model.MetadataObject.Tables.GetNewName(mt.Name);

            Table t;
            if (mt.GetSourceType() == TOM.PartitionSourceType.Calculated) t = new CalculatedTable(Handler, mt);
            else t = new Table(Handler, mt);
            Model.Tables.Add(t);
            t.InitRLSIndexer();

            // Update dependencies and do fix-up of calculated columns and measures on the cloned table:
            t.Dependants = new HashSet<IExpressionObject>(
                Dependants
                    .Where(eo => eo is CalculatedColumn && (eo as CalculatedColumn).Table == this)
                    .Select(eo => t.Columns[eo.Name]).OfType<IExpressionObject>());
            foreach(var eo in t.Columns.OfType<CalculatedColumn>())
            {
                eo.Dependencies = (Columns[eo.Name] as CalculatedColumn).Dependencies.ToDictionary(kvp => kvp.Key == this ? t : kvp.Key, kvp => kvp.Value);
            }
            foreach (var m in t.Measures)
            {
                m.Dependencies = Measures[m.Name].Dependencies.ToDictionary(kvp => kvp.Key == this ? t : kvp.Key, kvp => kvp.Value);
            }
            Handler.DoFixup(t, t.Name);

            Handler.UpdateTables();
            Handler.EndUpdate();

            return t;
        }*/

        protected override void Cleanup()
        {
            // Remove row-level-security for this table:
            RowLevelSecurity.Clear();

            // Then, delete any relationships this table participates in:
            UsedInRelationships.ToList().ForEach(r => r.Delete());

            // Delete any child objects, starting with hierarchies:
            Hierarchies.ForEach(h => h.Delete());
            Columns.ForEach(c => c.Delete());
            Measures.ForEach(m => m.Delete());

            // Delete any partitions on the table:
            Partitions.ForEach(p => p.Delete());

            base.Cleanup();
        }

        [Browsable(false)]
        public Table ParentTable { get { return this; } }

        [Browsable(false)]
        public ColumnCollection Columns { get; protected set; }
        [Browsable(false)]
        public MeasureCollection Measures { get; private set; }
        [Browsable(false)]
        public HierarchyCollection Hierarchies { get; private set; }

        [Category("Data Source"),NoMultiselect()]
        [Editor(typeof(PartitionCollectionEditor),typeof(UITypeEditor))]
        public PartitionCollection Partitions { get; private set; }

        [Category("Data Source")]
        public string Source {
            get
            {
                var ds = (MetadataObject.Partitions.FirstOrDefault().Source as TOM.QueryPartitionSource)?.DataSource;
                string sourceName = null;
                if (ds != null) sourceName = (Handler.WrapperLookup[ds] as DataSource)?.Name;
                return sourceName ?? ds?.Name;
            }
        }
        [Category("Data Source"), DisplayName("Source Type")]
        public TOM.PartitionSourceType SourceType
        {
            get
            {
                return MetadataObject.GetSourceType();
            }
        }

        protected override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Source":
                case "Partitions":
#if CL1400
                    return SourceType == TOM.PartitionSourceType.Query || SourceType == TOM.PartitionSourceType.M;
#else
                    return SourceType == TOM.PartitionSourceType.Query;
#endif
                // Compatibility Level 1400-specific properties:
                case "DefaultDetailRowsExpression":
                case "ShowAsVariationsOnly":
                case "IsPrivate":
                    return Model.Database.CompatibilityLevel >= 1400;
                default: return true;
            }
        }

        [Browsable(true), DisplayName("Row Level Filters"), Category("Security")]
        public TableRLSIndexer RowLevelSecurity { get; private set; }

        [Browsable(false)]
        public string DaxObjectName
        {
            get
            {
                return string.Format("'{0}'", Name);
            }
        }

        [Browsable(true), Category("Metadata"), DisplayName("DAX identifier")]
        public string DaxObjectFullName
        {
            get
            {
                return DaxObjectName;
            }
        }

        [Browsable(false)]
        public string DaxTableName
        {
            get
            {
                return DaxObjectName;
            }
        }

        /// <summary>
        /// Returns all columns, measures and hierarchies inside this table.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return Columns.Concat<ITabularNamedObject>(Measures).Concat(Hierarchies);
        }

        public IEnumerable<IDetailObject> GetChildrenByFolders(bool recursive)
        {
            throw new InvalidOperationException();
        }

        public PartitionViewTable PartitionViewTable { get; private set; }

        protected override void Init()
        {
            Columns = new ColumnCollection(this.GetObjectPath() + ".Columns", MetadataObject.Columns, this);
            Measures = new MeasureCollection(this.GetObjectPath() + ".Measures", MetadataObject.Measures, this);
            Hierarchies = new HierarchyCollection(this.GetObjectPath() + ".Hierarchies", MetadataObject.Hierarchies, this);
            Partitions = new PartitionCollection(this.GetObjectPath() + ".Partitions", MetadataObject.Partitions, this);

            Columns.CollectionChanged += Children_CollectionChanged;
            Measures.CollectionChanged += Children_CollectionChanged;
            Hierarchies.CollectionChanged += Children_CollectionChanged;
            Partitions.CollectionChanged += Partitions_CollectionChanged;

            PartitionViewTable = new PartitionViewTable(this);

            CheckChildrenErrors();
        }

        public void InitRLSIndexer()
        {
            RowLevelSecurity = new TableRLSIndexer(this);
        }


        [Category("Metadata"),DisplayName("Error Message")]
        public virtual string ErrorMessage { get; protected set; }

        public virtual void CheckChildrenErrors()
        {
            var errObj = GetChildren().OfType<IErrorMessageObject>().FirstOrDefault(c => !string.IsNullOrEmpty(c.ErrorMessage));
            if (errObj != null && (!(errObj as IExpressionObject)?.NeedsValidation ?? true))
            {
                ErrorMessage = "Error on " + (errObj as TabularNamedObject).Name + ": " + errObj.ErrorMessage;
            }
            else
            {
                ErrorMessage = null;
            }
        }

        protected void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
                Handler.Tree.OnNodesInserted(this, e.NewItems.Cast<ITabularObject>());
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                Handler.Tree.OnNodesRemoved(this, e.OldItems.Cast<ITabularObject>());
        }
        private void Partitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Handler.Tree.OnNodesInserted(PartitionViewTable, e.NewItems.Cast<ITabularObject>());
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                Handler.Tree.OnNodesRemoved(PartitionViewTable, e.OldItems.Cast<ITabularObject>());
        }

        public override string Name
        {
            set
            {
                if (value.IndexOfAny(InvalidTableNameChars) != -1) throw new ArgumentException("Table name cannot contain any of the following characters: " + string.Join(" ", InvalidTableNameChars));
                base.Name = value;
            }
            get { return base.Name; }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Name" && Handler.AutoFixup)
            {
                Handler.DoFixup(this, (string)newValue);
                Handler.UndoManager.EndBatch();
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == "Name")
            {
                Handler.BuildDependencyTree();

                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.AutoFixup) Handler.UndoManager.BeginBatch("Name change");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        public static readonly char[] InvalidTableNameChars = {
            '.', ',', ';', '\'', '`', ':', '/', '\\', '*', '|',
            '?', '"', '&', '%', '$' , '!', '+', '=', '(', ')',
            '[', ']', '{', '}', '<', '>'
        };

#if CL1400
        [DisplayName("Default Detail Rows Expression")]
        [Category("Options"), IntelliSense("A DAX expression specifying default detail rows for this table (drill-through in client tools).")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DetailRowsExpression
        {
            get
            {
                return MetadataObject.DefaultDetailRowsDefinition?.Expression;
            }
            set
            {
                var oldValue = DetailRowsExpression;

                if (oldValue == value) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging("DefaultDetailRowsExpression", value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DefaultDetailRowsDefinition == null) MetadataObject.DefaultDetailRowsDefinition = new TOM.DetailRowsDefinition();
                MetadataObject.DefaultDetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value)) MetadataObject.DefaultDetailRowsDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, "DefaultDetailRowsExpression", oldValue, value));
                OnPropertyChanged("DefaultDetailRowsExpression", oldValue, value);
            }
        }
#endif
    }

    /// <summary>
    /// Wrapper class for the "Table Partitions" logical group.
    /// </summary>
    public class PartitionViewTable: ITabularNamedObject, ITabularObjectContainer
    {
        public Table Table { get; private set; }
        internal PartitionViewTable(Table table)
        {
            Table = table;
        }

        [Category("Partitions"), NoMultiselect()]
        [Editor(typeof(PartitionCollectionEditor), typeof(UITypeEditor))]
        public PartitionCollection Partitions { get { return Table.Partitions; } }

        [Category("Partitions"),DisplayName("Table Name")]
        public string Name { get => Table.Name; set => Table.Name = value; }

        [Browsable(false)]
        public int MetadataIndex => Table.MetadataIndex;

        [Browsable(false)]
        public ObjectType ObjectType => ObjectType.Table;

        [Browsable(false)]
        public Model Model => Table.Model;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return Partitions;
        }
    }

    public static class TableExtension
    {
        public static TOM.PartitionSourceType GetSourceType(this TOM.Table table)
        {
            return table.Partitions.FirstOrDefault()?.SourceType ?? TOM.PartitionSourceType.None;
        }
    }
}
