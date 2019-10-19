using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    partial class Table: IFolder, ITabularPerspectiveObject, IDaxObject,
        IErrorMessageObject, IDaxDependantObject, IExpressionObject
    {
        /// <summary>
        /// Gets the visibility of the Table. Shorthand for !<see cref="IsHidden"/>.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible => !IsHidden;

        internal Dictionary<string, Folder> FolderCache = new Dictionary<string, Folder>();


        private DependsOnList _dependsOn = null;

        string IExpressionObject.Expression { get { return ""; } set { } }

        [Browsable(false)]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        [Browsable(false)]
        public ReferencedByList ReferencedBy { get; } = new ReferencedByList();

        protected override bool AllowDelete(out string message)
        {
            message = string.Empty;
            if (ReferencedBy.Count > 0 && ReferencedBy.Deep().Any(
                obj => 
                    (obj is ITabularTableObject && (obj as ITabularTableObject).Table != this) || 
                    (obj is Table && obj != this) || 
                    (obj is TablePermission)
            ))
                message += Messages.ReferencedByDAX;
            if (message == string.Empty) message = null;
            return true;
        }

        #region Convenient methods
        [IntelliSense("Adds a new measure to the table."), Tests.GenerateTest()]
        public Measure AddMeasure(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add measure");
            var measure = Measure.CreateNew(this, name);
            if (!string.IsNullOrEmpty(expression)) measure.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) measure.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return measure;
        }

        [IntelliSense("Adds a new calculated column to the table."),Tests.GenerateTest()]
        public CalculatedColumn AddCalculatedColumn(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add calculated column");
            var column = CalculatedColumn.CreateNew(this, name);
            if (!string.IsNullOrEmpty(expression)) column.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }

        [IntelliSense("Adds a new Data column to the table."), Tests.GenerateTest()]
        public DataColumn AddDataColumn(string name = null, string sourceColumn = null, string displayFolder = null, DataType dataType = DataType.String)
        {
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataColumn))) return null;

            Handler.BeginUpdate("add Data column");
            var column = DataColumn.CreateNew(this, name);
            column.DataType = dataType;
            if (!string.IsNullOrEmpty(sourceColumn)) column.SourceColumn = sourceColumn;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }


        [IntelliSense("Adds a new hierarchy to the table."), Tests.GenerateTest()]
        public Hierarchy AddHierarchy(string name = null, string displayFolder = null, params Column[] levels)
        {
            Handler.BeginUpdate("add hierarchy");
            var hierarchy = Hierarchy.CreateNew(this, name);
            if (!string.IsNullOrEmpty(displayFolder)) hierarchy.DisplayFolder = displayFolder;
            for(var i = 0; i < levels.Length; i++)
            {
                hierarchy.AddLevel(levels[i], ordinal: i);
            }
            Handler.EndUpdate();
            return hierarchy;
        }
        
        [IntelliSense("Adds a new hierarchy to the table.")]
        public Hierarchy AddHierarchy(string name, string displayFolder = null, params string[] levels)
        {
            return AddHierarchy(name, displayFolder, levels.Select(s => Columns[s]).ToArray());
        }

        #endregion
        #region Convenient Collections
        /// <summary>
        /// Enumerates all levels across all hierarchies on this table.
        /// </summary>
        [Browsable(false),IntelliSense("Enumerates all levels across all hierarchies on this table.")]
        public IEnumerable<Level> AllLevels { get { return Hierarchies.SelectMany(h => h.Levels); } }
        /// <summary>
        /// Enumerates all relationships in which this table participates.
        /// </summary>
        [Browsable(false),IntelliSense("Enumerates all relationships in which this table participates.")]
        public IEnumerable<SingleColumnRelationship> UsedInRelationships { get { return Model.Relationships.Where(r => r.FromTable == this || r.ToTable == this); } }

        [Browsable(false), IntelliSense("Enumerates only the Data Columns on this table.")]
        public IEnumerable<DataColumn> DataColumns => Columns.OfType<DataColumn>();

        [Browsable(false), IntelliSense("Enumerates only the Calculated Columns on this table.")]
        public IEnumerable<CalculatedColumn> CalculatedColumns => Columns.OfType<CalculatedColumn>();

        /// <summary>
        /// Enumerates all tables related to or from this table.
        /// </summary>
        [Browsable(false), IntelliSense("Enumerates all tables related to or from this table.")]
        public IEnumerable<Table> RelatedTables
        {
            get
            {
                return UsedInRelationships.Select(r => r.FromTable)
                    .Concat(UsedInRelationships.Select(r => r.ToTable))
                    .Where(t => t != this).Distinct();
            }
        }
        #endregion

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            // Clear folder cache:
            FolderCache.Clear();

            // Remove row-level-security for this table:
            RowLevelSecurity.Clear();
            if(Handler.CompatibilityLevel >= 1400) ObjectLevelSecurity.Clear();
            foreach (var r in Model.Roles) if(r.TablePermissions.Contains(Name)) r.TablePermissions[this].Delete();

            base.DeleteLinkedObjects(isChildOfDeleted);
        }

        [Browsable(false)]
        public Table ParentTable { get { return this; } }

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
        public PartitionSourceType SourceType
        {
            get
            {
                return (PartitionSourceType)MetadataObject.GetSourceType();
            }
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.SOURCE:
                case Properties.PARTITIONS:
                    return SourceType == PartitionSourceType.Query || SourceType == PartitionSourceType.M;
                case Properties.DEFAULTDETAILROWSEXPRESSION:
                case Properties.SHOWASVARIATIONSONLY:
                case Properties.ISPRIVATE:
                    return Handler.CompatibilityLevel >= 1400;
                case Properties.OBJECTLEVELSECURITY:
                    return Handler.CompatibilityLevel >= 1400 && Model.Roles.Any();
                case Properties.ROWLEVELSECURITY:
                    return Model.Roles.Any();
                case Properties.ALTERNATESOURCEPRECEDENCE:
                    return Handler.CompatibilityLevel >= 1460 && Handler.CompatibilityLevel < 1470;
                case Properties.EXCLUDEFROMMODELREFRESH:
                    return Handler.CompatibilityLevel >= 1480;
                default: return true;
            }
        }

        [Browsable(true), DisplayName("Row Level Security"), Category("Security")]
        public TableRLSIndexer RowLevelSecurity { get; private set; }

        [Browsable(false)]
        public string DaxObjectName
        {
            get
            {
                return string.Format("'{0}'", Name.Replace("'", "''"));
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
        public virtual IEnumerable<ITabularNamedObject> GetChildren()
        {
            foreach (var m in Measures) yield return m;
            foreach (var c in Columns) yield return c;
            foreach (var h in Hierarchies) yield return h;
            yield break;
        }

        public virtual IEnumerable<IFolderObject> GetChildrenByFolders()
        {
            return FolderCache[""].GetChildrenByFolders();
        }

        protected override void Init()
        {
            if (Partitions.Count == 0 && !(this is CalculatedTable))
            {
                // Make sure the table contains at least one partition (Calculated Tables handles this on their own), but don't add it to the undo stack:
                Handler.UndoManager.Enabled = false;

                if (Model.DataSources.Any(ds => ds.Type == DataSourceType.Structured))
                    MPartition.CreateNew(this, Name);
                else
                    Partition.CreateNew(this, Name);

                Handler.UndoManager.Enabled = true;
            }

            RowLevelSecurity = new TableRLSIndexer(this);

            if (Handler.CompatibilityLevel >= 1400)
            {
                ObjectLevelSecurity = new TableOLSIndexer(this);
            }

            base.Init();
        }

        private TableOLSIndexer _objectLevelSecurtiy;
        [DisplayName("Object Level Security"), Category("Security")]
        public TableOLSIndexer ObjectLevelSecurity
        {
            get
            {
                if (Handler.CompatibilityLevel < 1400) throw new InvalidOperationException(Messages.CompatibilityError_ObjectLevelSecurity);
                return _objectLevelSecurtiy;
            }
            set
            {
                if (Handler.CompatibilityLevel < 1400) throw new InvalidOperationException(Messages.CompatibilityError_ObjectLevelSecurity);
                _objectLevelSecurtiy = value;
            }
        }
        private bool ShouldSerializeObjectLevelSecurity() { return false; }

        public static readonly Dictionary<Type, DataType> DataTypeMapping =
            new Dictionary<Type, DataType>() {
                { typeof(string), DataType.String },
                { typeof(char), DataType.String },
                { typeof(byte), DataType.Int64 },
                { typeof(sbyte), DataType.Int64 },
                { typeof(short), DataType.Int64 },
                { typeof(ushort), DataType.Int64 },
                { typeof(int), DataType.Int64 },
                { typeof(uint), DataType.Int64 },
                { typeof(long), DataType.Int64 },
                { typeof(ulong), DataType.Int64 },
                { typeof(float), DataType.Double },
                { typeof(double), DataType.Double },
                { typeof(decimal), DataType.Decimal },
                { typeof(bool), DataType.Boolean },
                { typeof(DateTime), DataType.DateTime },
                { typeof(byte[]), DataType.Binary },
                { typeof(object), DataType.Variant }
            };

        [IntelliSense("Creates a Data Column of suitable type for each column in the source query. Only works for OLE DB partition sources.")]
        public void RefreshDataColumns()
        {
            if (Partitions.Count == 0 || !(Partitions[0].DataSource is ProviderDataSource) || string.IsNullOrEmpty(Partitions[0].Query))
                throw new InvalidOperationException("The first partition on this table must use a ProviderDataSource with a valid OLE DB query.");

            try
            {
                using (var conn = new OleDbConnection((Partitions[0].DataSource as ProviderDataSource).ConnectionString))
                {
                    conn.Open();

                    var cmd = new OleDbCommand(Partitions[0].Query, conn);
                    var rdr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                    var schema = rdr.GetSchemaTable();

                    foreach(DataRow row in schema.Rows)
                    {
                        var name = (string)row["ColumnName"];
                        var type = (Type)row["DataType"];
                        
                        if(!Columns.Contains(name))
                        {
                            var col = AddDataColumn(name, name);
                            col.DataType = DataTypeMapping.ContainsKey(type) ? DataTypeMapping[type] : DataType.Automatic;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to generate metadata from partition source query: " + ex.Message);
            }
        }

        internal void AddError(IFolderObject folderObject)
        {
            if (ErrorMessage == null)
            {
                ErrorMessage = "Child objects with errors:";
            }
            if (folderObject is Folder f) ErrorMessage += "\r\nObjects inside the '" + f.Name + "' folder.";

            else ErrorMessage += "\r\n" + folderObject.GetTypeName() + " " + folderObject.GetName();
        }

        private string em;

        [Category("Metadata"),DisplayName("Error Message")]
        public virtual string ErrorMessage {
            get { return em; }
            protected set { em = value; }
        }
        internal virtual void ClearError()
        {
            ErrorMessage = null;

            if (Handler.CompatibilityLevel >= 1400 && !string.IsNullOrEmpty(MetadataObject.DefaultDetailRowsDefinition?.ErrorMessage))
                ErrorMessage = "Detail rows expression: " + MetadataObject.DefaultDetailRowsDefinition.ErrorMessage;
        }

        /// <summary>
        /// Loops through all child objects and propagates any error messages to their immediate parents - this should be called
        /// whenever the folder structure is changed
        /// </summary>
        internal virtual void PropagateChildErrors()
        {
            foreach(var child in GetChildren().OfType<IErrorMessageObject>().Where(c => !string.IsNullOrEmpty(c.ErrorMessage)))
            {
                if(!(child is CalculationGroupAttribute)) Handler._errors.Add(child);
                if (child is IFolderObject fo)
                {
                    var parentFolder = fo.GetFolder(Handler.Tree.Culture);
                    if (parentFolder != null) parentFolder.AddError(fo);
                    else AddError(fo);
                }
            }
        }

        protected override void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.Children_CollectionChanged(sender, e);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == Properties.NAME)
            {
                if (Handler.Settings.AutoFixup)
                {
                    // Fixup is not performed during an undo operation. We rely on the undo stack to fixup the expressions
                    // affected by the name change (the undo stack should contain the expression changes that were made
                    // when the name was initially changed).
                    if (!Handler.UndoManager.UndoInProgress) FormulaFixup.DoFixup(this, true);
                    FormulaFixup.BuildDependencyTree();
                    Handler.EndUpdate();
                }

                // Update relationship "names" if this table participates in any relationships:
                var rels = UsedInRelationships.ToList();
                if (rels.Count > 1) Handler.Tree.BeginUpdate();
                rels.ForEach(r => r.UpdateName());
                if (rels.Count > 1) Handler.Tree.EndUpdate();
            }
            if (propertyName == Properties.DEFAULTDETAILROWSEXPRESSION)
            {
                FormulaFixup.BuildDependencyTree(this);
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == Properties.NAME)
            {
                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.Settings.AutoFixup) Handler.BeginUpdate("Set Property 'Name'");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [DisplayName("Default Detail Rows Expression")]
        [Category("Options"), IntelliSense("A DAX expression specifying default detail rows for this table (drill-through in client tools).")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DefaultDetailRowsExpression
        {
            get
            {
                return MetadataObject.DefaultDetailRowsDefinition?.Expression;
            }
            set
            {
                var oldValue = DefaultDetailRowsExpression;

                if (oldValue == value) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DEFAULTDETAILROWSEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DefaultDetailRowsDefinition == null) MetadataObject.DefaultDetailRowsDefinition = new TOM.DetailRowsDefinition();
                MetadataObject.DefaultDetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value)) MetadataObject.DefaultDetailRowsDefinition = null;

                if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, Properties.DEFAULTDETAILROWSEXPRESSION, oldValue, value));
                OnPropertyChanged(Properties.DEFAULTDETAILROWSEXPRESSION, oldValue, value);
            }
        }

        [Browsable(false)]
        public virtual bool NeedsValidation
        {
            get
            {
                return false;
            }

            set
            {
                
            }
        }
    }
    
    internal static partial class Properties
    {
        public const string DEFAULTDETAILROWSEXPRESSION = "DefaultDetailRowsExpression";
        public const string OBJECTLEVELSECURITY = "ObjectLevelSecurity";
        public const string ROWLEVELSECURITY = "RowLevelSecurity";
    }

    internal static class TableExtension
    {
        public static TOM.PartitionSourceType GetSourceType(this TOM.Table table)
        {
            return table.Partitions.FirstOrDefault()?.SourceType ?? TOM.PartitionSourceType.None;
        }
        public static bool IsCalculatedOrCalculationGroup(this TOM.Table table)
        {
            var sourceType = GetSourceType(table);
            return sourceType == TOM.PartitionSourceType.Calculated || sourceType == TOM.PartitionSourceType.CalculationGroup;
        }
    }
}
