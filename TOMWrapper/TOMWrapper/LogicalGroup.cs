using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    // TODO: Create a special grouping for Partitions on tables.

    public class LogicalGroup: ITabularNamedObject, ITabularObjectContainer
    {
        [ReadOnly(true)]
        public string Name { get; set; }
        [Browsable(false)]
        public TranslationIndexer TranslatedNames { get { return null; } }
        public ObjectType ObjectType { get { return ObjectType.Group; } }
        [Browsable(false)]
        public Model Model { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return Enumerable.Empty<ITabularNamedObject>();
        }


        public LogicalGroup(string name)
        {
            Name = name;
        }
    }
}
