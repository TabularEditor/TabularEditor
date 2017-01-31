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

        [IntelliSense("Deletes the table from the model.")]
        public override void Delete()
        {
            Handler.BeginUpdate("Delete relationships and translations");

            // First, remove the table from all perspectives:
            InPerspective.None();

            // Then, delete any relationships this table participates in:
            Model.Relationships.Where(r => r.FromTable == this || r.ToTable == this).ToList().ForEach(r => r.Delete());

            // Finally, delete any child objects:
            GetChildren().Cast<ITabularTableObject>().ToList().ForEach(c => c.Delete());

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
        public ColumnCollection Columns { get; private set; }
        [Browsable(false)]
        public MeasureCollection Measures { get; private set; }
        [Browsable(false)]
        public HierarchyCollection Hierarchies { get; private set; }

        [Category("Data Source"),TypeConverter(typeof(IndexerConverter)),NoMultiselect()]
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
            Columns = new ColumnCollection(Handler, this.GetObjectPath() + ".Columns", MetadataObject.Columns);
            Measures = new MeasureCollection(Handler, this.GetObjectPath() + ".Measures", MetadataObject.Measures);
            Hierarchies = new HierarchyCollection(Handler, this.GetObjectPath() + ".Hierarchies", MetadataObject.Hierarchies);
            Partitions = new PartitionCollection(Handler, this.GetObjectPath() + ".Partitions", MetadataObject.Partitions);

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

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == "Name" && Handler.AutoFixup) Handler.DoFixup(this, (string)newValue);
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == "Name") Handler.BuildDependencyTree();
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
