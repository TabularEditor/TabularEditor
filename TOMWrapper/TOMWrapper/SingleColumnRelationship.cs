using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class SingleColumnRelationship: IDynamicPropertyObject
    {
        public override void Delete()
        {
            if (Collection != null) Collection.Remove(this);
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.SingleColumnRelationship();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            // Restore from/to columns, as the related object could have been recreated in the TOM in the meantime:
            MetadataObject.FromColumn = Handler.Model.Tables[MetadataObject.FromTable.Name].Columns[MetadataObject.FromColumn.Name].MetadataObject;
            MetadataObject.ToColumn = Handler.Model.Tables[MetadataObject.ToTable.Name].Columns[MetadataObject.ToColumn.Name].MetadataObject;

            base.Undelete(collection);
        }

        protected override void Init()
        {
            UpdateName();
        }

        private void UpdateName()
        {
            InternalName = string.Format("{0} {1} {2}", GetFullName(MetadataObject.FromColumn) ?? "(none)",
                this.CrossFilteringBehavior == Microsoft.AnalysisServices.Tabular.CrossFilteringBehavior.OneDirection ? "-->" : "<-->",
                GetFullName(MetadataObject.ToColumn) ?? "(none)");
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

        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "FromTable": return false;
                case "ToTable": return false;
                case "TranslatedNames":
                case "TranslatedDisplayFolders":
                case "TranslatedDescriptions":
                    return false;
            }
            return true;
        }

        public bool Editable(string propertyName)
        {
            switch (propertyName)
            {
                case "Name": return false;
                case "FromCardinality": return false;
                case "ToCardinality": return false;
            }
            return true;
        }

        public override string Name
        {
            get
            {
                return InternalName;
            }

            set
            {
                // don't allow changing names of relationships.
            }
        }

        private string InternalName;

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            switch(propertyName)
            {
                case "ToColumn": if (newValue == FromColumn && newValue != null) cancel = true; break;
                case "FromColumn": if (newValue == ToColumn && newValue != null) cancel = true; break;
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch(propertyName)
            {
                case "ToColumn":
                case "FromColumn":
                case "CrossFilteringBehavior":
                case "IsActive":
                    // Force an update of the relationship in the Explorer Tree, as the name string may have changed,
                    // if any of the properties above are changed:
                    UpdateName();
                    Handler.UpdateObject(this);
                    break;
            }
        }
    }
}
