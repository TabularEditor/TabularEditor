using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Variation: IDynamicPropertyObject
    {

        public override void Delete()
        {
            base.Delete();
        }

        // TODO: Consider changing this into a generic pattern, and using it everywhere
        // It seems to be a better way to handle deletions of objects where the IsRemoved
        // property is set to TRUE on the metadataobject.
        public void RenewMetadataObject()
        {
            var tom = new TOM.Variation();
            Handler.WrapperLookup.Remove(MetadataObject);
            MetadataObject.CopyTo(tom);
            MetadataObject = tom;
            Handler.WrapperLookup.Add(MetadataObject, this);
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            RenewMetadataObject();

            base.Undelete(collection);
        }

        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "TranslatedNames":
                case "TranslatedDescriptions":
                    return false;
                default:
                    return true;
            }
        }

        public bool Editable(string propertyName)
        {
            return true;
        }

        public Variation() : base(TabularModelHandler.Singleton, new TOM.Variation() )
        {

        }
    }
}
