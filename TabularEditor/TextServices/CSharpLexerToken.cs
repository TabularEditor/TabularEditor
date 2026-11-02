namespace TabularEditor.TextServices
{
    /// <summary>
    /// A token of a C# script, read by <see cref="CSharpTokenizer"/>. Holds a position in the script, so that reading a
    /// token costs nothing beyond its slot in a list. <see cref="Text"/> allocates each time it's read, and
    /// <see cref="Line"/> and <see cref="Column"/> scan the script.
    /// </summary>
    public readonly struct CSharpLexerToken
    {
        private readonly string _source;

        internal CSharpLexerToken(string source, int type, int channel, int startIndex, int length, int tokenIndex = -1)
        {
            _source = source;
            Type = type;
            Channel = channel;
            StartIndex = startIndex;
            StopIndex = startIndex + length - 1;
            TokenIndex = tokenIndex;
        }

        /// <summary>The type of the token, one of the constants of <see cref="CSharpToken"/>.</summary>
        public int Type { get; }

        /// <summary>The channel of the token: <see cref="CSharpTokenizer.DefaultChannel"/>, <see cref="CSharpTokenizer.HiddenChannel"/>, <see cref="CSharpTokenizer.CommentsChannel"/> or <see cref="CSharpTokenizer.DirectiveChannel"/>.</summary>
        public int Channel { get; }

        /// <summary>The index of the first char of the token.</summary>
        public int StartIndex { get; }

        /// <summary>The index of the last char of the token.</summary>
        public int StopIndex { get; }

        /// <summary>The index of the token in the list it was taken from, or -1 when it isn't in one.</summary>
        public int TokenIndex { get; }

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

        /// <summary>The text of the token, which is <c>&lt;EOF&gt;</c> for the last one.</summary>
        public string Text => Type == CSharpToken.Eof ? "<EOF>" : _source.Substring(StartIndex, StopIndex - StartIndex + 1);

        internal CSharpLexerToken WithTokenIndex(int tokenIndex) => new CSharpLexerToken(_source, Type, Channel, StartIndex, StopIndex - StartIndex + 1, tokenIndex);
    }
}
