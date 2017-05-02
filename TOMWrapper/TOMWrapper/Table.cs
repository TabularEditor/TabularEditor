using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TextServices;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    partial class Table: ITabularObjectContainer, IDetailObjectContainer, ITabularPerspectiveObject, IDaxObject, IDynamicPropertyObject,
        IErrorMessageObject
    {
        [Browsable(false)]
        public HashSet<IExpressionObject> Dependants { get; private set; } = new HashSet<IExpressionObject>();

        #region Convenient methods
        [IntelliSense("Adds a new measure to the table.")]
        public Measure AddMeasure(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add measure");
            var measure = new Measure(this);
            if (!string.IsNullOrEmpty(name)) measure.Name = name;
            if (!string.IsNullOrEmpty(expression)) measure.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) measure.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return measure;
        }

        [IntelliSense("Adds a new calculated column to the table.")]
        public CalculatedColumn AddCalculatedColumn(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add calculated column");
            var column = new CalculatedColumn(this);
            if (!string.IsNullOrEmpty(name)) column.Name = name;
            if (!string.IsNullOrEmpty(expression)) column.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }

        [IntelliSense("Adds a new Data column to the table.")]
        public DataColumn AddDataColumn(string name = null, string sourceColumn = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add Data column");
            var column = new DataColumn(this);
            column.DataType = TOM.DataType.String;
            if (!string.IsNullOrEmpty(name)) column.Name = name;
            if (!string.IsNullOrEmpty(sourceColumn)) column.SourceColumn = sourceColumn;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }


        [IntelliSense("Adds a new hierarchy to the table.")]
        public Hierarchy AddHierarchy(string name = null, string displayFolder = null, params Column[] levels)
        {
            Handler.BeginUpdate("add hierarchy");
            var hierarchy = new Hierarchy(this);
            if (!string.IsNullOrEmpty(name)) hierarchy.Name = name;
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
        [Browsable(false)]
        public IEnumerable<Level> AllLevels { get { return Hierarchies.SelectMany(h => h.Levels); } }
        #endregion

        public override TabularNamedObject Clone(string newName = null, bool includeTranslations = false)
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
        }

        [IntelliSense("Deletes the table from the model.")]
        public override void Delete()
        {
            Handler.BeginUpdate("Delete child objects");

            // Remove row-level-security for this table:
            RowLevelSecurity.Clear();

            // Then, delete any relationships this table participates in:
            Model.Relationships.Where(r => r.FromTable == this || r.ToTable == this).ToList().ForEach(r => r.Delete());

            // Remove SortByColumn properties for all columns in the table:
            foreach (var c in Columns.Where(c => c.SortByColumn != null)) c.SortByColumn = null;

            // Finally, delete any child objects, starting with hierarchies:
            Hierarchies.ForEach(h => h.Delete());
            GetChildren().Cast<ITabularTableObject>().ToList().ForEach(c => c.Delete());

            // Remove the table from all perspectives:
            InPerspective.None();

            Handler.EndUpdate();


            base.Delete();
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.Table();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);

            Init();
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
        [Editor(typeof(RefreshGridCollectionEditor),typeof(UITypeEditor))]
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

        public virtual bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Source":
                case "Partitions":
                    return SourceType == TOM.PartitionSourceType.Query;
                default: return true;
            }
        }

        public virtual bool Editable(string propertyName)
        {
            return true;
        }

        [Browsable(true), DisplayName("Perspectives"), Category("Translations and Perspectives")]
        public PerspectiveIndexer InPerspective { get; private set; }

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

        protected override void Init()
        {
            InPerspective = new PerspectiveTableIndexer(this);
            Columns = new ColumnCollection(Handler, this.GetObjectPath() + ".Columns", MetadataObject.Columns, this);
            Measures = new MeasureCollection(Handler, this.GetObjectPath() + ".Measures", MetadataObject.Measures, this);
            Hierarchies = new HierarchyCollection(Handler, this.GetObjectPath() + ".Hierarchies", MetadataObject.Hierarchies, this);
            Partitions = new PartitionCollection(Handler, this.GetObjectPath() + ".Partitions", MetadataObject.Partitions, this);

            Columns.CollectionChanged += Children_CollectionChanged;
            Measures.CollectionChanged += Children_CollectionChanged;
            Hierarchies.CollectionChanged += Children_CollectionChanged;
            Partitions.CollectionChanged += Children_CollectionChanged;

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
    }

    public static class TableExtension
    {
        public static TOM.PartitionSourceType GetSourceType(this TOM.Table table)
        {
            return table.Partitions.FirstOrDefault()?.SourceType ?? TOM.PartitionSourceType.None;
        }
    }
}
