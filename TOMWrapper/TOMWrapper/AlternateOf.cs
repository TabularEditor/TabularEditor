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

        private bool suspendChange = false;

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
            if (!Handler.UndoManager.UndoInProgress && !suspendChange)
            {
                switch (propertyName)
                {
                    case Properties.BASETABLE:
                    case Properties.BASECOLUMN:
                    case Properties.SUMMARIZATION:
                        Handler.BeginUpdate($"Set Property '{propertyName.SplitCamelCase()}'");
                        break;
                }
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);
            if (!Handler.UndoManager.UndoInProgress && !suspendChange)
            {
                suspendChange = true;

                switch (propertyName)
                {
                    case Properties.BASETABLE:
                        if (newValue != null)
                        {
                            BaseColumn = null;
                            Summarization = SummarizationType.Count;
                        }
                        Handler.EndUpdate();
                        break;
                    case Properties.BASECOLUMN:
                        if (newValue != null)
                        {
                            BaseTable = null;
                        }
                        Handler.EndUpdate();
                        break;
                    case Properties.SUMMARIZATION:
                        if (BaseTable != null && (SummarizationType)newValue != SummarizationType.Count)
                        {
                            BaseTable = null;
                        }
                        Handler.EndUpdate();
                        break;
                }

                suspendChange = false;
            }
        }

        internal override bool IsBrowsable(string propertyName)
        {
            if (propertyName == Properties.COLUMN || propertyName == Properties.OBJECTTYPE) return false;
            if (propertyName == Properties.BASETABLE) return Summarization == SummarizationType.Count;
            if (propertyName == Properties.BASECOLUMN) return true;
            return base.IsBrowsable(propertyName);
        }

        internal override bool IsEditable(string propertyName)
        {
            if (propertyName == Properties.OBJECTTYPENAME) return false;
            return base.IsEditable(propertyName);
        }
    }
}
