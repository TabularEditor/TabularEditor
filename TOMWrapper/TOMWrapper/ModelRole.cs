using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class ModelRole
    {
        [Browsable(true), DisplayName("Row Level Filters"), Category("Security")]
        public RoleRLSIndexer RowLevelSecurity { get; private set; }
        
        public void InitRLSIndexer()
        {
            RowLevelSecurity = new RoleRLSIndexer(this);
        }

        public override TabularNamedObject Clone(string newName, bool includeTranslations)
        {
            Handler.BeginUpdate("duplicate role");
            var tom = MetadataObject.Clone();
            tom.IsRemoved = false;
            tom.Name = Model.Roles.MetadataObjectCollection.GetNewName(string.IsNullOrEmpty(newName) ? tom.Name + " copy" : newName);
            var r = new ModelRole(Handler, tom);
            r.InitRLSIndexer();
            Model.Roles.Add(r);

            if (includeTranslations)
            {
                r.TranslatedDescriptions.CopyFrom(TranslatedDescriptions);
                r.TranslatedDisplayFolders.CopyFrom(TranslatedDisplayFolders);
                if (string.IsNullOrEmpty(newName))
                    r.TranslatedNames.CopyFrom(TranslatedNames, n => n + " copy");
                else
                    r.TranslatedNames.CopyFrom(TranslatedNames, n => n.Replace(Name, newName));
            }

            Handler.EndUpdate();

            return r;
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.ModelRole();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }

        public override void Delete()
        {
            base.Delete();
        }

    }
}
