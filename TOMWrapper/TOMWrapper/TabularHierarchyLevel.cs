using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper
{
    public class TabularHierarchyLevel : TabularNamedObject
    {
        public override void Clone(string newName, bool includeTranslations) { throw new InvalidOperationException("Cannot clone level."); }
        public override void Delete()
        {
            // TODO: Delete level
            throw new NotImplementedException();
        }

        public Level Level { get { return MetadataObject as Level; } }
        public override int Icon { get { return TabularIcons.ICON_LEVEL1 + Level.Ordinal; } }
        public override TabularObjectType Type { get { return TabularObjectType.Level; } }

        [Browsable(false)]
        public override bool Visible { get { return true; } }

        [NoCultureBrowsable, Category("Options")]
        public int Ordinal { get { return Level.Ordinal; } set { Level.Ordinal = value; } }
        public override bool InPerspective(Perspective perspective) { return true; }
        public override void SetPerspective(string perspectiveName, bool include) { }

        [Browsable(false)]
        public override IDictionary<string, bool> PerspectiveMembership { get { return null; } }
    }

}
