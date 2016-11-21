using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;

namespace TabularEditor.TOMWrapper
{
    

    public class TabularModel : TabularNamedObject
    {
        public IEnumerable<TabularTable> Tables { get; private set; }

        public TabularModel(Model model)
        {
            MetadataObject = model;
            Tables = model.Tables.Select(t => new TabularTable(t)).ToList();
        }

        [NoCultureBrowsable,DisplayName("Database Name"),Category("Basic")]
        public string DatabaseName { get { return Model.Database.Name; } set { Model.Database.Name = value; } }
        [NoCultureBrowsable, DisplayName("Database ID"), Category("Basic")]
        public string DatabaseID { get { return Model.Database.ID; } set {
                if (DatabaseName == DatabaseID) DatabaseName = value;
                Model.Database.ID = value;
            } }

        public override void Clone(string newName, bool includeTranslations) { throw new InvalidOperationException("Cannot clone model."); }
        public override void Delete() { throw new InvalidOperationException("Cannot delete model."); }

        public new Model Model { get { return MetadataObject as Model; } }
        [DisplayName("Model Name")]
        public override string Name { get { return Model.Name; }
            set { Model.Name = value; }
        } 
        public override int Icon { get { return TabularIcons.ICON_CUBE; } }
        public override TabularObjectType Type { get { return TabularObjectType.Model; } }

        #region Irrelevant base class properties
        public override bool InPerspective(Perspective perspective) { return true; }
        public override void SetPerspective(string perspectiveName, bool include) { }

        [Browsable(false)]
        public override bool Visible { get { return true; } }
        [Browsable(false)]
        public override string LocalName { get { return Name; } set { Name = value; } }
        [Browsable(false)]
        public override string LocalDescription { get { return null; } }
        [Browsable(false)]
        public override IDictionary<string, bool> PerspectiveMembership { get { return null; } }
        [Browsable(false)]
        public override IDictionary<string, string> DescriptionTranslations { get { return null; } }
        [Browsable(false)]
        public override IDictionary<string, string> NameTranslations { get { return null; } }
        #endregion
    }
}
