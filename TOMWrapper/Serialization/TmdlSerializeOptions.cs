using Microsoft.AnalysisServices.Tabular.Serialization;
using Microsoft.AnalysisServices.Tabular.Tmdl;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Text;

namespace TabularEditor.TOMWrapper.Serialization
{
    public record class TmdlSerializeOptions
    {
        private const TmdlCasingStyle DefaultCasingStyle = TmdlCasingStyle.CamelCase;
        private const TmdlExpressionTrimStyle DefaultExpressionTrimStyle = TmdlExpressionTrimStyle.NoTrim;
        private const NewLineStyle DefaultNewLineStyle = NewLineStyle.SystemDefault;
        private const SerializationEncoding DefaultEncoding = SerializationEncoding.UTF8;
        private const int DefaultBaseIndentationLevel = 0;
        private const int DefaultSpacesIndentation = 0; // Use tabs for indentation by default
        private const bool DefaultIncludeRefs = true;

        internal bool IsDefault() => DefaultCasingStyle == CasingStyle
            && DefaultExpressionTrimStyle == ExpressionTrimStyle
            && DefaultNewLineStyle == NewLineStyle
            && DefaultEncoding == Encoding
            && DefaultBaseIndentationLevel == BaseIndentationLevel
            && DefaultSpacesIndentation == SpacesIndentation
            && DefaultIncludeRefs == IncludeRefs;

        public bool IncludeRefs { get; init; } = DefaultIncludeRefs;

        [JsonConverter(typeof(StringEnumConverter))]
        public TmdlCasingStyle CasingStyle { get; init; } = DefaultCasingStyle;

        [JsonConverter(typeof(StringEnumConverter))]
        public TmdlExpressionTrimStyle ExpressionTrimStyle { get; init; } = DefaultExpressionTrimStyle;

        [JsonConverter(typeof(StringEnumConverter))]
        public NewLineStyle NewLineStyle { get; init; } = DefaultNewLineStyle;

        [property: JsonConverter(typeof(StringEnumConverter))]
        public SerializationEncoding Encoding { get; init; } = DefaultEncoding;
        public int BaseIndentationLevel { get; init; } = DefaultBaseIndentationLevel;
        /// <summary>
        /// A positive value indicates that the TMDL serializer should use this number of spaces for indentation levels. A negative value or zero indicate that the TMDL serializer should use tabs for indentation.
        /// </summary>
        public int SpacesIndentation { get; init; } = DefaultSpacesIndentation;

        [JsonIgnore]
        public bool SerializeDefaults { get; set; } = false;

        public bool ShouldSerializeIncludeRefs() => SerializeDefaults || IncludeRefs != DefaultIncludeRefs;
        public bool ShouldSerializeCasingStyle() => SerializeDefaults || CasingStyle != DefaultCasingStyle;
        public bool ShouldSerializeExpressionTrimStyle() => SerializeDefaults || ExpressionTrimStyle != DefaultExpressionTrimStyle;
        public bool ShouldSerializeNewLineStyle() => SerializeDefaults || NewLineStyle != DefaultNewLineStyle;
        public bool ShouldSerializeEncoding() => SerializeDefaults || Encoding != DefaultEncoding;
        public bool ShouldSerializeBaseIndentationLevel() => SerializeDefaults || BaseIndentationLevel != DefaultBaseIndentationLevel;
        public bool ShouldSerializeSpacesIndentation() => SerializeDefaults || SpacesIndentation != DefaultSpacesIndentation;
    }

    public enum SerializationEncoding
    {
        Default = 0,
        UTF8 = 1,
        UTF7 = 2,
        UTF32 = 3,
        Unicode = 4,
        ASCII = 5,
        BigEndianUnicode = 6
    }

    static class TmdlSerializeOptionsExtensions
    {
        public static string GetNewLine(this TmdlSerializeOptions options, int repeat = 1)
        {
            var newLine = options.NewLineStyle switch
            {
                NewLineStyle.UnixStyle => "\n",
                NewLineStyle.WindowsStyle => "\r\n",
                _ => Environment.NewLine
            };
            return string.Concat(System.Linq.Enumerable.Repeat(newLine, repeat));
        }

        private static UTF8Encoding utf8encodingWithoutBom
            = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        // Todo: Consider configuration for BOM/no BOM. Same for UTF32, where we also have an option for big/small endian.
        // Also, consider codepage configuration for ASCII and other encodings.

        public static Encoding GetEncoding(this TmdlSerializeOptions options)
        {
            return options.Encoding switch
            {
                SerializationEncoding.UTF8 => utf8encodingWithoutBom,
                SerializationEncoding.UTF7 => Encoding.UTF7,
                SerializationEncoding.UTF32 => Encoding.UTF32,
                SerializationEncoding.Unicode => Encoding.Unicode,
                SerializationEncoding.ASCII => Encoding.ASCII,
                SerializationEncoding.BigEndianUnicode => Encoding.BigEndianUnicode,
                _ => Encoding.Default
            };
        }
    }
}
