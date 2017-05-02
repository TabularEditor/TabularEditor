using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Level: ITabularTableObject
    {
        Hierarchy hierarchyRef;

        /// <summary>
        /// Deletes the level from the hierarchy.
        /// </summary>
        [IntelliSense("Deletes the level from the hierarchy.")]
        public override void Delete()
        {
            hierarchyRef = Hierarchy;

            base.Delete();

            hierarchyRef.CompactLevelOrdinals();
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            // Since the original column could have been deleted since the level was deleted, let's find the column by name:
            var c = (collection as LevelCollection).Parent.Table.Columns[MetadataObject.Column.Name].MetadataObject;

            var tom = new TOM.Level() { Column = c, Name = MetadataObject.Name, Ordinal = MetadataObject.Ordinal };
            //MetadataObject.CopyTo(tom);
            ////tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(hierarchyRef.Levels);

            Hierarchy.FixLevelOrder(this, this.Ordinal);
        }

        public Level(Column column, Hierarchy hierarchy, string name) : 
            this(new TOM.Level()
            {
                Name = name ?? hierarchy.Levels.MetadataObjectCollection.GetNewName(column.Name),
                Column = column.MetadataObject
            })
        {
            hierarchy.Levels.Add(this);
        }

        [Browsable(false)]
        public Table Table
        {
            get
            {
                return Hierarchy.Table;
            }
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == "Ordinal")
            {
                // No automatic handling of Ordinal changes. We will handle it manually in the hierarchy's FixLevelOrder() method.
                cancel = true;
                this.MetadataObject.Ordinal = (int)newValue;
                Hierarchy.FixLevelOrder(this, (int)newValue);
            }
            if (propertyName == "Column")
            {
                if (newValue == null) throw new ArgumentNullException("Column");
                if (Hierarchy.Levels.Where(l => l != this).Any(l => l.Column == newValue))
                    throw new ArgumentException(string.Format("Another level in this hierarchy is already based on column \"{0}\"", (newValue as Column).Name), "Column");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Column")
            {
                Handler.UpdateObject(this);
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
    }

    public partial class LevelCollection
    {
        public override bool Remove(Level item)
        {
            var result = base.Remove(item);
            Handler.UpdateLevels(Parent);
            return result;
        }

        public override void Add(Level item)
        {
            base.Add(item);
            Handler.UpdateLevels(Parent);
        }
    }
}
