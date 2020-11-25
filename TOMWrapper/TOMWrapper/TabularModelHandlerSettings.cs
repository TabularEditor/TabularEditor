using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class TabularModelHandlerSettings : INotifyPropertyChanged
    {
        private bool _changeDetectionLocalServers;
        private bool _autoFixup;
        private bool _pBIFeaturesOnly;

        /// <summary>
        /// Set to true to always add a PowerQuery (M) partition by default to newly created tables
        /// </summary>
        public bool UsePowerQueryPartitionsByDefault { get; set; } = false;

        /// <summary>
        /// Specifies whether an AS trace should be started to monitor the server for changes
        /// made by other applications.
        /// </summary>
        public bool ChangeDetectionLocalServers
        {
            get => _changeDetectionLocalServers;
            set
            {
                if (value == _changeDetectionLocalServers) return;
                _changeDetectionLocalServers = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChangeDetectionLocalServers"));
            }
        }

        /// <summary>
        /// Specifies whether object name changes (tables, column, measures) should result in 
        /// automatic DAX expression updates to reflect the changed names. When set to true,
        /// all expressions in the model are parsed, to build a dependency tree.
        /// </summary>
        public bool AutoFixup {
            get => _autoFixup;
            set
            {
                if (value == _autoFixup) return;
                _autoFixup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AutoFixup"));
            }
        }

        /// <summary>
        /// If this is set to TRUE, only features supported by Power BI may be browsed/edited
        /// through the TOMWrapper. This is useful for example when a .pbit file has been loaded,
        /// or when connected to a Power BI Desktop instance.
        /// </summary>
        public bool PBIFeaturesOnly
        {
            get => _pBIFeaturesOnly;
            set
            {
                if (value == _pBIFeaturesOnly) return;
                _pBIFeaturesOnly = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PBIFeaturesOnly"));
            }
        }

        public static TabularModelHandlerSettings Default
        {
            get
            {
                return new TabularModelHandlerSettings
                {
                    AutoFixup = true,
                    PBIFeaturesOnly = true,
                    ChangeDetectionLocalServers = true
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
