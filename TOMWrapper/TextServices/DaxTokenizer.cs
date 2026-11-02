using Parlot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TextServices
{
    /// <summary>
    /// Reads the tokens of a DAX expression. Keywords and functions are matched without regard to case. Text which isn't
    /// part of any token is skipped: the first char that can't continue a token is dropped with what was read of it, so
    /// that for instance nothing remains of a string that is never closed.
    /// </summary>
    /// <remarks>
    /// A token is the longest text that matches any of the token types, and the types with a closing char accept the
    /// shorter text if a longer one turns out not to be closed. In the non-US format <c>;</c> separates arguments and
    /// <c>,</c> is a decimal separator, as well as the period of the functions which have one, like <c>STDEV.P</c>.
    /// </remarks>
    public readonly struct DaxTokenizer
    {
        public const int DefaultChannel = 0;
        public const int HiddenChannel = 1;
        public const int CommentsChannel = 2;
        public const int KeywordChannel = 3;

        private const byte Identifier = 1;
        private const byte Digit = 2;
        private const byte WhiteSpace = 3;

        private static readonly byte[] AsciiKinds = BuildAsciiKinds();

        private readonly Cursor _cursor;
        private readonly bool _nonUs;

        /// <summary>
        /// Creates a tokenizer for an expression.
        /// </summary>
        /// <param name="dax">The expression to read.</param>
        /// <param name="convertFromNonUs">Whether the expression uses the non-US format.</param>
        public DaxTokenizer(string dax, bool convertFromNonUs = false)
        {
            _cursor = new Cursor(dax);
            _nonUs = convertFromNonUs;
        }

        /// <summary>
        /// Returns all the tokens of an expression, on any channel.
        /// </summary>
        public static List<DaxLexerToken> GetAllTokens(string dax, bool convertFromNonUs = false)
        {
            // Expressions have about one token, white space included, every 3.5 chars
            var tokens = new List<DaxLexerToken>(dax.Length / 3 + 1);
            var tokenizer = new DaxTokenizer(dax, convertFromNonUs);
            while (tokenizer.Read(out var token)) tokens.Add(token);
            return tokens;
        }

        /// <summary>
        /// Returns the channel of a type of token.
        /// </summary>
        public static int GetChannel(int type)
        {
            if (type >= DaxToken.DATE_LITERAL) return DefaultChannel;
            if (type >= DaxToken.ABS) return KeywordChannel;
            return type == DaxToken.WHITESPACES ? HiddenChannel : CommentsChannel;
        }

        /// <summary>
        /// Returns the name of the <see cref="DaxToken"/> constant of a type of token.
        /// </summary>
        public static string GetSymbolicName(int type)
        {
            return type > 0 && type < DaxKeywords.SymbolicNames.Length ? DaxKeywords.SymbolicNames[type] : null;
        }

        /// <summary>
        /// Reads the next token.
        /// </summary>
        /// <returns><see langword="false"/> at the end of the expression.</returns>
        public bool Read(out DaxLexerToken token)
        {
            var cursor = _cursor;
            while (!cursor.Eof)
            {
                var length = Scan(cursor.Span, out var type);
                if (type != 0)
                {
                    token = new DaxLexerToken(cursor.Buffer, type, cursor.Offset, length);
                    cursor.Advance(length);
                    return true;
                }
                cursor.Advance(length);
            }
            token = default;
            return false;
        }

        /// <summary>
        /// Reads the next token that isn't white space or a comment.
        /// </summary>
        /// <returns><see langword="false"/> at the end of the expression.</returns>
        public bool ReadDefault(out DaxLexerToken token)
        {
            while (Read(out token))
            {
                if (token.Type >= DaxToken.ABS) return true;
            }
            return false;
        }

        // Returns the length of the token at the start of the text and its type. The type is 0 when no token matches,
        // the result being then the number of chars to drop.
        private int Scan(ReadOnlySpan<char> s, out int type)
        {
            var c = s[0];
            var kind = c < 128 ? AsciiKinds[c] : GetNonAsciiKind(c);

            switch (kind)
            {
                case Identifier:
                    return ScanIdentifier(s, out type);
                case WhiteSpace:
                    type = DaxToken.WHITESPACES;
                    return ScanWhiteSpace(s);
                case Digit:
                    return ScanNumber(s, out type);
            }

            type = 0;
            switch (c)
            {
                case '[': return ScanName(s, ']', DaxToken.COLUMN_OR_MEASURE, out type);
                case '(': type = DaxToken.OPEN_PARENS; return 1;
                case ')': type = DaxToken.CLOSE_PARENS; return 1;
                case '\'': return ScanName(s, '\'', DaxToken.TABLE, out type);
                case '"': return ScanString(s, 0, out type);
                case '{': type = DaxToken.OPEN_CURLY; return 1;
                case '}': type = DaxToken.CLOSE_CURLY; return 1;
                case '+': type = DaxToken.PLUS; return 1;
                case '*': type = DaxToken.STAR; return 1;
                case '^': type = DaxToken.CARET; return 1;
                case ':': type = DaxToken.COLON; return 1;
                case ',':
                    if (_nonUs) return ScanNumber(s, out type);
                    type = DaxToken.COMMA;
                    return 1;
                case ';':
                    if (_nonUs) type = DaxToken.COMMA;
                    return 1;
                case '.': return ScanNumber(s, out type);
                case '/':
                    if (s.Length > 1)
                    {
                        if (s[1] == '/')
                        {
                            type = DaxToken.SINGLE_LINE_COMMENT;
                            return ScanLineComment(s);
                        }
                        if (s[1] == '*') return ScanDelimitedComment(s, out type);
                    }
                    type = DaxToken.DIV;
                    return 1;
                case '-':
                    if (s.Length > 1 && s[1] == '-')
                    {
                        type = DaxToken.SINGLE_LINE_COMMENT;
                        return ScanLineComment(s);
                    }
                    type = DaxToken.MINUS;
                    return 1;
                case '=':
                    if (s.Length > 1 && s[1] == '>')
                    {
                        type = DaxToken.LAMBDA;
                        return 2;
                    }
                    type = DaxToken.ASSIGNMENT;
                    return 1;
                case '<':
                    if (s.Length > 1)
                    {
                        if (s[1] == '>')
                        {
                            type = DaxToken.OP_NE;
                            return 2;
                        }
                        if (s[1] == '=')
                        {
                            type = DaxToken.OP_LE;
                            return 2;
                        }
                    }
                    type = DaxToken.LT;
                    return 1;
                case '>':
                    if (s.Length > 1 && s[1] == '=')
                    {
                        type = DaxToken.OP_GE;
                        return 2;
                    }
                    type = DaxToken.GT;
                    return 1;
                case '&':
                    if (s.Length > 1 && s[1] == '&')
                    {
                        type = DaxToken.OP_AND;
                        return 2;
                    }
                    type = DaxToken.AMP;
                    return 1;
                case '|':
                    if (s.Length > 1 && s[1] == '|')
                    {
                        type = DaxToken.OP_OR;
                        return 2;
                    }
                    return DropThrough(s, 1);
                default:
                    return 1;
            }
        }

        private int ScanIdentifier(ReadOnlySpan<char> s, out int type)
        {
            var hash = 0u;
            var i = 0;
            for (; i < s.Length; i++)
            {
                var c = s[i];
                char upper;
                if (c < 128)
                {
                    var kind = AsciiKinds[c];
                    if (kind != Identifier && kind != Digit) break;
                    upper = (uint)(c - 'a') <= 'z' - 'a' ? (char)(c - 32) : c;
                }
                else
                {
                    upper = char.ToUpperInvariant(c);
                    if ((uint)(upper - 'A') > 'Z' - 'A') break;
                }
                hash = DaxKeywords.Hash(hash, upper);
            }

            if (i < s.Length)
            {
                var next = s[i];
                if (next == '"')
                {
                    if (i == 2 && ToUpper(s[0]) == 'D' && ToUpper(s[1]) == 'T')
                    {
                        var length = ScanString(s, 2, out type);
                        if (type != 0) return length;
                    }
                }
                else if (next == '.' || (next == ',' && _nonUs))
                {
                    type = DaxKeywords.FindDotted(s, i, _nonUs, out var length);
                    if (type != 0) return length;
                }
            }

            type = DaxKeywords.Find(s.Slice(0, i), hash);
            if (type == 0) type = DaxToken.TABLE_OR_VARIABLE;
            return i;
        }

        private static int ScanWhiteSpace(ReadOnlySpan<char> s)
        {
            var i = 1;
            for (; i < s.Length; i++)
            {
                var c = s[i];
                if ((c < 128 ? AsciiKinds[c] : GetNonAsciiKind(c)) != WhiteSpace) break;
            }
            return i;
        }

        // An integer, or a real number when a period and a digit follow: [0-9]+ | [0-9]* '.' [0-9]+
        private int ScanNumber(ReadOnlySpan<char> s, out int type)
        {
            var i = 0;
            while (i < s.Length && IsDigit(s[i])) i++;

            if (i < s.Length && (s[i] == '.' || (s[i] == ',' && _nonUs)))
            {
                var fraction = i + 1;
                if (fraction < s.Length && IsDigit(s[fraction]))
                {
                    i = fraction + 1;
                    while (i < s.Length && IsDigit(s[i])) i++;
                    type = DaxToken.REAL_LITERAL;
                    return i;
                }

                if (i == 0)
                {
                    type = 0;
                    return DropThrough(s, 1);
                }
            }

            type = DaxToken.INTEGER_LITERAL;
            return i;
        }

        // A string, or a date literal when it's preceded by the DT prefix. A doubled quote doesn't end the text, but it
        // does end a token if what follows is not closed.
        private static int ScanString(ReadOnlySpan<char> s, int quoteIndex, out int type)
        {
            var accepted = -1;
            var i = quoteIndex + 1;
            while (true)
            {
                var found = s.Slice(i).IndexOf('"');
                if (found < 0)
                {
                    if (accepted >= 0) break;
                    type = 0;
                    return s.Length;
                }

                i += found;
                accepted = i + 1;
                if (accepted == s.Length || s[accepted] != '"') break;
                i = accepted + 1;
            }

            type = quoteIndex == 0 ? DaxToken.STRING_LITERAL : DaxToken.DATE_LITERAL;
            return accepted;
        }

        // A table or an object name: new lines and double quotes can't be part of it, the closing char is doubled to be.
        private static int ScanName(ReadOnlySpan<char> s, char close, int nameType, out int type)
        {
            var accepted = -1;
            var i = 1;
            for (; i < s.Length; i++)
            {
                var c = s[i];
                if (c == close)
                {
                    accepted = i + 1;
                    if (accepted == s.Length || s[accepted] != close) break;
                    i++;
                }
                else if (c == '"' || IsNewLine(c))
                {
                    break;
                }
            }

            if (accepted < 0)
            {
                type = 0;
                return DropThrough(s, i);
            }

            type = nameType;
            return accepted;
        }

        private static int ScanLineComment(ReadOnlySpan<char> s)
        {
            var i = 2;
            while (i < s.Length && !IsNewLine(s[i])) i++;
            return i;
        }

        // A comment that may contain other ones and ends with the expression if not closed: /* ( comment | '/' ~'*' | ~'/' )*? ( */ | EOF )
        // It is a division if what follows can't be such a comment.
        private static int ScanDelimitedComment(ReadOnlySpan<char> s, out int type)
        {
            type = DaxToken.DELIMITED_COMMENT;
            var i = 2;
            while (true)
            {
                var found = s.Slice(i).IndexOfAny('*', '/');
                if (found < 0) return s.Length;

                i += found;
                var c = s[i];
                if (c == '*')
                {
                    if (i + 1 < s.Length && s[i + 1] == '/') return i + 2;
                    i++;
                }
                else if (i + 1 < s.Length)
                {
                    if (s[i + 1] == '*') return ScanNestedComment(s, i, ref type);
                    i += 2;
                }
                else
                {
                    type = DaxToken.DIV;
                    return 1;
                }
            }
        }

        // Past a first nested comment what a */ closes depends on the text that follows, so all the readings are followed.
        // A reading is a depth, and a state. The readings are ordered by preference, the first being that a */ closes
        // a comment, and the ones after a reading that ends the outermost comment are abandoned. The longest one wins.
        private static int ScanNestedComment(ReadOnlySpan<char> s, int slashIndex, ref int type)
        {
            const int inBody = 0, afterStar = 1, afterSlash = 2;

            var accepted = -1;
            var readings = new List<(int Depth, int State)> { (1, afterSlash) };
            var next = new List<(int Depth, int State)>();

            for (var i = slashIndex + 1; i < s.Length && readings.Count > 0; i++)
            {
                var c = s[i];
                next.Clear();
                foreach (var (depth, state) in readings)
                {
                    if (state == inBody)
                    {
                        if (c == '*')
                        {
                            AddReading(next, depth, afterStar);
                            AddReading(next, depth, inBody);
                        }
                        else if (c == '/') AddReading(next, depth, afterSlash);
                        else AddReading(next, depth, inBody);
                    }
                    else if (state == afterStar)
                    {
                        if (c != '/') continue;
                        if (depth == 1)
                        {
                            accepted = i + 1;
                            break;
                        }
                        AddReading(next, depth - 1, inBody);
                    }
                    else if (c == '*') AddReading(next, depth + 1, inBody);
                    else AddReading(next, depth, inBody);
                }

                var swap = readings;
                readings = next;
                next = swap;
            }

            // Only a body can end with the expression, which then closes all the comments
            foreach (var (_, state) in readings)
            {
                if (state == inBody) return s.Length;
            }

            if (accepted >= 0) return accepted;

            type = DaxToken.DIV;
            return 1;
        }

        private static void AddReading(List<(int Depth, int State)> readings, int depth, int state)
        {
            if (!readings.Contains((depth, state))) readings.Add((depth, state));
        }

        // The number of chars to drop when the char at failIndex stops a token that can't be accepted yet
        private static int DropThrough(ReadOnlySpan<char> s, int failIndex) => Math.Min(failIndex + 1, s.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(char c) => (uint)(c - '0') <= 9;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNewLine(char c) => c == '\n' || c == '\r' || c == '\u0085' || c == '\u2028' || c == '\u2029';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ToUpper(char c)
        {
            if (c < 128) return (uint)(c - 'a') <= 'z' - 'a' ? (char)(c - 32) : c;
            return char.ToUpperInvariant(c);
        }

        // The white space, and the chars of other alphabets that have an ASCII letter as their upper case.
        private static byte GetNonAsciiKind(char c)
        {
            switch (c)
            {
                case '\u0085':
                case '\u00A0':
                case '\u1680':
                case '\u180E':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u2028':
                case '\u2029':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                    return WhiteSpace;
            }
            return (uint)(char.ToUpperInvariant(c) - 'A') <= 'Z' - 'A' ? Identifier : (byte)0;
        }

        private static byte[] BuildAsciiKinds()
        {
            var kinds = new byte[128];
            for (var c = 'A'; c <= 'Z'; c++) kinds[c] = Identifier;
            for (var c = 'a'; c <= 'z'; c++) kinds[c] = Identifier;
            for (var c = '0'; c <= '9'; c++) kinds[c] = Digit;
            kinds['_'] = Identifier;
            kinds[' '] = WhiteSpace;
            kinds['\t'] = WhiteSpace;
            kinds['\n'] = WhiteSpace;
            kinds['\v'] = WhiteSpace;
            kinds['\f'] = WhiteSpace;
            kinds['\r'] = WhiteSpace;
            return kinds;
        }
    }
}
