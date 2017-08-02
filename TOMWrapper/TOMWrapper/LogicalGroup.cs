using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public class LogicalGroup: ITabularNamedObject, ITabularObjectContainer
    {
        [ReadOnly(true)]
        public string Name { get; set; }
        [Browsable(false)]
        public TranslationIndexer TranslatedNames { get { return null; } }
        public ObjectType ObjectType { get { return ObjectType.Group; } }
        [Browsable(false)]
        public Model Model { get { return TabularModelHandler.Singleton.Model; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            switch (Name)
            {
                case "Tables": return Model.Tables;
                case "Roles": return Model.Roles;
                case "Perspectives": return Model.Perspectives;
                case "Translations": return Model.Cultures;
                case "Relationships": return Model.Relationships;
                case "Data Sources": return Model.DataSources;
                case "Table Partitions": return Model.Tables.Where(t => !(t is CalculatedTable)).Select(t => t.PartitionViewTable);
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
