using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TextServices
{
    /// <summary>
    /// A token of a DAX expression, read by <see cref="DaxTokenizer"/>. Holds a position in the expression, so that reading
    /// a token costs nothing beyond its slot in a list. <see cref="Text"/> allocates each time it's read, and
    /// <see cref="Line"/> and <see cref="Column"/> scan the expression.
    /// </summary>
    public readonly struct DaxLexerToken
    {
        private readonly string _source;

        internal DaxLexerToken(string source, int type, int startIndex, int length)
        {
            _source = source;
            Type = type;
            StartIndex = startIndex;
            StopIndex = startIndex + length - 1;
        }

        /// <summary>The type of the token, one of the constants of <see cref="DaxToken"/>.</summary>
        public int Type { get; }

        /// <summary>The index of the first char of the token.</summary>
        public int StartIndex { get; }

        /// <summary>The index of the last char of the token.</summary>
        public int StopIndex { get; }

        /// <summary>The line of the first char of the token, starting at 1. Only <c>\n</c> ends a line.</summary>
        public int Line
        {
            get
            {
                var line = 1;
                for (var i = _source.IndexOf('\n', 0, StartIndex); i >= 0; i = _source.IndexOf('\n', i + 1, StartIndex - i - 1)) line++;
                return line;
            }
        }

        /// <summary>The number of chars between the start of the line and the first char of the token.</summary>
        public int Column => StartIndex == 0 ? 0 : StartIndex - _source.LastIndexOf('\n', StartIndex - 1, StartIndex) - 1;

        /// <summary>The channel of the token: <see cref="DaxTokenizer.DefaultChannel"/>, <see cref="DaxTokenizer.HiddenChannel"/>, <see cref="DaxTokenizer.CommentsChannel"/> or <see cref="DaxTokenizer.KeywordChannel"/>.</summary>
        public int Channel => DaxTokenizer.GetChannel(Type);

        /// <summary>
        /// The text of the token. String and date literals are returned without their quotes. Table and object names are
        /// returned without their quotes or brackets, and with <c>''</c> and <c>]]</c> unescaped.
        /// </summary>
        public string Text
        {
            get
            {
                var length = StopIndex - StartIndex + 1;
                switch (Type)
                {
                    case DaxToken.STRING_LITERAL: return _source.Substring(StartIndex + 1, length - 2);
                    case DaxToken.DATE_LITERAL: return _source.Substring(StartIndex + 3, length - 4);
                    case DaxToken.TABLE: return _source.Substring(StartIndex + 1, length - 2).Replace("''", "'");
                    case DaxToken.COLUMN_OR_MEASURE: return _source.Substring(StartIndex + 1, length - 2).Replace("]]", "]");
                    default: return _source.Substring(StartIndex, length);
                }
            }
        }
    }
}
