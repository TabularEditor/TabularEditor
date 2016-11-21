using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class CalculatedColumn
    {
        [IntelliSense("Deletes the Calculated Column from the table.")]
        public override void Delete()
        {
            InPerspective.None();
            base.Delete();
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.CalculatedColumn();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Expression") NeedsValidation = true;

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        [Browsable(false)]
        public bool NeedsValidation { get; set; }

        public TabularNamedObject CloneTo(Table table, string newName = null, bool includeTranslations = true)
        {
            Handler.BeginUpdate("duplicate calculated column");
            var tom = MetadataObject.Clone() as TOM.CalculatedColumn;
            tom.IsRemoved = false;
            tom.Name = Table.Columns.MetadataObjectCollection.GetNewName(string.IsNullOrEmpty(newName) ? tom.Name + " copy" : newName);
            var c = new CalculatedColumn(Handler, tom);
            Table.Columns.Add(c);

            if (includeTranslations)
            {
                c.TranslatedDescriptions.CopyFrom(TranslatedDescriptions);
                c.TranslatedDisplayFolders.CopyFrom(TranslatedDisplayFolders);
                if (string.IsNullOrEmpty(newName))
                    c.TranslatedNames.CopyFrom(TranslatedNames, n => n + " copy");
                else
                    c.TranslatedNames.CopyFrom(TranslatedNames, n => n.Replace(Name, newName));
            }
            c.InPerspective.CopyFrom(InPerspective);

            Handler.EndUpdate();

            return c;
        }

        public override TabularNamedObject Clone(string newName = null, bool includeTranslations = true)
        {
            return CloneTo(Table, newName, includeTranslations);
        }
    }
}
