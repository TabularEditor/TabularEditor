using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
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
