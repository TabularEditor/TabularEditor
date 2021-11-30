using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;
using TabularEditor.TOMWrapper.PowerBI;
using TabularEditor.Utils;
using System.ComponentModel.Design;

namespace TabularEditor.TOMWrapper
{
    public partial class Table: IFolder, ITabularPerspectiveObject, IDaxObject,
        IErrorMessageObject, IDaxDependantObject, IExpressionObject
    {
        /// <summary>
        /// Indicates whether the table is currently visible to end users. This is the case if the table contains visible measures.
        /// </summary>
        [Browsable(false), IntelliSense("Indicates whether the table is currently visible to end users. This is the case if the table contains visible measures.")]
        public bool IsVisible => !IsHidden || Measures.Any(m => !m.IsHidden);

        internal Dictionary<string, Folder> FolderCache = new Dictionary<string, Folder>();


        private DependsOnList _dependsOn = null;

        string IExpressionObject.Expression { get { return ""; } set { } }

        /// <summary>
        /// Gets the list of objects that this table depends on.
        /// </summary>
        [Browsable(false), IntelliSense("Gets the list of objects that this table depends on.")]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        /// <summary>
        /// Gets the list of objects that reference this table.
        /// </summary>
        [Browsable(false), IntelliSense("Gets the list of objects that reference this table.")]
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
        /// <summary>
        /// Adds a new measure to the table and returns a reference to the measure.
        /// </summary>
        /// <param name="name">Name of the measure</param>
        /// <param name="expression">DAX expression to assign to the measure</param>
        /// <param name="displayFolder">Display Folder to assign to the measure</param>
        /// <returns>A reference to the newly added measure.</returns>
        [IntelliSense("Adds a new measure to the table and returns a reference to the measure."), Tests.GenerateTest()]
        public Measure AddMeasure(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add measure");
            var measure = Measure.CreateNew(this, name);
            if (!string.IsNullOrEmpty(expression)) measure.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) measure.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return measure;
        }

        /// <summary>
        /// Adds a new (legacy) partition to the table and returns a reference to the partition.
        /// </summary>
        /// <param name="name">The name of the partition</param>
        /// <param name="query">The query expression to assign to the partition.</param>
        /// <returns>A reference to the newly added partition.</returns>
        [IntelliSense("Adds a new (legacy) partition to the table and returns a reference to the partition."), Tests.GenerateTest()]
        public Partition AddPartition(string name = null, string query = null)
        {
            Handler.BeginUpdate("add partition");
            var partition = Partition.CreateNew(this, name);
            if (!string.IsNullOrEmpty(query)) partition.Query = query;
            Handler.EndUpdate();
            return partition;
        }

        /// <summary>
        /// Adds a new M partition to the table and returns a reference to the partition.
        /// </summary>
        /// <param name="name">The name of the partition</param>
        /// <param name="expression">The M expression to assign to the partition.</param>
        /// <returns>A reference to the newly added partition.</returns>
        [IntelliSense("Adds a new M partition to the table and returns a reference to the partition."), Tests.GenerateTest(), Tests.CompatibilityLevel(1400)]
        public MPartition AddMPartition(string name = null, string expression = null)
        {
            Handler.BeginUpdate("add partition");
            var partition = MPartition.CreateNew(this, name);
            if(!string.IsNullOrEmpty(expression)) partition.Expression = expression;
            Handler.EndUpdate();
            return partition;
        }

        /// <summary>
        /// Adds a new calculated column to the table and returns a reference to the column.
        /// </summary>
        /// <param name="name">The name of the column</param>
        /// <param name="expression">DAX expression to assign to the column</param>
        /// <param name="displayFolder">Display Folder to assign to the column</param>
        /// <returns>A reference to the newly added column</returns>
        [IntelliSense("Adds a new calculated column to the table and returns a reference to the column."),Tests.GenerateTest()]
        public CalculatedColumn AddCalculatedColumn(string name = null, string expression = null, string displayFolder = null)
        {
            Handler.BeginUpdate("add calculated column");
            var column = CalculatedColumn.CreateNew(this, name);
            if (!string.IsNullOrEmpty(expression)) column.Expression = expression;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }

        /// <summary>
        /// Adds a new data column to the table and returns a reference to the column.
        /// </summary>
        /// <param name="name">The name of the column</param>
        /// <param name="sourceColumn">The name of the column in the source query</param>
        /// <param name="displayFolder">Display Folder to assign to the column</param>
        /// <param name="dataType">Data Type to assign to the column</param>
        /// <returns>A reference to the newly added column</returns>
        [IntelliSense("Adds a new data column to the table and returns a reference to the column."), Tests.GenerateTest()]
        public DataColumn AddDataColumn(string name = null, string sourceColumn = null, string displayFolder = null, DataType dataType = DataType.String)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(DataColumn)))
                throw new PowerBIGovernanceException("Adding columns to a table in this Power BI Model is not supported.");

            Handler.BeginUpdate("add Data column");
            var column = DataColumn.CreateNew(this, name);
            column.DataType = dataType;
            if (!string.IsNullOrEmpty(sourceColumn)) column.SourceColumn = sourceColumn;
            if (!string.IsNullOrEmpty(displayFolder)) column.DisplayFolder = displayFolder;
            Handler.EndUpdate();
            return column;
        }

        /// <summary>
        /// Adds a new hierarchy to the table and returns a reference to the hierarchy.
        /// </summary>
        /// <param name="name">Name of the hierarchy.</param>
        /// <param name="displayFolder">Display folder of the hierarchy.</param>
        /// <param name="levels">A list of columns to add as levels of the hierarchy</param>
        /// <returns></returns>
        [IntelliSense("Adds a new hierarchy to the table and returns a reference to the hierarchy."), Tests.GenerateTest()]
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

        /// <summary>
        /// Adds a new hierarchy to the table and returns a reference to the hierarchy.
        /// </summary>
        /// <param name="name">Name of the hierarchy.</param>
        /// <param name="displayFolder">Display folder of the hierarchy.</param>
        /// <param name="levels">A list of column names to add as levels of the hierarchy</param>
        /// <returns></returns>
        [IntelliSense("Adds a new hierarchy to the table and returns a reference to the hierarchy.")]
        public Hierarchy AddHierarchy(string name, string displayFolder, params string[] levels)
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

        Table IFolder.ParentTable { get { return this; } }

        /// <summary>
        /// Gets the name of the data source used by the table.
        /// </summary>
        [Category("Metadata"), IntelliSense("Gets the name of the data source used by the table.")]
        public string Source {
            get
            {
                var ds = (MetadataObject.Partitions.FirstOrDefault().Source as TOM.QueryPartitionSource)?.DataSource;
                string sourceName = null;
                if (ds != null) sourceName = (Handler.WrapperLookup[ds] as DataSource)?.Name;
                return sourceName ?? ds?.Name;
            }
        }

        /// <summary>
        /// Gets the type of the data source used by the table.
        /// </summary>
        [Category("Metadata"), DisplayName("Source Type"), IntelliSense("Gets the type of the data source used by the table.")]
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
                case Properties.DEFAULTDETAILROWSEXPRESSION: return Browsable(Properties.DEFAULTDETAILROWSDEFINITION);
                case Properties.OBJECTLEVELSECURITY:
                    return Handler.CompatibilityLevel >= 1400 && Model.Roles.Any();
                case Properties.ROWLEVELSECURITY:
                    return Model.Roles.Any();
                case nameof(RollingWindowPeriods):
                case nameof(RollingWindowGranularity):
                case nameof(IncrementalGranularity):
                case nameof(IncrementalPeriods):
                case nameof(IncrementalPeriodsOffset):
                case nameof(PollingExpression):
                case nameof(SourceExpression):
                    return MetadataObject.RefreshPolicy != null;
                case nameof(EnableRefreshPolicy):
                    return Handler.PbiMode;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Provides a convenient way to access the Row Level Filters assigned to this table across different roles.
        /// </summary>
        [Browsable(true), DisplayName("Row Level Security"), Category("Translations, Perspectives, Security"), IntelliSense("Provides a convenient way to access the Row Level Filters assigned to this table across different roles.")]
        public TableRLSIndexer RowLevelSecurity { get; private set; }

        /// <summary>
        /// Gets a string that may be used for referencing the table in a DAX expression.
        /// </summary>
        [Browsable(false), IntelliSense("Gets a string that may be used for referencing the table in a DAX expression.")]
        public string DaxObjectName
        {
            get
            {
                return string.Format("'{0}'", Name.Replace("'", "''"));
            }
        }

        /// <summary>
        /// Gets a string that may be used for referencing the table in a DAX expression.
        /// </summary>
        [Browsable(true), Category("Metadata"), DisplayName("DAX Identifier")]
        public string DaxObjectFullName
        {
            get
            {
                return DaxObjectName;
            }
        }

        /// <summary>
        /// Gets a string that may be used for referencing the table in a DAX expression.
        /// </summary>
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
        [IntelliSense("Returns all columns, measures and hierarchies inside this table.")]
        public virtual IEnumerable<ITabularNamedObject> GetChildren()
        {
            foreach (var m in Measures) yield return m;
            foreach (var c in Columns) yield return c;
            foreach (var h in Hierarchies) yield return h;
            yield break;
        }

        /// <summary>
        /// Returns all columns, measures and hierarchies at the root of the table (i.e. those that have an empty DisplayFolder string).
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IFolderObject> GetChildrenByFolders()
        {
            return FolderCache[""].GetChildrenByFolders();
        }

        static partial void InitMetadata(TOM.Table metadataObject, Model parent)
        {
            if (metadataObject.Partitions.Count > 0) return;

            var modelHasStructuredDataSources = parent.DataSources.Any(ds => ds.Type == DataSourceType.Structured);
            var modelHasPowerQueryPartitions = parent.Tables.Any(t => t.Partitions.Any(p => p.SourceType == PartitionSourceType.M));
            var modelHasProviderDataSources = parent.DataSources.Any(ds => ds.Type == DataSourceType.Provider);

            // Ensure the table is initialized with a single partition. If the model does not contain any Provider data sources, the type
            // of partition added to the table is determind by the following logic:
            if (parent.Handler.CompatibilityLevel >= 1400 && !modelHasProviderDataSources
                && (modelHasStructuredDataSources || modelHasPowerQueryPartitions || parent.Handler.Settings.UsePowerQueryPartitionsByDefault)
                )
            {
                var tomPartition = new TOM.Partition
                {
                    Name = metadataObject.Name,
                    Source = new TOM.MPartitionSource()
                };
                metadataObject.Partitions.Add(tomPartition);
            }
            else
            {
                var tomPartition = new TOM.Partition
                {
                    Name = metadataObject.Name
                };
                var qps = new TOM.QueryPartitionSource();
                tomPartition.Source = qps;
                if (!parent.DataSources.Any(ds => ds.Type == DataSourceType.Provider)) parent.AddDataSource();
                qps.DataSource = parent.DataSources.FirstOrDefault(ds => ds.Type == DataSourceType.Provider).MetadataObject;
                metadataObject.Partitions.Add(tomPartition);
            }
        }

        protected override void Init()
        {
            RowLevelSecurity = new TableRLSIndexer(this);

            if (Handler.CompatibilityLevel >= 1400)
            {
                ObjectLevelSecurity = new TableOLSIndexer(this);
            }

            base.Init();
        }

        private TableOLSIndexer _objectLevelSecurtiy;

        /// <summary>
        /// Provides a convenient way to get or set the Object-Level permissions assigned to this table across different roles.
        /// </summary>
        [DisplayName("Object Level Security"), Category("Translations, Perspectives, Security"), IntelliSense("Provides a convenient way to get or set the Object-Level permissions assigned to this table across different roles.")]
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

        private static readonly Dictionary<Type, DataType> DataTypeMapping =
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

        [Obsolete]
        [IntelliSense("DEPRECATED: Use SchemaCheck(table) instead.")]
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

        /// <summary>
        /// Gets the error message currently reported on this table.
        /// </summary>
        [Category("Metadata"),DisplayName("Error Message"), IntelliSense("Gets the error message currently reported on this table.")]
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
                //if(!(child is CalculationGroupAttribute)) Handler._errors.Add(child);
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

        /// <summary>
        /// A DAX expression specifying default detail rows for this table (drill-through in client tools).
        /// </summary>
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

                if (oldValue == value || (oldValue == null && value == string.Empty)) return;

                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.DEFAULTDETAILROWSEXPRESSION, value, ref undoable, ref cancel);
                if (cancel) return;

                if (MetadataObject.DefaultDetailRowsDefinition == null && !string.IsNullOrEmpty(value))
                    MetadataObject.DefaultDetailRowsDefinition = new TOM.DetailRowsDefinition();
                if (!string.IsNullOrEmpty(value))
                    MetadataObject.DefaultDetailRowsDefinition.Expression = value;
                if (string.IsNullOrWhiteSpace(value) && MetadataObject.DefaultDetailRowsDefinition != null)
                    MetadataObject.DefaultDetailRowsDefinition = null;

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

            private set
            {
                
            }
        }

        [Category("Refresh Policy"),Description("Target granularity of the rolling window for the whole dataset"),IntelliSense("Target granularity of the rolling window for the whole dataset")]
        public RefreshGranularityType RollingWindowGranularity
        {
            get => (RefreshGranularityType)((MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.RollingWindowGranularity ?? TOM.RefreshGranularityType.Invalid);
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.RollingWindowGranularity, (TOM.RefreshGranularityType)value, v => rp.RollingWindowGranularity = v);
            }
        }
        private bool ShouldSerializeRollingWindowGranularity() { return false; }

        [Category("Refresh Policy"), Description("Number of periods for the rolling window for the whole dataset"), IntelliSense("Number of periods for the rolling window for the whole dataset")]
        public int RollingWindowPeriods
        {
            get => (MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.RollingWindowPeriods ?? 0;
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.RollingWindowPeriods, value, v => rp.RollingWindowPeriods = v);
            }
        }
        private bool ShouldSerializeRollingWindowPeriods() { return false; }

        [Category("Refresh Policy"), Description("Granularity of the (most recent) incremental refresh range"), IntelliSense("Granularity of the (most recent) incremental refresh range")]
        public RefreshGranularityType IncrementalGranularity
        {
            get => (RefreshGranularityType)((MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.IncrementalGranularity ?? TOM.RefreshGranularityType.Invalid);
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.IncrementalGranularity, (TOM.RefreshGranularityType)value, v => rp.IncrementalGranularity = v);
            }
        }
        private bool ShouldSerializeIncrementalGranularity() { return false; }

        [Category("Refresh Policy"), Description("Number of periods for the incremental refresh range"), IntelliSense("Number of periods for the incremental refresh range")]
        public int IncrementalPeriods
        {
            get => (MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.IncrementalPeriods ?? 0;
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.IncrementalPeriods, value, v => rp.IncrementalPeriods = v);
            }
        }
        private bool ShouldSerializeIncrementalPeriods() { return false; }

        [Category("Refresh Policy"), Description("Lag or leading periods from Now() to the rolling window head"), IntelliSense("Lag or leading periods from Now() to the rolling window head")]
        public int IncrementalPeriodsOffset
        {
            get => (MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.IncrementalPeriodsOffset ?? 0;
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.IncrementalPeriodsOffset, value, v => rp.IncrementalPeriodsOffset = v);
            }
        }
        private bool ShouldSerializeIncrementalPeriodsOffset() { return false; }

        [Category("Refresh Policy"), Description("If not null, M expression that is evaluated and stored for each partition upon each refresh or merge operation. Result is stored in Partition.RefreshBookmark"),IntelliSense("If not null, M expression that is evaluated and stored for each partition upon each refresh or merge operation. Result is stored in Partition.RefreshBookmark")]
        public string PollingExpression
        {
            get => (MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.PollingExpression;
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.PollingExpression, value, v => rp.PollingExpression = v);
            }
        }
        private bool ShouldSerializePollingExpression() { return false; }

        [Category("Refresh Policy"), Description("M expression that is used as the source M expression for new partitions. It includes the date filter by referring to 2 M parameters named [RangeStart] and [RangeEnd], which can be dates or integers depending on IntegerDateKey below"), IntelliSense("M expression that is used as the source M expression for new partitions. It includes the date filter by referring to 2 M parameters named [RangeStart] and [RangeEnd], which can be dates or integers depending on IntegerDateKey below")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string SourceExpression
        {
            get => (MetadataObject.RefreshPolicy as TOM.BasicRefreshPolicy)?.SourceExpression;
            set
            {
                if (MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy rp)
                    SetValue(rp.SourceExpression, value, v => rp.SourceExpression = v);
            }
        }
        private bool ShouldSerializeSourceExpression() { return false; }

        [Category("Refresh Policy"), Description("Enable or disable incremental refresh policy for this table"), IntelliSense("Enable or disable incremental refresh policy for this table")]
        public bool EnableRefreshPolicy
        {
            get => MetadataObject.RefreshPolicy is TOM.BasicRefreshPolicy;
            set {
                SetValue(EnableRefreshPolicy, value, v => MetadataObject.RefreshPolicy = v ? new TOM.BasicRefreshPolicy() : null);
            }
        }
        private bool ShouldSerializeEnableRefreshPolicy() { return false; }

        /// <summary>
        /// Update partition definitions based on the current refresh policy using the specified Effective Date
        /// </summary>
        [IntelliSense("Update partition definitions based on the current refresh policy using the specified Effective Date")]
        public void ApplyRefreshPolicy(DateTime effectiveDate)
        {
            if (MetadataObject.RefreshPolicy != null)
            {
                if (Handler.HasUnsavedChanges) throw new NotSupportedException("You must save changes to your model metadata to the server, before you can apply a refresh policy.");
                MetadataObject.ApplyRefreshPolicy(effectiveDate, false);
                Reinit();
                Handler.UpdateVersion();
                Handler.Tree.OnStructureChanged(this.Partitions);
            }
            else
                throw new NotSupportedException("No refresh policy has been defined for this table.");
        }

        /// <summary>
        /// Update partition definitions based on the current refresh policy with Now() as Effective Date
        /// </summary>
        [IntelliSense("Update partition definitions based on the current refresh policy with Now() as Effective Date")]
        public void ApplyRefreshPolicy()
        {
            if (MetadataObject.RefreshPolicy != null)
            {
                if (Handler.HasUnsavedChanges) throw new NotSupportedException("You must save changes to your model metadata to the server, before you can apply a refresh policy.");
                MetadataObject.ApplyRefreshPolicy(false);
                Reinit();
                Handler.UpdateVersion();
                Handler.Tree.OnStructureChanged(this.Partitions);
            }
            else
                throw new NotSupportedException("No refresh policy has been defined for this table.");
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
        public static bool IsCalculated(this TOM.Table table)
        {
            var sourceType = GetSourceType(table);
            return sourceType == TOM.PartitionSourceType.Calculated;
        }
        public static bool IsImported(this TOM.Table table)
        {
            var sourceType = GetSourceType(table);
            return sourceType == TOM.PartitionSourceType.M || sourceType == TOM.PartitionSourceType.Query || sourceType == TOM.PartitionSourceType.PolicyRange;
        }
        public static bool IsImported(this Table table)
        {
            return table.MetadataObject.IsImported();
        }
    }
}
