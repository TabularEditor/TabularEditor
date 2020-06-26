using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class AlternateOf
    {
        public static AlternateOf CreateNew()
        {
            var tomAlternateOf = new TOM.AlternateOf();
            var obj = new AlternateOf(tomAlternateOf);
            obj.Init();
            return obj;
        }

        internal static AlternateOf CreateFromMetadata(Column parent, TOM.AlternateOf metadataObject)
        {
            var obj = new AlternateOf(metadataObject);
            parent.MetadataObject.AlternateOf = metadataObject;

            obj.Init();

            return obj;
        }

        [IntelliSense("Delete the AlternateOf")]
        public void Delete()
        {
            this.Column.AlternateOf = null;
        }

        public override string ObjectTypeName => "Alternate Of";

        internal override bool IsBrowsable(string propertyName)
        {
            if (propertyName == Properties.COLUMN || propertyName == Properties.OBJECTTYPE || propertyName == Properties.BASETABLE) return false;
            return base.IsBrowsable(propertyName);
        }
    }
}
