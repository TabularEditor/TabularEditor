using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public enum SaveFormat
    {
        /// <summary>
        /// Saves only the Model Schema as a Model.bim file
        /// </summary>
        ModelSchemaOnly,

        /// <summary>
        /// Saves the Model Schema to an existing .pbit (Power BI Template) file
        /// </summary>
        PowerBiTemplate,

        /// <summary>
        /// Saves the Model Schema together with a Visual Studio Tabular Project file and user settings file
        /// </summary>
        VisualStudioProject,

        /// <summary>
        /// Saves the Model Schema as a Tabular Editor folder structure
        /// </summary>
        TabularEditorFolder
    }

    public enum ModelSourceType
    {
        /// <summary>
        /// SSAS Tabular database Compatibility Level 1200 or 1400
        /// </summary>
        Database,

        /// <summary>
        /// Model.bim Compatibility Level 1200 or 1400 JSON file
        /// </summary>
        File,

        /// <summary>
        /// Model.bim exploded into a folder structure by Tabular Editor
        /// </summary>
        Folder,

        /// <summary>
        /// Power BI Template file (.pbit)
        /// </summary>
        Pbit,

        UnsavedFile
    }

    public class TabularModelHandlerSettings
    {
        /// <summary>
        /// Specifies whether object name changes (tables, column, measures) should result in 
        /// automatic DAX expression updates to reflect the changed names. When set to true,
        /// all expressions in the model are parsed, to build a dependency tree.
        /// </summary>
        public bool AutoFixup { get; set; }

        /// <summary>
        /// If this is set to TRUE, only features supported by Power BI may be browsed/edited
        /// through the TOMWrapper. This is useful for example when a .pbit file has been loaded,
        /// or when connected to a Power BI Desktop instance.
        /// </summary>
        public bool PBIFeaturesOnly { get; set; }

        public static TabularModelHandlerSettings Default
        {
            get
            {
                return new TabularModelHandlerSettings
                {
                    AutoFixup = true,
                    PBIFeaturesOnly = true
                };
            }
        }
    }

}
