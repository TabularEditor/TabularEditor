using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    partial class Model: ITabularObjectContainer
    {
        #region Convenient methods
        [IntelliSense("Adds a new perspective to the model.")]
        public Perspective AddPerspective(string name = null)
        {
            Handler.BeginUpdate("add perspective");
            var perspective = Perspective.CreateNew(this, name);
            Handler.EndUpdate();
            return perspective;
        }

        public CalculatedTable AddCalculatedTable(string name = null, string expression = null)
        {
            Handler.BeginUpdate("add calculated table");
            var t = CalculatedTable.CreateNew(this, name, expression);
            Handler.EndUpdate();
            t.InitRLSIndexer();
            return t;
        }

        public Table AddTable(string name = null)
        {
            Handler.BeginUpdate("add table");
            var t = Table.CreateNew(this, name);
            t.InitRLSIndexer();
            
            Handler.EndUpdate();
            return t;
        }

        public SingleColumnRelationship AddRelationship()
        {
            Handler.BeginUpdate("add relationship");
            var rel = SingleColumnRelationship.CreateNew(this);
            Handler.EndUpdate();
            return rel;
        }

        [IntelliSense("Adds a new translation to the model.")]
        public Culture AddTranslation(string cultureId)
        {
            Handler.BeginUpdate("add translation");
            var culture = TOMWrapper.Culture.CreateNew(cultureId);
            Handler.EndUpdate();
            return culture;
        }

        [IntelliSense("Adds a new security role to the model.")]
        public ModelRole AddRole(string name = null)
        {
            Handler.BeginUpdate("add role");
            var role = ModelRole.CreateNew(this);
            //role.InitRLSIndexer();
            if (!string.IsNullOrEmpty(name)) role.Name = name;
            Handler.EndUpdate();
            return role;
        }

        public ProviderDataSource AddDataSource(string name = null)
        {
            Handler.BeginUpdate("add data source");
            var ds = ProviderDataSource.CreateNew(this, name);
            Handler.EndUpdate();
            return ds;

        }
        #endregion
        #region Convenient Collections
        /// <summary>
        /// Iterates all hierarchies on all tables of the model.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Hierarchy> AllHierarchies { get { return Tables.SelectMany(t => t.Hierarchies); } }

        /// <summary>
        /// Iterates all columns on all tables of the model.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Column> AllColumns { get { return Tables.SelectMany(t => t.Columns); } }

        /// <summary>
        /// Iterates all partitions on all tables of the model.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Partition> AllPartitions { get { return Tables.SelectMany(t => t.Partitions); } }

        /// <summary>
        /// Iterates all measures on all tables of the model.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Measure> AllMeasures { get { return Tables.SelectMany(t => t.Measures); } }

        /// <summary>
        /// Iterates all levels on all hierarchies on all tables of the model.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<Level> AllLevels { get { return Tables.SelectMany(t => t.Hierarchies).SelectMany(h => h.Levels); } }
        #endregion

        public readonly LogicalGroup GroupTables = new LogicalGroup("Tables");
        public readonly LogicalGroup GroupDataSources = new LogicalGroup("Data Sources");
        public readonly LogicalGroup GroupPerspectives = new LogicalGroup("Perspectives");
        public readonly LogicalGroup GroupRelationships = new LogicalGroup("Relationships");
        public readonly LogicalGroup GroupTranslations = new LogicalGroup("Translations");
        public readonly LogicalGroup GroupRoles = new LogicalGroup("Roles");
        public readonly LogicalGroup GroupPartitions = new LogicalGroup("Table Partitions");

        [Browsable(false)]
        public IEnumerable<LogicalGroup> LogicalChildGroups { get
            {
                yield return GroupTables;
                yield return GroupPartitions;
                yield return GroupDataSources;
                yield return GroupPerspectives;
                yield return GroupRelationships;
                yield return GroupTranslations;
                yield return GroupRoles;
            } }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return LogicalChildGroups.AsEnumerable().Cast<ITabularNamedObject>();
        }

        public override bool CanDelete(out string message)
        {
            message = Messages.CannotDeleteObject;
            return false;
        }

        protected override void Init()
        {
            
        }

        [Category("Basic")]
        public Database Database { get; internal set; }

        public void LoadChildObjects()
        {
            Tables.ForEach(r => r.InitRLSIndexer());
            Roles.ForEach(r => r.InitRLSIndexer());
        }
    }
}
