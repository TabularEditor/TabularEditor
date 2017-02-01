using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public abstract partial class Column: ITabularPerspectiveObject, IDaxObject
    {
        public HashSet<IExpressionObject> Dependants { get; private set; } = new HashSet<IExpressionObject>();

        [Browsable(true),DisplayName("Perspectives"), Category("Translations and Perspectives")]
        public PerspectiveIndexer InPerspective { get; private set; }

        [IntelliSense("Deletes the Column from the table.")]
        public override void Delete()
        {
            InPerspective.None();
            base.Delete();
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = this is DataColumn ? (TOM.Column)new TOM.DataColumn() :
                    this is CalculatedColumn ? (TOM.Column)new TOM.CalculatedColumn() :
                    this is CalculatedTableColumn ? (TOM.Column)new TOM.CalculatedTableColumn() :
                    null;

            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        protected override void Init()
        {
            InPerspective = new PerspectiveColumnIndexer(this);
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == "Name") Handler.BuildDependencyTree();
            if(propertyName == "IsKey" && (bool)newValue == true)
            {
                // When the IsKey column is set to "true", all other columns must have their IsKey set to false.
                // This has to happen within one undo-batch, so the change can be perfectly restored.
                Handler.UndoManager.BeginBatch("key column");
                foreach(var col in Table.Columns.Where(c => c.IsKey))
                {
                    col.IsKey = false;
                }
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == "IsKey" && IsKey == true)
            {
                Handler.UndoManager.EndBatch();
            }
            if (propertyName == "Name" && Handler.AutoFixup) Handler.DoFixup(this, (string)newValue);
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        [Browsable(true), Category("Metadata"), DisplayName("DAX identifier")]
        public string DaxObjectFullName
        {
            get
            {
                return string.Format("{0}{1}", DaxTableName, DaxObjectName);
            }
        }

        [Browsable(false)]
        public string DaxObjectName
        {
            get
            {
                return string.Format("[{0}]", Name.Replace("]", "]]"));
            }
        }

        [Browsable(false)]
        public string DaxTableName
        {
            get { return Table.DaxTableName; }
        }

    }

    public partial class ColumnCollection
    {
        public override IEnumerator<Column> GetEnumerator()
        {
            return MetadataObjectCollection.Where(c => c.Type != Microsoft.AnalysisServices.Tabular.ColumnType.RowNumber).Select(c => Handler.WrapperLookup[c] as Column).GetEnumerator();
        }
    }
}
