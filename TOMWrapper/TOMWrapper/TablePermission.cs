using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{

    public partial class TablePermission : IDaxDependantObject, IExpressionObject
    {
        public bool NoEffect
        {
            get
            {
                if(Handler.CompatibilityLevel >= 1400)
                {
                    if (
                        MetadataPermission != TOM.MetadataPermission.Default ||
                        ColumnPermissions.Any(cp => cp != TOM.MetadataPermission.Default)
                    ) return false;
                }
                if (!string.IsNullOrWhiteSpace(FilterExpression)) return false;
                return true;
            }
        }

        private DependsOnList _dependsOn = null;

        [Browsable(false)]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        [Browsable(true), Category("Metadata"),DisplayName("Table")]
        public string TableName => Table.Name;
        [Browsable(true), Category("Metadata"),DisplayName("Role")]
        public string RoleName => Role.Name;

        [Browsable(false)]
        public bool NeedsValidation { get; set; } = false;
        string IExpressionObject.Expression { get => FilterExpression; set => FilterExpression = value; }
        string ITabularNamedObject.Name { get => Table.Name; set { } }

        int ITabularNamedObject.MetadataIndex => Role.MetadataObject.TablePermissions.IndexOf(MetadataObject);

        internal static TablePermission CreateFromMetadata(TOM.TablePermission metadataObject)
        {
            var obj = new TablePermission(metadataObject);
            obj.Init();
            return obj;
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == Properties.EXPRESSION || propertyName == Properties.FILTEREXPRESSION)
            {
                NeedsValidation = true;
                FormulaFixup.BuildDependencyTree(this);
            }
        }

        protected override void Init()
        {
            ColumnPermissions = new RoleColumnOLSIndexer(this);
        }

        [Browsable(true), DisplayName("OLS Column Permissions"), Category("Security")]
        public RoleColumnOLSIndexer ColumnPermissions { get; private set; }

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.FILTEREXPRESSION:
                case Properties.ANNOTATIONS:
                case Properties.EXTENDEDPROPERTIES:
                case Properties.ERRORMESSAGE:
                case Properties.STATE:
                case "TableName":
                case "RoleName":
                    return true;
                case Properties.METADATAPERMISSION:
                case Properties.COLUMNPERMISSIONS:
                    return Handler.CompatibilityLevel >= 1400;
                default:
                    return false;
            }
        }
    }

    public partial class TablePermissionCollection
    {
        public TablePermission this[Table table]
        {
            get
            {
                return this[table.Name];
            }
        }
    }

    /*
    public class RLSFilterExpression: TabularObject, IDaxDependantObject, ITabularNamedObject, IExpressionObject, IErrorMessageObject, IExtendedPropertyObject, IAnnotationObject, IInternalAnnotationObject
    {
        public ModelRole Role;
        public Table Table;

        internal override void RenewMetadataObject()
        {
            var tp = MetadataObject.Clone();
            throw new NotImplementedException();
        }

        static internal RLSFilterExpression CreateFromMetadata(TOM.TablePermission tablePermission)
        {
            var result = new RLSFilterExpression();
            result.MetadataObject = tablePermission;
        }

        private RLSFilterExpression(TOM.TablePermission)
        {

        }

        static internal RLSFilterExpression Get(ModelRole role, Table table)
        {
            RLSFilterExpression result;
            if (!role.RowLevelSecurity.FilterExpressions.TryGetValue(table, out result)) {
                result = new RLSFilterExpression(role, table);
                role.RowLevelSecurity._filterExpressions.Add(table, result);
            }
            return result;
        }

        bool ITabularNamedObject.CanDelete()
        {
            return true;
        }

        bool ITabularNamedObject.CanEditName()
        {
            return false;
        }

        bool ITabularNamedObject.CanDelete(out string message)
        {
            message = null;
            return true;
        }

        void ITabularNamedObject.Delete()
        {

            throw new NotImplementedException();
        }

        private DependsOnList _dependsOn = null;

        public event PropertyChangedEventHandler PropertyChanged;

        [Browsable(false)]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        public bool IsRemoved => false;

        public int MetadataIndex => -1;

        public Model Model => Table.Model;

        public ObjectType ObjectType => ObjectType.RLSFilterExpression;

        string ITabularNamedObject.Name { get => Table.Name; set { } }

        int ITabularNamedObject.MetadataIndex => Role.RowLevelSecurity.FilterExpressions.Keys.Count(k => string.Compare(k.Name, Table.Name, true) < 0);

        bool ITabularObject.IsRemoved => false;

        public bool NeedsValidation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Expression { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ErrorMessage => MetadataObject.ErrorMessage;

        internal new TOM.TablePermission MetadataObject
        {
            get { return base.MetadataObject as TOM.TablePermission; }
            set { base.MetadataObject = value; }
        }

        ///<summary>The collection of Annotations on the current Measure.</summary>
        [Browsable(true), NoMultiselect, Category("Translations and Perspectives"), Description("The collection of Annotations on the current Measure."), Editor(typeof(AnnotationCollectionEditor), typeof(UITypeEditor))]
        public AnnotationCollection Annotations { get; private set; }
        ///<summary>Gets the value of the annotation with the given index, assuming it exists.</summary>
        [IntelliSense("Gets the value of the annotation with the given index, assuming it exists.")]
        public string GetAnnotation(int index)
        {
            return MetadataObject.Annotations[index].Value;
        }
        ///<summary>Returns true if an annotation with the given name exists. Otherwise false.</summary>
        [IntelliSense("Returns true if an annotation with the given name exists. Otherwise false.")]
        public bool HasAnnotation(string name)
        {
            return MetadataObject.Annotations.ContainsName(name);
        }
        ///<summary>Gets the value of the annotation with the given name. Returns null if no such annotation exists.</summary>
        [IntelliSense("Gets the value of the annotation with the given name. Returns null if no such annotation exists.")]
        public string GetAnnotation(string name)
        {
            return HasAnnotation(name) ? MetadataObject.Annotations[name].Value : null;
        }
        ///<summary>Sets the value of the annotation with the given index, assuming it exists.</summary>
        [IntelliSense("Sets the value of the annotation with the given index, assuming it exists.")]
        public void SetAnnotation(int index, string value)
        {
            SetAnnotation(index, value, true);
        }
        internal void SetAnnotation(int index, string value, bool undoable)
        {
            var name = MetadataObject.Annotations[index].Name;
            SetAnnotation(name, value, undoable);
        }
        void IInternalAnnotationObject.SetAnnotation(int index, string value, bool undoable)
        {
            SetAnnotation(index, value, undoable);
        }
        ///<summary>Returns a unique name for a new annotation.</summary>
        public string GetNewAnnotationName()
        {
            return MetadataObject.Annotations.GetNewName("New Annotation");
        }
        ///<summary>Sets the value of the annotation having the given name. If no such annotation exists, it will be created. If value is set to null, the annotation will be removed.</summary>
        [IntelliSense("Sets the value of the annotation having the given name. If no such annotation exists, it will be created. If value is set to null, the annotation will be removed.")]
        public void SetAnnotation(string name, string value)
        {
            SetAnnotation(name, value, true);
        }
        internal void SetAnnotation(string name, string value, bool undoable)
        {
            if (name == null) name = GetNewAnnotationName();

            if (value == null)
            {
                // Remove annotation if set to null:
                RemoveAnnotation(name, undoable);
                return;
            }

            if (undoable)
            {
                if (GetAnnotation(name) == value) return;
                bool undoable2 = true;
                bool cancel = false;
                OnPropertyChanging(Properties.ANNOTATIONS, name + ":" + value, ref undoable2, ref cancel);
                if (cancel) return;
            }

            if (MetadataObject.Annotations.Contains(name))
            {
                // Change existing annotation:

                var oldValue = GetAnnotation(name);
                MetadataObject.Annotations[name].Value = value;
                if (undoable)
                {
                    Handler.UndoManager.Add(new UndoAnnotationAction(this, name, value, oldValue));
                    OnPropertyChanged(Properties.ANNOTATIONS, name + ":" + oldValue, name + ":" + value);
                }
            }
            else
            {
                // Add new annotation:

                MetadataObject.Annotations.Add(new TOM.Annotation { Name = name, Value = value });
                if (undoable)
                {
                    Handler.UndoManager.Add(new UndoAnnotationAction(this, name, value, null));
                    OnPropertyChanged(Properties.ANNOTATIONS, null, name + ":" + value);
                }
            }
        }
        void IInternalAnnotationObject.SetAnnotation(string name, string value, bool undoable)
        {
            this.SetAnnotation(name, value, undoable);
        }
        ///<summary>Remove an annotation by the given name.</summary>
        [IntelliSense("Remove an annotation by the given name.")]
        public void RemoveAnnotation(string name)
        {
            RemoveAnnotation(name, true);
        }
        internal void RemoveAnnotation(string name, bool undoable)
        {
            if (MetadataObject.Annotations.Contains(name))
            {
                if (undoable)
                {
                    bool undoable2 = true;
                    bool cancel = false;
                    OnPropertyChanging(Properties.ANNOTATIONS, name + ":" + GetAnnotation(name), ref undoable2, ref cancel);
                    if (cancel) return;
                }

                var oldValue = MetadataObject.Annotations[name].Value;
                MetadataObject.Annotations.Remove(name);

                if (undoable)
                {
                    Handler.UndoManager.Add(new UndoAnnotationAction(this, name, null, oldValue));
                    OnPropertyChanged(Properties.ANNOTATIONS, name + ":" + oldValue, null);
                }
            }
        }
        void IInternalAnnotationObject.RemoveAnnotation(string name, bool undoable)
        {
            this.RemoveAnnotation(name, undoable);
        }
        ///<summary>Gets the number of annotations on the current Measure.</summary>
        [IntelliSense("Gets the number of annotations on the current Measure.")]
        public int GetAnnotationsCount()
        {
            return MetadataObject.Annotations.Count;
        }
        ///<summary>Gets a collection of all annotation names on the current Measure.</summary>
        [IntelliSense("Gets a collection of all annotation names on the current Measure.")]
        public IEnumerable<string> GetAnnotations()
        {
            return MetadataObject.Annotations.Select(a => a.Name);
        }

        ///<summary>The collection of Extended Properties on the current Measure.</summary>
        [DisplayName("Extended Properties"), NoMultiselect, Category("Translations and Perspectives"), Description("The collection of Extended Properties on the current Measure."), Editor(typeof(ExtendedPropertyCollectionEditor), typeof(UITypeEditor))]
        public ExtendedPropertyCollection ExtendedProperties { get; private set; }

        ///<summary>Returns true if an ExtendedProperty with the given name exists. Otherwise false.</summary>
        [IntelliSense("Returns true if an ExtendedProperty with the given name exists. Otherwise false.")]
        public bool HasExtendedProperty(string name)
        {
            return MetadataObject.ExtendedProperties.ContainsName(name);
        }
        ///<summary>Gets the type of the ExtendedProperty with the given index, assuming it exists.</summary>
        public ExtendedPropertyType GetExtendedPropertyType(int index)
        {
            return (ExtendedPropertyType)MetadataObject.ExtendedProperties[index].Type;
        }
        ///<summary>Gets the type of the ExtendedProperty with the given name, assuming it exists.</summary>
        public ExtendedPropertyType GetExtendedPropertyType(string name)
        {
            return (ExtendedPropertyType)MetadataObject.ExtendedProperties[name].Type;
        }
        ///<summary>Gets the value of the ExtendedProperty with the given index, assuming it exists.</summary>
        public string GetExtendedProperty(int index)
        {
            var ep = MetadataObject.ExtendedProperties[index];
            return ep.Type == TOM.ExtendedPropertyType.Json ? (ep as TOM.JsonExtendedProperty).Value : (ep as TOM.StringExtendedProperty).Value;
        }
        ///<summary>Gets the value of the ExtendedProperty with the given name. Returns null if no such ExtendedProperty exists.</summary>
        [IntelliSense("Gets the value of the ExtendedProperty with the given name. Returns null if no such ExtendedProperty exists.")]
        public string GetExtendedProperty(string name)
        {
            if (!HasExtendedProperty(name)) return null;
            var ep = MetadataObject.ExtendedProperties[name];
            return ep.Type == TOM.ExtendedPropertyType.Json ? (ep as TOM.JsonExtendedProperty).Value : (ep as TOM.StringExtendedProperty).Value;
        }
        ///<summary>Sets the value of the ExtendedProperty with the given index, optionally specifiying the type (string or JSON) of the ExtendedProperty.</summary>
        public void SetExtendedProperty(int index, string value, ExtendedPropertyType type = ExtendedPropertyType.String)
        {
            var name = MetadataObject.ExtendedProperties[index].Name;
            SetExtendedProperty(name, value, type);
        }
        ///<summary>Returns a unique name for a new ExtendedProperty.</summary>
        public string GetNewExtendedPropertyName()
        {
            return MetadataObject.ExtendedProperties.GetNewName("New ExtendedProperty");
        }
        ///<summary>Sets the value of the ExtendedProperty having the given name. If no such ExtendedProperty exists, it will be created. If value is set to null, the ExtendedProperty will be removed.</summary>
        [IntelliSense("Sets the value of the ExtendedProperty having the given name. If no such ExtendedProperty exists, it will be created. If value is set to null, the ExtendedProperty will be removed.")]
        public void SetExtendedProperty(string name, string value, ExtendedPropertyType type = ExtendedPropertyType.String)
        {
            if (name == null) name = GetNewExtendedPropertyName();

            if (value == null)
            {
                // Remove ExtendedProperty if set to null:
                RemoveExtendedProperty(name);
                return;
            }

            if (GetExtendedProperty(name) == value) return;
            bool undoable = true;
            bool cancel = false;
            OnPropertyChanging(Properties.EXTENDEDPROPERTIES, name + ":" + value, ref undoable, ref cancel);
            if (cancel) return;

            if (MetadataObject.ExtendedProperties.Contains(name))
            {
                // Change existing ExtendedProperty:
                var oldValue = GetExtendedProperty(name);
                var oldType = GetExtendedPropertyType(name);
                var ep = MetadataObject.ExtendedProperties[name];
                if (ep is TOM.JsonExtendedProperty)
                    (ep as TOM.JsonExtendedProperty).Value = value;
                else
                    (ep as TOM.StringExtendedProperty).Value = value;

                if (undoable) Handler.UndoManager.Add(new UndoExtendedPropertyAction(this, name, value, oldValue, oldType));
                OnPropertyChanged(Properties.EXTENDEDPROPERTIES, name + ":" + oldValue, name + ":" + value);
            }
            else
            {
                // Add new ExtendedProperty:
                if (type == ExtendedPropertyType.Json)
                    MetadataObject.ExtendedProperties.Add(new TOM.JsonExtendedProperty { Name = name, Value = value });
                else
                    MetadataObject.ExtendedProperties.Add(new TOM.StringExtendedProperty { Name = name, Value = value });

                if (undoable) Handler.UndoManager.Add(new UndoExtendedPropertyAction(this, name, value, null, type));
                OnPropertyChanged(Properties.EXTENDEDPROPERTIES, null, name + ":" + value);
            }

        }
        ///<summary>Remove an ExtendedProperty by the given name.</summary>
        [IntelliSense("Remove an ExtendedProperty by the given name.")]
        public void RemoveExtendedProperty(string name)
        {
            if (MetadataObject.ExtendedProperties.Contains(name))
            {
                // Get current value:
                bool undoable = true;
                bool cancel = false;
                OnPropertyChanging(Properties.EXTENDEDPROPERTIES, name + ":" + GetExtendedProperty(name), ref undoable, ref cancel);
                if (cancel) return;

                var oldValue = GetExtendedProperty(name);
                var oldType = GetExtendedPropertyType(name);
                MetadataObject.ExtendedProperties.Remove(name);

                // Undo-handling:
                if (undoable) Handler.UndoManager.Add(new UndoExtendedPropertyAction(this, name, null, oldValue, oldType));
                OnPropertyChanged(Properties.EXTENDEDPROPERTIES, name + ":" + oldValue, null);
            }
        }
        ///<summary>Gets the number of ExtendedProperties on the current object.</summary>
        [IntelliSense("Gets the number of ExtendedProperties on the current object.")]
        public int GetExtendedPropertyCount()
        {
            return MetadataObject.ExtendedProperties.Count;
        }
        ///<summary>Gets a collection of all ExtendedProperty names on the current object.</summary>
        [IntelliSense("Gets a collection of all ExtendedProperty names on the current object.")]
        public IEnumerable<string> GetExtendedProperties()
        {
            return MetadataObject.ExtendedProperties.Select(a => a.Name);
        }
    }*/
}
