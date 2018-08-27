using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Utils
{
    public class SerializeOptions: IEquatable<SerializeOptions>
    {
        public SerializeOptions Clone()
        {
            SerializeOptions other = (SerializeOptions)MemberwiseClone();
            other.Levels = new HashSet<string>(Levels);
            return other;
        }

        public static SerializeOptions Default
        {
            get
            {
                var so = new SerializeOptions();
                so.Levels = new HashSet<string> {
                    "Data Sources",
                    "Perspectives",
                    "Relationships",
                    "Roles",
                    "Tables",
                    "Tables/Columns",
                    "Tables/Hierarchies",
                    "Tables/Measures",
                    "Tables/Partitions",
                    "Translations"
                };
                return so;
            }
        }
        public static SerializeOptions PowerBi
        {
            get
            {
                return new SerializeOptions()
                {
                    IgnoreInferredObjects = false,
                    IgnoreInferredProperties = false,
                    IgnoreTimestamps = false
                };
            }
        }
        public bool IgnoreInferredObjects = true;
        public bool IgnoreInferredProperties = true;
        public bool IgnoreTimestamps = true;
        public bool SplitMultilineStrings = true;
        public bool PrefixFilenames = false;
        public bool LocalTranslations = false;
        public bool LocalPerspectives = false;

        public HashSet<string> Levels = new HashSet<string>();


        public bool Equals(SerializeOptions other)
        {
            return other.Levels.SetEquals(this.Levels)
                && other.IgnoreInferredObjects == IgnoreInferredObjects
                && other.IgnoreInferredProperties == IgnoreInferredProperties
                && other.IgnoreTimestamps == IgnoreTimestamps
                && other.SplitMultilineStrings == SplitMultilineStrings
                && other.PrefixFilenames == PrefixFilenames
                && other.LocalTranslations == LocalTranslations
                && other.LocalPerspectives == LocalPerspectives;
        }
    }

}
