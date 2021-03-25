using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// Objects that can be cloned
    /// </summary>
    public interface IClonableObject
    {
        TabularNamedObject Clone(string newName = null, bool includeTranslations = false, TabularNamedObject newParent = null);
    }

    public interface ITabularObject: INotifyPropertyChanged
    {
        ObjectType ObjectType { get; }
        Model Model { get; }
        bool IsRemoved { get; }
    }

    internal interface IInternalTabularObject: ITabularObject
    {
        void ReapplyReferences();
    }

    public interface ILineageTagObject: ITabularNamedObject
    {
        string LineageTag { get; set; }
    }

    public interface ISynonymObject: ITabularNamedObject
    {
        SynonymIndexer Synonyms { get; }
    }

    /// <summary>
    /// Objects whose name and description properties can be translated
    /// </summary>
    public interface ITranslatableObject: IAnnotationObject, ITabularNamedObject
    {
        TranslationIndexer TranslatedNames { get; }
        TranslationIndexer TranslatedDescriptions { get; }
    }
    internal interface IInternalTranslatableObject: ITranslatableObject, IInternalAnnotationObject
    {

    }

    public interface ITabularNamedObject : ITabularObject
    {
        string Name { get; set; }
        int MetadataIndex { get; }
        bool CanDelete();
        /// <summary>
        /// True if the Name property of this object can be changed, false otherwise.
        /// </summary>
        bool CanEditName();
        bool CanDelete(out string message);
        void Delete();
    }

    internal interface IInternalTabularNamedObject: ITabularNamedObject, IInternalTabularObject
    {
        void RemoveReferences();
    }

    #region Common interfaces
    /// <summary>
    /// Objects that can be shown/hidden
    /// </summary>
    public interface IHideableObject
    {
        bool IsHidden { get; set; }
        bool IsVisible { get; }
    }

    /// <summary>
    /// Objects that can be shown/hidden in individual perspectives
    /// </summary>
    public interface ITabularPerspectiveObject: IHideableObject, IAnnotationObject
    {
        PerspectiveIndexer InPerspective { get; }
    }

    internal interface IInternalTabularPerspectiveObject: ITabularPerspectiveObject, IInternalAnnotationObject
    {

    }

    /// <summary>
    /// Objects that can have descriptions
    /// </summary>
    public interface IDescriptionObject
    {
        string Description { get; set; }
    }

    /// <summary>
    /// Objects that can have error messages
    /// </summary>
    public interface IErrorMessageObject
    {
        string ErrorMessage { get; }
    }

    /// <summary>
    /// Objects that have annotations
    /// </summary>
    public interface IAnnotationObject: ITabularObject
    {
        bool HasAnnotation(string name);
        string GetAnnotation(int index);
        string GetAnnotation(string name);
        string GetNewAnnotationName();
        void SetAnnotation(int index, string value);
        void SetAnnotation(string name, string value);
        void RemoveAnnotation(string name);
        int GetAnnotationsCount();
        IEnumerable<string> GetAnnotations();
        AnnotationCollection Annotations { get; }
    }

    internal interface IInternalAnnotationObject: IAnnotationObject
    {
        void SetAnnotation(int index, string value, bool undoable = false);
        void SetAnnotation(string name, string value, bool undoable = false);
        void RemoveAnnotation(string name, bool undoable = false);
    }

    public interface IExtendedPropertyObject: ITabularObject
    {
        bool HasExtendedProperty(string name);
        string GetExtendedProperty(string name);
        string GetExtendedProperty(int index);
        string GetNewExtendedPropertyName();
        void SetExtendedProperty(int index, string value, ExtendedPropertyType type);
        void SetExtendedProperty(string name, string value, ExtendedPropertyType type);
        void RemoveExtendedProperty(string name);
        ExtendedPropertyType GetExtendedPropertyType(string name);
        ExtendedPropertyType GetExtendedPropertyType(int index);
        IEnumerable<string> GetExtendedProperties();
        int GetExtendedPropertyCount();
        ExtendedPropertyCollection ExtendedProperties { get; }
    }

    internal interface IInternalExtendedPropertyObject: IExtendedPropertyObject
    {
        void SetExtendedProperty(int index, string value, ExtendedPropertyType type, bool undoable = false);
        void SetExtendedProperty(string name, string value, ExtendedPropertyType type, bool undoable = false);
        void RemoveExtendedProperty(string name, bool undoable = false);
    }

    /// <summary>
    /// Objects that have an expression (measure, calcualted column, partition, etc.)
    /// </summary>
    public interface IExpressionObject: ITabularNamedObject
    {
        bool NeedsValidation { get; }
        void ResetModifiedState();
        string Expression { get; set; }
    }

    /// <summary>
    /// Objects that can depend on one or more DAXObjects through expression dependencies
    /// </summary>
    public interface IDaxDependantObject: ITabularObject
    {
        DependsOnList DependsOn { get; }
    }

    /// <summary>
    /// Object that belongs to a specific table.
    /// </summary>
    public interface ITabularTableObject : ITabularNamedObject
    {
        Table Table { get; }
    }

    /// <summary>
    /// Objects that can be referenced in a DAX expression (table, column, measure)
    /// </summary>
    public interface IDaxObject: ITabularNamedObject
    {
        string DaxObjectName { get; }
        string DaxObjectFullName { get; }
        string DaxTableName { get; }
        ReferencedByList ReferencedBy { get; }
    }

    public interface IFormattableObject
    {
        string FormatString { get; set; }
        DataType DataType { get; }
    }
    #endregion

    /// <summary>
    /// Represents an object than can be contained in a Display Folder. Examples:
    ///  - Measures
    ///  - Columns
    ///  - Hierarchies
    ///  - Folders
    /// </summary>
    public interface IFolderObject : ITabularTableObject
    {
        string DisplayFolder { get; set; }
        TranslationIndexer TranslatedDisplayFolders { get; }
    }

    /// <summary>
    /// Represents an objects that can contain IFolderObject's
    ///  - Folders
    ///  - Table
    /// </summary>
    public interface IFolder : ITabularNamedObject, ITabularObjectContainer
    {
        IEnumerable<IFolderObject> GetChildrenByFolders();
        Table ParentTable { get; }
    }
}
