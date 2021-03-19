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
    public sealed class LogicalGroups: IEnumerable<LogicalGroup>
    {
        public static readonly LogicalGroups Singleton = new LogicalGroups();

        public const string TABLES = "Tables";
        public const string ROLES = "Roles";
        public const string PERSPECTIVES = "Perspectives";
        public const string TRANSLATIONS = "Translations";
        public const string RELATIONSHIPS = "Relationships";
        public const string DATASOURCES = "Data Sources";
        public const string EXPRESSIONS = "Shared Expressions";
        //public const string CALCULATIONGROUPS = "Calculation Groups";

        public readonly LogicalGroup DataSources = new LogicalGroup(DATASOURCES);
        public readonly LogicalGroup Perspectives = new LogicalGroup(PERSPECTIVES);
        public readonly LogicalGroup Relationships = new LogicalGroup(RELATIONSHIPS);
        public readonly LogicalGroup Roles = new LogicalGroup(ROLES);
        public readonly LogicalGroup Expressions = new LogicalGroup(EXPRESSIONS);
        public readonly LogicalGroup Tables = new LogicalGroup(TABLES);
        //public readonly LogicalGroup CalculationGroups = new LogicalGroup(CALCULATIONGROUPS);
        public readonly LogicalGroup Translations = new LogicalGroup(TRANSLATIONS);

        private IEnumerable<LogicalGroup> Groups()
        {
            yield return DataSources;
            yield return Perspectives;
            yield return Relationships;
            yield return Roles;
            if(TabularModelHandler.Singleton.CompatibilityLevel >= 1400) yield return Expressions;
            yield return Tables;
            yield return Translations;
        }

        public IEnumerator<LogicalGroup> GetEnumerator()
        {
            return Groups().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [TypeConverter(typeof(DynamicPropertyConverter))]
    public sealed class LogicalGroup: ITabularNamedObject, ITabularObjectContainer, IDynamicPropertyObject
    {
        bool ITabularNamedObject.CanEditName() { return false; }

        private string _name;

        [ReadOnly(true), Category("Basic")]
        public string Name { get => _name; set { } }
        public TranslationIndexer TranslatedNames { get { return null; } }
        public ObjectType ObjectType { get { return ObjectType.Group; } }
        public Model Model { get { return TabularModelHandler.Singleton.Model; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsRemoved => false;

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            switch (Name)
            {
                case LogicalGroups.TABLES: return Model.Tables;
                case LogicalGroups.ROLES: return Model.Roles;
                case LogicalGroups.EXPRESSIONS: return Model.Expressions;
                case LogicalGroups.PERSPECTIVES: return Model.Perspectives;
                case LogicalGroups.TRANSLATIONS: return Model.Cultures;
                case LogicalGroups.RELATIONSHIPS: return Model.Relationships;
                case LogicalGroups.DATASOURCES: return Model.DataSources;
                //case LogicalGroups.CALCULATIONGROUPS: return Model.Tables.OfType<CalculationGroupTable>();
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

        [Editor(typeof(TabularEditor.PropertyGridUI.ClonableObjectCollectionEditor<Perspective>), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter)), Category("Basic")]
        public PerspectiveCollection Perspectives { get { return Model.Perspectives; } }

        [Editor(typeof(TabularEditor.PropertyGridUI.CultureCollectionEditor), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter)), Category("Basic")]
        public CultureCollection Cultures { get { return Model.Cultures; } }

        [Editor(typeof(TabularEditor.PropertyGridUI.ClonableObjectCollectionEditor<ModelRole>), typeof(UITypeEditor)), TypeConverter(typeof(StringConverter)), Category("Basic")]
        public ModelRoleCollection Roles { get { return Model.Roles; } }

        public bool Browsable(string propertyName)
        {
            switch(Name)
            {
                case LogicalGroups.PERSPECTIVES: return propertyName == "Perspectives";
                case LogicalGroups.TRANSLATIONS: return propertyName == "Cultures";
                case LogicalGroups.ROLES: return propertyName == "Roles";
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

        internal LogicalGroup(string name)
        {
            this._name = name;
        }
    }
}
