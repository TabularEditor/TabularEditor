using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;
using System.Linq;

namespace TabularEditor.TOMWrapper
{

    public class TabularCalculatedColumn : TabularColumn
    {
        public override void Clone(string newName, bool includeTranslations)
        {
            var c = Column.Clone();
            c.Name = Table.Columns.GetNewName(newName);
            Table.Columns.Add(c);

            // Set perspective memberships of cloned object:
            foreach (var p in PerspectiveMembership) c.SetPerspective(p.Key, p.Value);

            // Optionally set translations:
            if (includeTranslations)
            {
                foreach (var n in NameTranslations.Where(n => !string.IsNullOrEmpty(n.Value))) c.SetName(Name != n.Value ? n.Value : c.Name, Model.Cultures[n.Key]);
                foreach (var n in DescriptionTranslations.Where(n => !string.IsNullOrEmpty(n.Value))) c.SetDescription(n.Value, Model.Cultures[n.Key]);
                foreach (var n in DisplayFolderTranslations.Where(n => !string.IsNullOrEmpty(n.Value))) c.SetDisplayFolder(n.Value, Model.Cultures[n.Key]);
            }
        }
        public override void Delete()
        {
            // TODO: Formula fixup, check for dependencies
            foreach (var p in PerspectiveMembership) SetPerspective(p.Key, false);
            Column.RemoveAllTranslations();
            Table.Columns.Remove(Column);
        }

        [Browsable(false)]
        protected new CalculatedColumn Column { get { return MetadataObject as CalculatedColumn; } }
        public override int Icon { get { return TabularIcons.ICON_CALCCOLUMN; } }
        public override TabularObjectType Type { get { return TabularObjectType.CalculatedColumn; } }

        [Browsable(false)]
        public string Expression { get { return Column.Expression; } set { Column.Expression = value; } }
    }

}
