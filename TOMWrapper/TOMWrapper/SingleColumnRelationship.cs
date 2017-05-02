using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class SingleColumnRelationship : IDynamicPropertyObject
    {
        public override void Delete()
        {
            if (Collection != null) Collection.Remove(this);
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.SingleColumnRelationship();
            MetadataObject.CopyTo(tom);
            //tom.IsRemoved = false;
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
            switch (propertyName)
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
            switch (propertyName)
            {
                case "ToColumn": if (newValue == FromColumn && newValue != null) cancel = true; break;
                case "FromColumn": if (newValue == ToColumn && newValue != null) cancel = true; break;
            }
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
                    Handler.UpdateObject(this);
                    break;
            }
        }
    }

    public partial class RelationshipCollection2 : TabularObjectCollection<SingleColumnRelationship, TOM.Relationship, TOM.Model>
    {
        public Model Parent { get; private set; }

        public RelationshipCollection2(string collectionName, TOM.RelationshipCollection metadataObjectCollection, Model parent) : base(collectionName, metadataObjectCollection)
        {
            Parent = parent;

            // Construct child objects (they are automatically added to the Handler's WrapperLookup dictionary):
            foreach (var obj in MetadataObjectCollection)
            {
                switch ((obj as TOM.Relationship).Type)
                {
                    case TOM.RelationshipType.SingleColumn: new SingleColumnRelationship(obj as TOM.SingleColumnRelationship) { Collection = this }; break;
                }
            }
        }

        [Description("Sets the IsActive property of all objects in the collection at once.")]
        public bool IsActive
        {
            set
            {
                if (Handler == null) return;
                Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("IsActive"));
                this.ToList().ForEach(item => { item.IsActive = value; });
                Handler.UndoManager.EndBatch();
            }
        }
        [Description("Sets the CrossFilteringBehavior property of all objects in the collection at once.")]
        public TOM.CrossFilteringBehavior CrossFilteringBehavior
        {
            set
            {
                if (Handler == null) return;
                Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("CrossFilteringBehavior"));
                this.ToList().ForEach(item => { item.CrossFilteringBehavior = value; });
                Handler.UndoManager.EndBatch();
            }
        }
        [Description("Sets the JoinOnDateBehavior property of all objects in the collection at once.")]
        public TOM.DateTimeRelationshipBehavior JoinOnDateBehavior
        {
            set
            {
                if (Handler == null) return;
                Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("JoinOnDateBehavior"));
                this.ToList().ForEach(item => { item.JoinOnDateBehavior = value; });
                Handler.UndoManager.EndBatch();
            }
        }
        [Description("Sets the RelyOnReferentialIntegrity property of all objects in the collection at once.")]
        public bool RelyOnReferentialIntegrity
        {
            set
            {
                if (Handler == null) return;
                Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("RelyOnReferentialIntegrity"));
                this.ToList().ForEach(item => { item.RelyOnReferentialIntegrity = value; });
                Handler.UndoManager.EndBatch();
            }
        }
        [Description("Sets the SecurityFilteringBehavior property of all objects in the collection at once.")]
        public TOM.SecurityFilteringBehavior SecurityFilteringBehavior
        {
            set
            {
                if (Handler == null) return;
                Handler.UndoManager.BeginBatch(UndoPropertyChangedAction.GetActionNameFromProperty("SecurityFilteringBehavior"));
                this.ToList().ForEach(item => { item.SecurityFilteringBehavior = value; });
                Handler.UndoManager.EndBatch();
            }
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", Count, (Count == 1 ? "Relationship" : "Relationships").ToLower());
        }
    }

}