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
        [Browsable(false),IntelliSense("The collection of tables in this model.")]
        public TableCollection Tables { get; private set; }

        [Category("Translations and Perspectives"),DisplayName("Model Perspectives")]
        [Editor(typeof(TabularEditor.PropertyGridUI.RefreshGridCollectionEditor),typeof(UITypeEditor)),TypeConverter(typeof(StringConverter))]
        public PerspectiveCollection Perspectives { get; private set; }

        [Category("Translations and Perspectives"), DisplayName("Model Cultures")]
        [Editor(typeof(TabularEditor.PropertyGridUI.CultureCollectionEditor),typeof(UITypeEditor)),TypeConverter(typeof(StringConverter))]
        public CultureCollection Cultures { get; private set; }

        public IEnumerable<TabularNamedObject> GetChildren()
        {
            return Tables;
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
            Perspectives = new PerspectiveCollection(Handler, "Model.Perspectives", MetadataObject.Perspectives);
            Cultures = new CultureCollection(Handler, "Model.Cultures", MetadataObject.Cultures);
            Tables = new TableCollection(Handler, "Model.Tables", MetadataObject.Tables);

            Tables.CollectionChanged += Tables_CollectionChanged;

            
        }

        // TODO: Handle differently
        [TypeConverter(typeof(CollectionConverter))]
        public List<string> Relationships { get
            {
                return MetadataObject.Relationships.OfType<TOM.SingleColumnRelationship>()
                    .Select(r => string.Format("'{0}'[{1}] {4} '{2}'[{3}]{5}", 
                    r.FromTable.Name, r.FromColumn.Name, r.ToTable.Name, r.ToColumn.Name,
                    r.CrossFilteringBehavior == TOM.CrossFilteringBehavior.OneDirection ? "==>" : "<==>",
                    r.IsActive ? "" : " (inactive)"
                    )).ToList();
            }
        }

        private void Tables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Handler.Tree.OnNodesInserted(this, e.NewItems.Cast<ITabularObject>());
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                Handler.Tree.OnNodesRemoved(this, e.OldItems.Cast<ITabularObject>());
        }
    }
}
