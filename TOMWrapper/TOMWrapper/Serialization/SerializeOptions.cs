using Newtonsoft.Json;
using System.Collections.Generic;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper.Serialization
{
    public class SerializeOptions
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
                return new SerializeOptions();
            }
        }

        public static SerializeOptions DefaultFolder
        {
            get
            {
                var so = new SerializeOptions();
                so.Levels = new HashSet<string> {
                    "Data Sources",
                    "Shared Expressions",
                    "Perspectives",
                    "Relationships",
                    "Roles",
                    "Tables",
                    "Tables/Columns",
                    "Tables/Hierarchies",
                    "Tables/Measures",
                    "Tables/Partitions",
                    "Tables/CalculationItems",
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
        public bool IgnoreLineageTags { get; set; } = false;
        public bool ShouldSerializeIgnoreLineageTags() => IgnoreLineageTags;
        public bool SplitMultilineStrings = true;

        public bool IgnorePrivacySettings { get; set; } = false;
        public bool ShouldSerializeIgnorePrivacySettings() => IgnorePrivacySettings;
        public bool IgnoreIncrementalRefreshPartitions { get; set; } = false;
        public bool ShouldSerializeIgnoreIncrementalRefreshPartitions() => IgnoreIncrementalRefreshPartitions;

        public bool PrefixFilenames = false;
        public bool AlsoSaveAsBim { get; set; } = false;
        public bool ShouldSerializeAlsoSaveAsBim() => AlsoSaveAsBim;
        public bool LocalTranslations = false;
        public bool LocalPerspectives = false;
        public bool SortArrays = true;
        public bool LocalRelationships = false;

        public bool IncludeSensitive { get; set; } = false;
        public bool ShouldSerializeIncludeSensitive() => IncludeSensitive;
        public TmdlSerializeOptions TmdlOptions { get; set; } = new TmdlSerializeOptions();
        public bool ShouldSerializeTmdlOptions() => !TmdlOptions.IsDefault();
        [JsonIgnore]
        public string DatabaseNameOverride = null;

        public HashSet<string> Levels = new HashSet<string>();

        public bool Equals(SerializeOptions other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return other.Levels.SetEquals(this.Levels)
                && other.IgnoreInferredObjects == IgnoreInferredObjects
                && other.IgnoreLineageTags == IgnoreLineageTags
                && other.IgnoreInferredProperties == IgnoreInferredProperties
                && other.IgnoreTimestamps == IgnoreTimestamps
                && other.SplitMultilineStrings == SplitMultilineStrings
                && other.PrefixFilenames == PrefixFilenames
                && other.AlsoSaveAsBim == AlsoSaveAsBim
                && other.LocalTranslations == LocalTranslations
                && other.LocalPerspectives == LocalPerspectives
                && other.LocalRelationships == LocalRelationships
                && other.IgnoreIncrementalRefreshPartitions == IgnoreIncrementalRefreshPartitions
                && other.IncludeSensitive == IncludeSensitive
                && other.TmdlOptions == TmdlOptions;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SerializeOptions);
        }

        public static bool operator== (SerializeOptions obj1, SerializeOptions obj2)
        {
            if (ReferenceEquals(obj1, null))
                return ReferenceEquals(obj2, null);

            return obj1.Equals(obj2);
        }

        public static bool operator!= (SerializeOptions obj1, SerializeOptions obj2)
        {
            return !(obj1 == obj2);
        }
    }
    internal class DataSourceProperties
    {
        public DataSourceProperties() { }

        public DataSourceProperties(DataSource source)
        {
            if (source is ProviderDataSource pds)
            {
                Username = pds.Account;
                Password = pds.Password;
                ConnectionString = pds.ConnectionString;
            }
            else if (source is StructuredDataSource sds)
            {
                Username = sds.Username;
                Password = sds.Password;
                PrivacySetting = sds.PrivacySetting;
                AccountKey = sds.Credential[TOM.CredentialProperty.Key] as string;
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string PrivacySetting { get; set; }
        public string ConnectionString { get; set; }
        public string AccountKey { get; set; }
        public string ImpersonationMode { get; set; }
    }
}
