#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.AnalysisServices.Tabular.Extensions;

namespace Microsoft.AnalysisServices.Tabular.Serialization
{
    public static class MetadataSerializationOptionsBuilderExtension
    {
        public static MetadataSerializationOptionsBuilder WithRestrictedInformation(this MetadataSerializationOptionsBuilder builder, bool includeRestrictedInformation)
        {
            return includeRestrictedInformation
                ? builder.WithRestrictedInformation()
                : builder.WithoutRestrictedInformation();
        }


        public static MetadataSerializationOptionsBuilder WithMetadataOrderHints(this MetadataSerializationOptionsBuilder builder, bool includeMetadataOrderHints)
        {
            return includeMetadataOrderHints
                ? builder.WithMetadataOrderHints()
                : builder.WithoutMetadataOrderHints();
        }
    }
}
