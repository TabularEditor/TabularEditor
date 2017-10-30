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
    }

    /// <summary>
    /// Objects whose name and description properties can be translated
    /// </summary>
    public interface ITranslatableObject: IAnnotationObject, ITabularNamedObject
    {
        TranslationIndexer TranslatedNames { get; }
        TranslationIndexer TranslatedDescriptions { get; }
    }

    public interface ITabularNamedObject : ITabularObject
    {
        string Name { get; set; }
        int MetadataIndex { get; }
        bool CanDelete();
        bool CanDelete(out string message);
        void Delete();
    }

    #region Common interfaces
    /// <summary>
    /// Objects that can be shown/hidden
    /// </summary>
    public interface IHideableObject
    {
        bool IsHidden { get; set; }
    }

    /// <summary>
    /// Objects that can be shown/hidden in individual perspectives
    /// </summary>
    public interface ITabularPerspectiveObject: IHideableObject, IAnnotationObject
    {
        PerspectiveIndexer InPerspective { get; }
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
        string GetAnnotation(int index);
        string GetAnnotation(string name);
        string GetNewAnnotationName();
        void SetAnnotation(int index, string value, bool undoable = true);
        void SetAnnotation(string name, string value, bool undoable = true);
        void RemoveAnnotation(string name, bool undoable = true);
        int GetAnnotationsCount();
        IEnumerable<string> GetAnnotations();
    }

    /// <summary>
    /// Objects that have an expression (measure, calcualted column, partition, etc.)
    /// </summary>
    public interface IExpressionObject: ITabularNamedObject
    {
        bool NeedsValidation { get; set; }
        string Expression { get; set; }
    }

    /// <summary>
    /// Objects that can depend on one or more DAXObjects through expression dependencies
    /// </summary>
    public interface IDaxDependantObject: ITabularNamedObject
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
    public interface IDetailObject : ITabularTableObject
    {
        string DisplayFolder { get; set; }
        TranslationIndexer TranslatedDisplayFolders { get; }
    }

    /// <summary>
    /// Represents an objects that can contain other objects as well as display folders. Examples:
    ///  - Folders
    ///  - Table
    /// </summary>
    public interface IDetailObjectContainer : ITabularNamedObject
    {
        IEnumerable<IDetailObject> GetChildrenByFolders(bool recursive = false);
        Table ParentTable { get; }
    }
}
