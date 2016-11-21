using System.Linq;
using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper
{
    public class TabularMeasure : TabularTableObject
    {
        public override void Clone(string newName, bool includeTranslations)
        {
            var m = Measure.Clone();
            m.Name = Table.Measures.GetNewName(newName);
            Table.Measures.Add(m);

            // Set perspective memberships of cloned object:
            foreach (var p in PerspectiveMembership) m.SetPerspective(p.Key, p.Value);

            // Optionally set translations:
            if (includeTranslations)
            {
                foreach (var n in NameTranslations.Where(n => !string.IsNullOrEmpty(n.Value))) m.SetName(Name != n.Value ? n.Value : m.Name, Model.Cultures[n.Key]);
                foreach (var n in DescriptionTranslations.Where(n => !string.IsNullOrEmpty(n.Value))) m.SetDescription(n.Value, Model.Cultures[n.Key]);
                foreach (var n in DisplayFolderTranslations.Where(n => !string.IsNullOrEmpty(n.Value))) m.SetDisplayFolder(n.Value, Model.Cultures[n.Key]);
            }
        }
        public override void Delete()
        {
            // TODO: Formula fix-up - check for dependencies
            foreach (var p in PerspectiveMembership) SetPerspective(p.Key, false);
            Measure.RemoveAllTranslations();
            Table.Measures.Remove(Measure);
        }

        public override string GetTooltipText()
        {
            return Measure.ErrorMessage.Replace("\n", "");
        }
        public override Table Table { get { return Measure.Table; } }

        [Browsable(false)]
        public Measure Measure { get { return MetadataObject as Measure; } }
        public override int Icon { get { return TabularIcons.ICON_MEASURE; } }
        public override TabularObjectType Type { get { return TabularObjectType.Measure; } }
        public override string GetDisplayFolder(Culture culture)
        {
            return Measure.GetDisplayFolder(culture);
        }
        public override void SetDisplayFolder(string folder, Culture culture)
        {
            Measure.SetDisplayFolder(folder, culture);
        }
        public override bool Visible { get { return !Measure.IsHidden; } set { Measure.IsHidden = !value; } }

        public bool ShouldSerializeFormatString() { return false; }
        [NoCultureBrowsable, Category("Options"), DisplayName("Format String"), MultiSelectBrowsable]
        public string FormatString { get { return Measure.FormatString; } set { Measure.FormatString = value; } }

        [Browsable(false)]
        public string Expression { get { return Measure.Expression; } set { Measure.Expression = value; } }
        public override bool InPerspective(Perspective perspective)
        {
            return Measure.InPerspective(perspective);
        }
        public override void SetPerspective(string perspectiveName, bool inPerspective)
        {
            Measure.SetPerspective(perspectiveName, inPerspective);
        }
    }

}
