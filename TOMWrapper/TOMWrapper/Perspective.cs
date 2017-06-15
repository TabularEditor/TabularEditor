using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Perspective
    {
        /*public override TabularNamedObject Clone(string newName, bool includeTranslations)
        {
            Handler.BeginUpdate("duplicate perspective");
            var tom = MetadataObject.Clone();
            //tom.IsRemoved = false;
            tom.Name = Model.Perspectives.MetadataObjectCollection.GetNewName(string.IsNullOrEmpty(newName) ? tom.Name + " copy" : newName);
            var p = new Perspective(Handler, tom);
            Model.Perspectives.Add(p);

            if (includeTranslations)
            {
                p.TranslatedDescriptions.CopyFrom(TranslatedDescriptions);
                p.TranslatedDisplayFolders.CopyFrom(TranslatedDisplayFolders);
                if (string.IsNullOrEmpty(newName))
                    p.TranslatedNames.CopyFrom(TranslatedNames, n => n + " copy");
                else
                    p.TranslatedNames.CopyFrom(TranslatedNames, n => n.Replace(Name, newName));
            }

            Handler.EndUpdate();

            return p;
        }*/
    }
}
