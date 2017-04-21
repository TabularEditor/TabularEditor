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
            var perspective = new Perspective(this);
            if (!string.IsNullOrEmpty(name)) perspective.Name = name;
            Handler.EndUpdate();
            return perspective;
        }

        public CalculatedTable AddCalculatedTable()
        {
            Handler.BeginUpdate("add calculated table");
            var t = new CalculatedTable(this);
            Handler.EndUpdate();
            t.InitRLSIndexer();
            return t;
        }

        public Table AddTable()
        {
            Handler.BeginUpdate("add table");
            var t = new Table(this);
            var p = new Partition();
            t.Partitions.Add(p);
            t.InitRLSIndexer();
            
            Handler.EndUpdate();
            return t;
        }

        public SingleColumnRelationship AddRelationship()
        {
            Handler.BeginUpdate("add relationship");
            var rel = new SingleColumnRelationship(this);
            Handler.EndUpdate();
            return rel;
        }

        [IntelliSense("Adds a new translation to the model.")]
        public Culture AddTranslation(string cultureId)
        {
            Handler.BeginUpdate("add translation");
            var culture = new Culture(cultureId);
            Cultures.Add(culture);
            Handler.EndUpdate();
            return culture;
        }

        [IntelliSense("Adds a new security role to the model.")]
        public ModelRole AddRole(string name = null)
        {
            Handler.BeginUpdate("add role");
            var role = new ModelRole(this);
            role.InitRLSIndexer();
            if (!string.IsNullOrEmpty(name)) role.Name = name;
            Handler.EndUpdate();
            return role;
        }
        #endregion

        [Browsable(false),IntelliSense("The collection of tables in this model.")]
        public TableCollection Tables { get; private set; }

        [Category("Translations and Perspectives"),DisplayName("Model Perspectives")]
        [Editor(typeof(TabularEditor.PropertyGridUI.RefreshGridCollectionEditor),typeof(UITypeEditor)),TypeConverter(typeof(StringConverter))]
        public PerspectiveCollection Perspectives { get; private set; }

        [Category("Translations and Perspectives"), DisplayName("Model Cultures")]
        [Editor(typeof(TabularEditor.PropertyGridUI.CultureCollectionEditor),typeof(UITypeEditor)),TypeConverter(typeof(StringConverter))]
        public CultureCollection Cultures { get; private set; }

        [Category("Security"), DisplayName("Model Roles")]
        [Editor(typeof(TabularEditor.PropertyGridUI.RefreshGridCollectionEditor), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter))]
        public ModelRoleCollection Roles { get; private set; }

        [Browsable(false)]
        public RelationshipCollection2 Relationships { get; private set; }

        [Browsable(false)]
        public DataSourceCollection DataSources { get; private set; }

        public readonly LogicalGroup GroupTables = new LogicalGroup("Tables");
        public readonly LogicalGroup GroupDataSources = new LogicalGroup("Data Sources");
        public readonly LogicalGroup GroupPerspectives = new LogicalGroup("Perspectives");
        public readonly LogicalGroup GroupRelationships = new LogicalGroup("Relationships");
        public readonly LogicalGroup GroupTranslations = new LogicalGroup("Translations");
        public readonly LogicalGroup GroupRoles = new LogicalGroup("Roles");

        public IEnumerable<LogicalGroup> LogicalChildGroups { get
            {
                yield return GroupTables;
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

        protected override void Init()
        {
            _database = new Database(MetadataObject.Database);
        }

        private Database _database;
        [Category("Basic")]
        public Database Database
        {
            get { return _database; }
        }

        public void LoadChildObjects()
        {
            DataSources = new DataSourceCollection(Handler, "Model.DataSources", MetadataObject.DataSources, this);
            Perspectives = new PerspectiveCollection(Handler, "Model.Perspectives", MetadataObject.Perspectives, this);
            Cultures = new CultureCollection(Handler, "Model.Cultures", MetadataObject.Cultures, this);
            Tables = new TableCollection(Handler, "Model.Tables", MetadataObject.Tables, this);
            Relationships = new RelationshipCollection2(Handler, "Model.Relationships", MetadataObject.Relationships, this);
            Roles = new ModelRoleCollection(Handler, "Model.Roles", MetadataObject.Roles, this);

            Tables.ForEach(r => r.InitRLSIndexer());
            Roles.ForEach(r => r.InitRLSIndexer());

            Tables.CollectionChanged += Children_CollectionChanged;
            Perspectives.CollectionChanged += Children_CollectionChanged;
            Cultures.CollectionChanged += Children_CollectionChanged;
            Roles.CollectionChanged += Children_CollectionChanged;
            DataSources.CollectionChanged += Children_CollectionChanged;
            Relationships.CollectionChanged += Children_CollectionChanged;
            
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Handler.Tree.OnNodesInserted(this, e.NewItems.Cast<ITabularObject>());
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                Handler.Tree.OnNodesRemoved(this, e.OldItems.Cast<ITabularObject>());
        }
    }
}
