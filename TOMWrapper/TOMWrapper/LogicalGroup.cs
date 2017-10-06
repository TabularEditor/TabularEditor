using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;

namespace TabularEditor.TOMWrapper
{
    public class LogicalGroups: IEnumerable<LogicalGroup>
    {
        public static readonly LogicalGroups Singleton = new LogicalGroups();

        public readonly LogicalGroup Tables = new LogicalGroup("Tables");
        public readonly LogicalGroup DataSources = new LogicalGroup("Data Sources");
        public readonly LogicalGroup Perspectives = new LogicalGroup("Perspectives");
        public readonly LogicalGroup Relationships = new LogicalGroup("Relationships");
        public readonly LogicalGroup Translations = new LogicalGroup("Translations");
        public readonly LogicalGroup Roles = new LogicalGroup("Roles");
        public readonly LogicalGroup Partitions = new LogicalGroup("Table Partitions");

        private IEnumerable<LogicalGroup> Groups()
        {
            yield return Tables;
            yield return Partitions;
            yield return DataSources;
            yield return Perspectives;
            yield return Relationships;
            yield return Translations;
            yield return Roles;
        }

        public IEnumerator<LogicalGroup> GetEnumerator()
        {
#if CL1400
            if (TabularModelHandler.Singleton.UsePowerBIGovernance)
                return Groups().Where(grp => PowerBI.PowerBIGovernance.AllowGroup(grp.Name)).GetEnumerator();
#endif
            return Groups().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [TypeConverter(typeof(DynamicPropertyConverter))]
    public class LogicalGroup: ITabularNamedObject, ITabularObjectContainer, IDynamicPropertyObject
    {


        public const string TABLES = "Tables";
        public const string ROLES = "Roles";
        public const string PERSPECTIVES = "Perspectives";
        public const string TRANSLATIONS = "Translations";
        public const string RELATIONSHIPS = "Relationships";
        public const string DATASOURCES = "Data Sources";
        public const string TABLEPARTITIONS = "Table Partitions";

        [ReadOnly(true)]
        public string Name { get; set; }
        public TranslationIndexer TranslatedNames { get { return null; } }
        public ObjectType ObjectType { get { return ObjectType.Group; } }
        public Model Model { get { return TabularModelHandler.Singleton.Model; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            switch (Name)
            {
                case TABLES: return Model.Tables;
                case ROLES: return Model.Roles;
                case PERSPECTIVES: return Model.Perspectives;
                case TRANSLATIONS: return Model.Cultures;
                case RELATIONSHIPS: return Model.Relationships;
                case DATASOURCES: return Model.DataSources;
                case TABLEPARTITIONS: return Model.Tables.Where(t => !(t is CalculatedTable)).Select(t => t.PartitionViewTable);
            }
            return Enumerable.Empty<TabularNamedObject>();
        }



        public bool CanDelete()
        {
            return false;
        }

        public bool CanDelete(out string message)
        {
            message = Messages.CannotDeleteObject;
            return false;
        }

        public void Delete()
        {
            throw new NotSupportedException();
        }

        [Editor(typeof(TabularEditor.PropertyGridUI.ClonableObjectCollectionEditor<Perspective>), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter))]
        public PerspectiveCollection Perspectives { get { return Model.Perspectives; } }

        [Editor(typeof(TabularEditor.PropertyGridUI.CultureCollectionEditor), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter))]
        public CultureCollection Cultures { get { return Model.Cultures; } }

        [Editor(typeof(TabularEditor.PropertyGridUI.ClonableObjectCollectionEditor<ModelRole>), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter))]
        public ModelRoleCollection Roles { get { return Model.Roles; } }

        public bool Browsable(string propertyName)
        {
            switch(Name)
            {
                case PERSPECTIVES: return propertyName == "Perspectives";
                case TRANSLATIONS: return propertyName == "Cultures";
                case ROLES: return propertyName == "Roles";
                default:
                    return propertyName == "Name"; // For all other groups, only show the name.
            }
        }

        public bool Editable(string propertyName)
        {
            return false;
        }

        [Browsable(false)]
        public int MetadataIndex {
            get {
                return -1;
            }
        }

        public LogicalGroup(string name)
        {
            Name = name;
        }
    }
}
