using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper
{
    public class TabularHierarchy : TabularTableObject
    {
        public override void Clone(string newName, bool includeTranslations)
        {
            // TODO: Clone hierarchy
            throw new NotImplementedException();
        }
        public override void Delete()
        {
            // TODO: Delete hierarchy
            throw new NotImplementedException();
        }

        public override Table Table { get { return Hierarchy.Table; } }

        [Browsable(false)]
        public Hierarchy Hierarchy { get { return MetadataObject as Hierarchy; } }
        public override int Icon { get { return TabularIcons.ICON_HIERARCHY; } }
        public override TabularObjectType Type { get { return TabularObjectType.Hierarchy; } }
        public override string GetDisplayFolder(Culture culture)
        {
            return Hierarchy.GetDisplayFolder(culture);
        }
        public override void SetDisplayFolder(string folder, Culture culture)
        {
            Hierarchy.SetDisplayFolder(folder, culture);
        }

        public override bool Visible { get { return !Hierarchy.IsHidden; } set { Hierarchy.IsHidden = !value; } }
        public IEnumerable<TabularHierarchyLevel> GetLevels(TabularObjectCache cache)
        {
            return Hierarchy.Levels.Select(l => cache[l] as TabularHierarchyLevel);
        }
        public override bool InPerspective(Perspective perspective)
        {
            return Hierarchy.InPerspective(perspective);
        }
        public override void SetPerspective(string perspectiveName, bool inPerspective)
        {
            Hierarchy.SetPerspective(perspectiveName, inPerspective);
        }
    }

}
