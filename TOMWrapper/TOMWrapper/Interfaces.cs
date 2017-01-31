using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public interface IClonableObject
    {
        TabularNamedObject Clone(string newName, bool includeTranslations);
    }

    public interface ITabularObject: INotifyPropertyChanged
    {
        ObjectType ObjectType { get; }
        Model Model { get; }
    }

    public interface ITabularNamedObject : ITabularObject
    {
        string Name { get; set; }
        TranslationIndexer TranslatedNames { get; }
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
    public interface ITabularPerspectiveObject: IHideableObject
    {
        PerspectiveIndexer InPerspective { get; }
    }

    /// <summary>
    /// Objects that can have descriptions
    /// </summary>
    public interface IDescriptionObject
    {
        string Description { get; set; }
        TranslationIndexer TranslatedDescriptions { get; }
    }

    /// <summary>
    /// Objects that can have error messages
    /// </summary>
    public interface IErrorMessageObject
    {
        string ErrorMessage { get; }
    }

    public interface IExpressionObject: IDaxObject
    {
        string Expression { get; set; }
        bool NeedsValidation { get; set; }
        Dictionary<IDaxObject, List<Dependency>> Dependencies { get; }
    }

    /// <summary>
    /// Object that belongs to a specific table.
    /// </summary>
    public interface ITabularTableObject : ITabularNamedObject
    {
        Table Table { get; }
        void Delete();
    }

    public interface IDaxObject: ITabularNamedObject
    {
        string DaxObjectName { get; }
        string DaxObjectFullName { get; }
        string DaxTableName { get; }
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
