using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class SingleColumnRelationship: IErrorMessageObject
    {
        internal override void ReapplyReferences()
        {
            base.ReapplyReferences();

            // Restore from/to columns, as the related object could have been recreated in the TOM in the meantime:
            if(MetadataObject.FromColumn != null)
                MetadataObject.FromColumn = Handler.Model.Tables[MetadataObject.FromTable.Name].Columns[MetadataObject.FromColumn.Name].MetadataObject;
            if(MetadataObject.ToColumn != null)
                MetadataObject.ToColumn = Handler.Model.Tables[MetadataObject.ToTable.Name].Columns[MetadataObject.ToColumn.Name].MetadataObject;
        }

        internal void UpdateName()
        {
            InternalName = string.Format("{0} {1} {2}", GetFullName(MetadataObject.FromColumn) ?? "(none)",
                this.CrossFilteringBehavior == CrossFilteringBehavior.OneDirection ? "-->" : "<-->",
                GetFullName(MetadataObject.ToColumn) ?? "(none)");
            Handler.UpdateObjectName(this);
        }

        public override string ToString()
        {
            return InternalName;
        }

        private string GetFullName(TOM.Column col)
        {
            if (col == null) return null;
            return string.Format("'{0}'[{1}]", col.Table.Name, col.Name);
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case "FromTable":
                case "ToTable":
                case "Type":
                    return false;
            }
            return true;
        }

        internal override bool IsEditable(string propertyName)
        {
            switch (propertyName)
            {
                case "Name": return false;
            }
            return true;
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(InternalName)) UpdateName();
                return InternalName;
            }

            set
            {
                // don't allow changing names of relationships.
            }
        }

        [Category("Metadata")]
        public string ErrorMessage
        {
            get
            {
                if (Model != null && Model.Relationships.Any(r => r != this && (
                     (r.FromColumn == FromColumn && r.ToColumn == ToColumn) ||
                     (r.ToColumn == FromColumn && r.FromColumn == ToColumn))))
                    return "More than one relationship exists between the same two columns.";
                else
                    return null;
            }
        }

        private string InternalName = null;

        [Category("Basic")]
        public string ID { get { return MetadataObject.Name; } }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            switch (propertyName)
            {
                case "ToColumn": if (newValue == FromColumn && newValue != null) cancel = true; return;
                case "FromColumn": if (newValue == ToColumn && newValue != null) cancel = true; return;
            }

            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch (propertyName)
            {
                case "ToColumn":
                case "FromColumn":
                case "CrossFilteringBehavior":
                case "IsActive":
                    // Force an update of the relationship in the Explorer Tree, as the name string may have changed,
                    // if any of the properties above are changed:
                    UpdateName();
                    break;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
    }
}