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
#if CL1400
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataColumn))) return null;
#endif
            Handler.BeginUpdate("add perspective");
            var perspective = Perspective.CreateNew(this, name);
            Handler.EndUpdate();
            return perspective;
        }

        [IntelliSense("Adds a new calculated table to the model.")]
        public CalculatedTable AddCalculatedTable(string name = null, string expression = null)
        {
            Handler.BeginUpdate("add calculated table");
            var t = CalculatedTable.CreateNew(this, name, expression);
            Handler.EndUpdate();
            t.InitRLSIndexer();
            return t;
        }

        [IntelliSense("Adds a new table to the model.")]
        public Table AddTable(string name = null)
        {
#if CL1400
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataColumn))) return null;
#endif
            Handler.BeginUpdate("add table");
            var t = Table.CreateNew(this, name);
            t.InitRLSIndexer();
            
            Handler.EndUpdate();
            return t;
        }

        [IntelliSense("Adds a new relationship table to the model.")]
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
#if CL1400
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataColumn))) return null;
#endif
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

        [IntelliSense("Adds a new data source to the model.")]
        public ProviderDataSource AddDataSource(string name = null)
        {
#if CL1400
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataSource))) return null;
#endif
            Handler.BeginUpdate("add data source");

            var ds = ProviderDataSource.CreateNew(this, name);
            Handler.EndUpdate();
            return ds;
        }

#if CL1400
        [IntelliSense("Adds a new strucured data source to the model.")]
        public StructuredDataSource AddStructuredDataSource(string name = null)
        {
            if (Handler.UsePowerBIGovernance && !PowerBI.PowerBIGovernance.AllowCreate(typeof(DataColumn))) return null;

            Handler.BeginUpdate("add data source");
            var ds = StructuredDataSource.CreateNew(this, name);
            Handler.EndUpdate();
            return ds;
        }
#endif
        #endregion
        #region Convenient Collections
        /// <summary>
        /// Iterates all hierarchies on all tables of the model.
        /// </summary>
        [Browsable(false),IntelliSense("A collection of every hierarchy across all tables in the model.")]
        public IEnumerable<Hierarchy> AllHierarchies { get { return Tables.SelectMany(t => t.Hierarchies); } }

        /// <summary>
        /// Iterates all columns on all tables of the model.
        /// </summary>
        [Browsable(false),IntelliSense("A collection of every column across all tables in the model.")]
        public IEnumerable<Column> AllColumns { get { return Tables.SelectMany(t => t.Columns); } }

        /// <summary>
        /// Iterates all partitions on all tables of the model.
        /// </summary>
        [Browsable(false), IntelliSense("A collection of every partition across all tables in the model.")]
        public IEnumerable<Partition> AllPartitions { get { return Tables.SelectMany(t => t.Partitions); } }

        /// <summary>
        /// Iterates all measures on all tables of the model.
        /// </summary>
        [Browsable(false), IntelliSense("A collection of every measure across all tables in the model.")]
        public IEnumerable<Measure> AllMeasures { get { return Tables.SelectMany(t => t.Measures); } }

        /// <summary>
        /// Iterates all levels on all hierarchies on all tables of the model.
        /// </summary>
        [Browsable(false), IntelliSense("A collection of every level in every hierarchy across all tables in the model.")]
        public IEnumerable<Level> AllLevels { get { return Tables.SelectMany(t => t.Hierarchies).SelectMany(h => h.Levels); } }
        #endregion

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return Groups;
        }

        protected override bool AllowDelete(out string message)
        {
            message = Messages.CannotDeleteObject;
            return false;
        }

        protected override void Init()
        {
            
        }

        [Browsable(false)]
        public LogicalGroups Groups { get { return LogicalGroups.Singleton; } }

        [Category("Basic")]
        [IntelliSense("Gets the database object of the model.")]
        public Database Database { get; internal set; }

        public void LoadChildObjects()
        {
            Tables.ForEach(r => r.InitRLSIndexer());
            Roles.ForEach(r => r.InitRLSIndexer());
        }
    }
}
