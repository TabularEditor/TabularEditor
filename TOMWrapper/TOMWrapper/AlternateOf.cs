using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [ReadOnly(true)]
        public override string ObjectTypeName => "Alternate Of";

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);

            if(propertyName == Properties.BASETABLE && newValue != null)
            {
                BaseColumn = null;
                Summarization = SummarizationType.Count;
            }
            if (propertyName == Properties.BASECOLUMN && newValue != null)
            {
                BaseTable = null;
                if (Summarization == SummarizationType.Count)
                    Summarization = SummarizationType.Sum;
            }
            if (propertyName == Properties.SUMMARIZATION)
            {
                if(BaseColumn != null && (SummarizationType)newValue == SummarizationType.Count)
                {
                    BaseColumn = null;
                }
                else if (BaseTable != null && (SummarizationType)newValue != SummarizationType.Count)
                {
                    BaseTable = null;
                }
            }
        }

        internal override bool IsBrowsable(string propertyName)
        {
            if (propertyName == Properties.COLUMN || propertyName == Properties.OBJECTTYPE) return false;
            if (propertyName == Properties.BASETABLE) return Summarization == SummarizationType.Count;
            if (propertyName == Properties.BASECOLUMN) return Summarization != SummarizationType.Count;
            return base.IsBrowsable(propertyName);
        }

        internal override bool IsEditable(string propertyName)
        {
            if (propertyName == Properties.OBJECTTYPENAME) return false;
            return base.IsEditable(propertyName);
        }
    }
}
