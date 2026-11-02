using Parlot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TabularEditor.TextServices
{
    /// <summary>
    /// Reads the tokens of a C# script. Text which isn't part of any token is skipped: the first char that can't continue
    /// a token is dropped with what was read of it, so that for instance nothing remains of a string that is never closed.
    /// </summary>
    /// <remarks>
    /// A token is the longest text that matches any of the token types, and a shorter match stands if a longer one turns
    /// out not to complete. What a char means depends on the mode, which interpolated strings and directives change.
    /// This is the grammar of the scripts rather than of C#: identifiers only have ASCII letters, and a <c>_</c> can only
    /// start one.
    /// </remarks>
    public sealed class CSharpTokenizer
    {
        public const int DefaultChannel = 0;
        public const int HiddenChannel = 1;
        public const int CommentsChannel = 2;
        public const int DirectiveChannel = 3;

        private const int DefaultMode = 0;
        private const int InterpolationStringMode = 1;
        private const int InterpolationFormatMode = 2;
        private const int DirectiveMode = 3;
        private const int DirectiveTextMode = 4;

        private const string Arglist = "__arglist";

        // The keywords of each first letter
        private static readonly (string Text, int Type)[][] Keywords = BuildKeywords();

        private readonly Cursor _cursor;
        private int _mode;

        // Only used by interpolated strings, and created with the first one.
        private Stack<int> _modes;
        private Stack<bool> _verbatims;
        private Stack<int> _curlyLevels;
        private int _interpolatedStringLevel;
        private bool _verbatim;

        /// <summary>
        /// Creates a tokenizer for a script.
        /// </summary>
        /// <param name="script">The script to read.</param>
        public CSharpTokenizer(string script)
        {
            _cursor = new Cursor(script);
        }

        /// <summary>
        /// Adds all the tokens of a script to a list, on any channel and with the <see cref="CSharpToken.Eof"/> token last.
        /// </summary>
        /// <param name="script">The script to read.</param>
        /// <param name="tokens">The list to add to, which keeps the tokens read so far when an exception is thrown.</param>
        /// <exception cref="IndexOutOfRangeException">A colon in an interpolated string has none of <c>}</c>, <c>:</c> or <c>)</c> after it.</exception>
        public static void GetAllTokens(string script, List<CSharpLexerToken> tokens)
        {
            var tokenizer = new CSharpTokenizer(script);
            CSharpLexerToken token;
            do
            {
                token = tokenizer.Read();
                tokens.Add(token);
            }
            while (token.Type != CSharpToken.Eof);
        }

        /// <summary>
        /// Reads the next token, which is the <see cref="CSharpToken.Eof"/> one at the end of the script.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">A colon in an interpolated string has none of <c>}</c>, <c>:</c> or <c>)</c> after it.</exception>
        public CSharpLexerToken Read()
        {
            var cursor = _cursor;
            while (!cursor.Eof)
            {
                var length = Scan(cursor.Span, out var type, out var channel);
                var start = cursor.Offset;
                cursor.Advance(length);
                if (type != 0) return new CSharpLexerToken(cursor.Buffer, type, channel, start, length);
            }
            return new CSharpLexerToken(cursor.Buffer, CSharpToken.Eof, DefaultChannel, cursor.Offset, 0);
        }

        // Returns the length of the token at the start of the text, its type and its channel. The type is 0 when no token
        // matches or the token is skipped, the result being then the number of chars to drop.
        private int Scan(ReadOnlySpan<char> s, out int type, out int channel)
        {
            channel = DefaultChannel;
            switch (_mode)
            {
                case InterpolationStringMode: return ScanInterpolationString(s, out type);
                case InterpolationFormatMode: return ScanInterpolationFormat(s, out type);
                case DirectiveMode: return ScanDirective(s, out type, out channel);
                case DirectiveTextMode: return ScanDirectiveText(s, out type, out channel);
                default: return ScanDefault(s, out type, out channel);
            }
        }

        private int ScanDefault(ReadOnlySpan<char> s, out int type, out int channel)
        {
            channel = DefaultChannel;
            var c = s[0];

            if (IsLetter(c))
            {
                var end = ScanIdentifierPart(s, 1);
                type = FindKeyword(s.Slice(0, end));
                return end;
            }

            if (IsDigit(c)) return ScanNumber(s, out type);

            if (IsWhiteSpace(c) || IsNewLine(c))
            {
                type = CSharpToken.WHITESPACES;
                channel = HiddenChannel;
                var i = 1;
                while (i < s.Length && (IsWhiteSpace(s[i]) || IsNewLine(s[i]))) i++;
                return i;
            }

            type = 0;
            switch (c)
            {
                case '_':
                    if (s.StartsWith(Arglist.AsSpan(), StringComparison.Ordinal))
                    {
                        type = CSharpToken.ARGLIST;
                        return Arglist.Length;
                    }
                    type = CSharpToken.IDENTIFIER;
                    return ScanIdentifierPart(s, 1);
                case '\\':
                    return ScanEscapedIdentifier(s, 0, CSharpToken.IDENTIFIER, out type);
                case '@':
                    if (s.Length > 1)
                    {
                        var next = s[1];
                        if (next == '"') return ScanVerbatimString(s, out type);
                        if (next == '\\') return ScanEscapedIdentifier(s, 1, CSharpToken.IDENTIFIER, out type);
                        if (IsLetter(next) || next == '_')
                        {
                            type = CSharpToken.IDENTIFIER;
                            return ScanIdentifierPart(s, 2);
                        }
                    }
                    return DropThrough(s, 1);
                case '$':
                    if (Next(s, 1) == '"')
                    {
                        EnterInterpolatedString(false);
                        type = CSharpToken.INTERPOLATED_REGULAR_STRING_START;
                        return 2;
                    }
                    if (Next(s, 1) != '@') return DropThrough(s, 1);
                    if (Next(s, 2) != '"') return DropThrough(s, 2);
                    EnterInterpolatedString(true);
                    type = CSharpToken.INTERPOLATED_VERBATIUM_STRING_START;
                    return 3;
                case '"': return ScanRegularString(s, out type);
                case '\'': return ScanCharacter(s, out type);
                case '/': return ScanSlash(s, out type, out channel);
                case '.':
                    if (IsDigit(Next(s, 1)))
                    {
                        type = CSharpToken.REAL_LITERAL;
                        return ScanFraction(s, 1);
                    }
                    type = CSharpToken.DOT;
                    return 1;
                case '#':
                    _mode = DirectiveMode;
                    type = CSharpToken.SHARP;
                    return 1;
                case '{':
                    if (_interpolatedStringLevel > 0) _curlyLevels.Push(_curlyLevels.Pop() + 1);
                    type = CSharpToken.OPEN_BRACE;
                    return 1;
                case '}':
                    type = CSharpToken.CLOSE_BRACE;
                    if (_interpolatedStringLevel > 0)
                    {
                        _curlyLevels.Push(_curlyLevels.Pop() - 1);
                        if (_curlyLevels.Peek() == 0)
                        {
                            _curlyLevels.Pop();
                            _mode = _modes.Pop();
                            type = 0;
                        }
                    }
                    return 1;
                case ':':
                    if (Next(s, 1) == ':')
                    {
                        type = CSharpToken.DOUBLE_COLON;
                        return 2;
                    }
                    if (_interpolatedStringLevel > 0 && StartsFormatString(s)) _mode = InterpolationFormatMode;
                    type = CSharpToken.COLON;
                    return 1;
                case '[': type = CSharpToken.OPEN_BRACKET; return 1;
                case ']': type = CSharpToken.CLOSE_BRACKET; return 1;
                case '(': type = CSharpToken.OPEN_PARENS; return 1;
                case ')': type = CSharpToken.CLOSE_PARENS; return 1;
                case ',': type = CSharpToken.COMMA; return 1;
                case ';': type = CSharpToken.SEMICOLON; return 1;
                case '~': type = CSharpToken.TILDE; return 1;
                case '+': return ScanOperator(s, '+', CSharpToken.OP_INC, CSharpToken.OP_ADD_ASSIGNMENT, CSharpToken.PLUS, out type);
                case '&': return ScanOperator(s, '&', CSharpToken.OP_AND, CSharpToken.OP_AND_ASSIGNMENT, CSharpToken.AMP, out type);
                case '|': return ScanOperator(s, '|', CSharpToken.OP_OR, CSharpToken.OP_OR_ASSIGNMENT, CSharpToken.BITWISE_OR, out type);
                case '=': return ScanOperator(s, '>', CSharpToken.OP_LAMBDA, CSharpToken.OP_EQ, CSharpToken.ASSIGNMENT, out type);
                case '*': return ScanOperator(s, CSharpToken.OP_MULT_ASSIGNMENT, CSharpToken.STAR, out type);
                case '%': return ScanOperator(s, CSharpToken.OP_MOD_ASSIGNMENT, CSharpToken.PERCENT, out type);
                case '^': return ScanOperator(s, CSharpToken.OP_XOR_ASSIGNMENT, CSharpToken.CARET, out type);
                case '!': return ScanOperator(s, CSharpToken.OP_NE, CSharpToken.BANG, out type);
                case '>': return ScanOperator(s, CSharpToken.OP_GE, CSharpToken.GT, out type);
                case '-':
                    if (Next(s, 1) == '>')
                    {
                        type = CSharpToken.OP_PTR;
                        return 2;
                    }
                    return ScanOperator(s, '-', CSharpToken.OP_DEC, CSharpToken.OP_SUB_ASSIGNMENT, CSharpToken.MINUS, out type);
                case '<':
                    if (Next(s, 1) == '<')
                    {
                        if (Next(s, 2) == '=')
                        {
                            type = CSharpToken.OP_LEFT_SHIFT_ASSIGNMENT;
                            return 3;
                        }
                        type = CSharpToken.OP_LEFT_SHIFT;
                        return 2;
                    }
                    return ScanOperator(s, CSharpToken.OP_LE, CSharpToken.LT, out type);
                case '?':
                    if (Next(s, 1) == '?')
                    {
                        type = CSharpToken.OP_COALESCING;
                        return 2;
                    }
                    type = CSharpToken.INTERR;
                    return 1;
                case '\u00EF':
                    if (Next(s, 1) != '\u00BB') return DropThrough(s, 1);
                    if (Next(s, 2) != '\u00BF') return DropThrough(s, 2);
                    type = CSharpToken.BYTE_ORDER_MARK;
                    return 3;
                default:
                    return 1;
            }
        }

        private int ScanInterpolationString(ReadOnlySpan<char> s, out int type)
        {
            switch (s[0])
            {
                case '{':
                    if (Next(s, 1) == '{')
                    {
                        type = CSharpToken.DOUBLE_CURLY_INSIDE;
                        return 2;
                    }
                    _curlyLevels.Push(1);
                    _modes.Push(_mode);
                    _mode = DefaultMode;
                    type = 0;
                    return 1;
                case '"':
                    if (_verbatim && Next(s, 1) == '"')
                    {
                        type = CSharpToken.VERBATIUM_DOUBLE_QUOTE_INSIDE;
                        return 2;
                    }
                    _interpolatedStringLevel--;
                    _verbatims.Pop();
                    _verbatim = _verbatims.Count > 0 && _verbatims.Peek();
                    _mode = _modes.Pop();
                    type = CSharpToken.DOUBLE_QUOTE_INSIDE;
                    return 1;
                case '\\' when !_verbatim:
                    if (s.Length > 1 && IsSimpleEscape(s[1]))
                    {
                        type = CSharpToken.REGULAR_CHAR_INSIDE;
                        return 2;
                    }
                    type = 0;
                    return DropThrough(s, 1);
            }

            var i = 1;
            if (_verbatim)
            {
                type = CSharpToken.VERBATIUM_INSIDE_STRING;
                while (i < s.Length && s[i] != '{' && s[i] != '"') i++;
            }
            else
            {
                type = CSharpToken.REGULAR_STRING_INSIDE;
                while (i < s.Length && s[i] != '{' && s[i] != '\\' && s[i] != '"') i++;
            }
            return i;
        }

        private int ScanInterpolationFormat(ReadOnlySpan<char> s, out int type)
        {
            if (s[0] == '}')
            {
                if (Next(s, 1) == '}')
                {
                    type = CSharpToken.FORMAT_STRING;
                    return 2;
                }
                _curlyLevels.Pop();
                _mode = _modes.Pop();
                type = 0;
                return 1;
            }

            type = CSharpToken.FORMAT_STRING;
            var end = s.IndexOf('}');
            return end < 0 ? s.Length : end;
        }

        private int ScanDirective(ReadOnlySpan<char> s, out int type, out int channel)
        {
            channel = DirectiveChannel;
            var c = s[0];

            if (IsLetter(c) || c == '_')
            {
                return ScanDirectiveWord(s, ScanIdentifierPart(s, 1), out type);
            }

            if (IsWhiteSpace(c))
            {
                type = CSharpToken.DIRECTIVE_WHITESPACES;
                channel = HiddenChannel;
                return ScanWhiteSpace(s, 1);
            }

            if (IsDigit(c))
            {
                type = CSharpToken.DIGITS;
                return ScanDigits(s, 1);
            }

            if (IsNewLine(c)) return ScanDirectiveNewLine(s, out type);

            type = 0;
            switch (c)
            {
                case '(': type = CSharpToken.OPEN_PARENS; return 1;
                case ')': type = CSharpToken.CLOSE_PARENS; return 1;
                case '!':
                    if (Next(s, 1) == '=')
                    {
                        type = CSharpToken.OP_NE;
                        return 2;
                    }
                    type = CSharpToken.BANG;
                    return 1;
                case '=':
                    if (Next(s, 1) != '=') return DropThrough(s, 1);
                    type = CSharpToken.OP_EQ;
                    return 2;
                case '&':
                    if (Next(s, 1) != '&') return DropThrough(s, 1);
                    type = CSharpToken.OP_AND;
                    return 2;
                case '|':
                    if (Next(s, 1) != '|') return DropThrough(s, 1);
                    type = CSharpToken.OP_OR;
                    return 2;
                case '"':
                    var i = 1;
                    while (i < s.Length && s[i] != '"' && !IsNewLine(s[i])) i++;
                    if (i == s.Length || s[i] != '"') return DropThrough(s, i);
                    type = CSharpToken.STRING;
                    return i + 1;
                case '/':
                    if (Next(s, 1) != '/') return DropThrough(s, 1);
                    type = CSharpToken.SINGLE_LINE_COMMENT;
                    channel = CommentsChannel;
                    return ScanLine(s, 2);
                case '\\':
                    var length = ScanEscapedIdentifier(s, 0, CSharpToken.CONDITIONAL_SYMBOL, out type);
                    return length;
                default:
                    return 1;
            }
        }

        // A word of a directive: a symbol unless it's exactly one of the names of the directive. Some of those
        // switch to free text, with the white space after them being part of their token.
        private int ScanDirectiveWord(ReadOnlySpan<char> s, int length, out int type)
        {
            type = CSharpToken.CONDITIONAL_SYMBOL;
            var word = s.Slice(0, length);
            switch (length)
            {
                case 2:
                    if (Is(word, "if")) type = CSharpToken.IF;
                    break;
                case 4:
                    if (Is(word, "true")) type = CSharpToken.TRUE;
                    else if (Is(word, "elif")) type = CSharpToken.ELIF;
                    else if (Is(word, "else")) type = CSharpToken.ELSE;
                    else if (Is(word, "line")) type = CSharpToken.LINE;
                    break;
                case 5:
                    if (Is(word, "false")) type = CSharpToken.FALSE;
                    else if (Is(word, "undef")) type = CSharpToken.UNDEF;
                    else if (Is(word, "endif")) type = CSharpToken.ENDIF;
                    else if (Is(word, "error")) return ScanDirectiveTextStart(s, length, CSharpToken.ERROR, true, ref type);
                    break;
                case 6:
                    if (Is(word, "define")) type = CSharpToken.DEFINE;
                    else if (Is(word, "hidden")) type = CSharpToken.DIRECTIVE_HIDDEN;
                    else if (Is(word, "pragma")) return ScanDirectiveTextStart(s, length, CSharpToken.PRAGMA, true, ref type);
                    else if (Is(word, "region")) return ScanDirectiveTextStart(s, length, CSharpToken.REGION, false, ref type);
                    break;
                case 7:
                    if (Is(word, "default")) type = CSharpToken.DEFAULT;
                    else if (Is(word, "warning")) return ScanDirectiveTextStart(s, length, CSharpToken.WARNING, true, ref type);
                    break;
                case 9:
                    if (Is(word, "endregion")) return ScanDirectiveTextStart(s, length, CSharpToken.ENDREGION, false, ref type);
                    break;
            }
            return length;
        }

        private int ScanDirectiveTextStart(ReadOnlySpan<char> s, int length, int directiveType, bool needsWhiteSpace, ref int type)
        {
            var end = ScanWhiteSpace(s, length);
            if (end == length && needsWhiteSpace) return length;
            type = directiveType;
            _mode = DirectiveTextMode;
            return end;
        }

        private int ScanDirectiveText(ReadOnlySpan<char> s, out int type, out int channel)
        {
            channel = DirectiveChannel;
            if (IsNewLine(s[0])) return ScanDirectiveNewLine(s, out type);
            type = CSharpToken.TEXT;
            return ScanLine(s, 1);
        }

        private int ScanDirectiveNewLine(ReadOnlySpan<char> s, out int type)
        {
            _mode = DefaultMode;
            type = CSharpToken.DIRECTIVE_NEW_LINE;
            return s[0] == '\r' && Next(s, 1) == '\n' ? 2 : 1;
        }

        private void EnterInterpolatedString(bool verbatim)
        {
            if (_modes == null)
            {
                _modes = new Stack<int>();
                _verbatims = new Stack<bool>();
                _curlyLevels = new Stack<int>();
            }

            _interpolatedStringLevel++;
            _verbatims.Push(verbatim);
            _verbatim = verbatim;
            _modes.Push(_mode);
            _mode = InterpolationStringMode;
        }

        // Whether the colon at the start of the text starts a format string: the first of '}', ':' and ')' after it has to
        // be a '}'. This looks at chars rather than tokens. The grammar keeps looking past the end of the script, which
        // takes it about 2^31 reads until the index overflows and it gets this exception. Only the wait is left out.
        private static bool StartsFormatString(ReadOnlySpan<char> s)
        {
            for (var i = 1; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '}') return true;
                if (c == ':' || c == ')') return false;
            }
            throw new IndexOutOfRangeException("A colon in an interpolated string has none of '}', ':' or ')' after it.");
        }

        // A comment, or a division. A delimited comment is a shorter match than a documentation one which starts the same
        // way, when the "*/" of the comment is the second star of "/**/" and another "*/" follows.
        private static int ScanSlash(ReadOnlySpan<char> s, out int type, out int channel)
        {
            channel = CommentsChannel;
            switch (Next(s, 1))
            {
                case '/':
                    type = Next(s, 2) == '/' ? CSharpToken.SINGLE_LINE_DOC_COMMENT : CSharpToken.SINGLE_LINE_COMMENT;
                    return ScanLine(s, 2);
                case '*':
                    var close = s.Slice(2).IndexOf("*/".AsSpan(), StringComparison.Ordinal);
                    if (close >= 0)
                    {
                        type = CSharpToken.DELIMITED_COMMENT;
                        var end = close + 4;
                        if (Next(s, 2) == '*')
                        {
                            var docClose = s.Slice(3).IndexOf("*/".AsSpan(), StringComparison.Ordinal);
                            if (docClose >= 0)
                            {
                                type = CSharpToken.DELIMITED_DOC_COMMENT;
                                end = docClose + 5;
                            }
                        }
                        return end;
                    }
                    break;
                case '=':
                    channel = DefaultChannel;
                    type = CSharpToken.OP_DIV_ASSIGNMENT;
                    return 2;
            }

            channel = DefaultChannel;
            type = CSharpToken.DIV;
            return 1;
        }

        // [0-9]+ suffix? and what can extend it: a hexadecimal integer, a member access, or a real number.
        private static int ScanNumber(ReadOnlySpan<char> s, out int type)
        {
            var digits = ScanDigits(s, 1);

            if (digits == 1 && s[0] == '0' && (Next(s, 1) | 0x20) == 'x' && IsHexDigit(Next(s, 2)))
            {
                type = CSharpToken.HEX_INTEGER_LITERAL;
                var i = 3;
                while (i < s.Length && IsHexDigit(s[i])) i++;
                return ScanIntegerSuffix(s, i);
            }

            var next = Next(s, digits);
            if (next == '.' && IsDigit(Next(s, digits + 1)))
            {
                type = CSharpToken.REAL_LITERAL;
                return ScanFraction(s, digits + 1);
            }

            if (IsRealSuffix(next))
            {
                type = CSharpToken.REAL_LITERAL;
                return digits + 1;
            }

            if ((next | 0x20) == 'e')
            {
                var exponent = ScanExponent(s, digits);
                if (exponent > digits)
                {
                    type = CSharpToken.REAL_LITERAL;
                    return IsRealSuffix(Next(s, exponent)) ? exponent + 1 : exponent;
                }
            }

            var end = ScanIntegerSuffix(s, digits);
            if (Next(s, end) == '.')
            {
                var access = ScanMemberAccess(s, end + 1);
                if (access > 0)
                {
                    type = CSharpToken.LITERAL_ACCESS;
                    return access;
                }
            }

            type = CSharpToken.INTEGER_LITERAL;
            return end;
        }

        // The end of '@'? IdentifierOrKeyword at an index, or 0 if it doesn't match
        private static int ScanMemberAccess(ReadOnlySpan<char> s, int i)
        {
            if (Next(s, i) == '@') i++;
            var c = Next(s, i);
            if (IsLetter(c) || c == '_') return ScanIdentifierPart(s, i + 1);
            if (c == '\\')
            {
                var end = ScanUnicodeEscape(s, i, out _);
                if (end > 0) return ScanIdentifierPart(s, end);
            }
            return 0;
        }

        private static int ScanFraction(ReadOnlySpan<char> s, int i)
        {
            i = ScanDigits(s, i);
            i = ScanExponent(s, i);
            return IsRealSuffix(Next(s, i)) ? i + 1 : i;
        }

        // The end of the exponent at an index, which is that index when there is none
        private static int ScanExponent(ReadOnlySpan<char> s, int i)
        {
            if ((Next(s, i) | 0x20) != 'e') return i;
            var digits = i + 1;
            var sign = Next(s, digits);
            if (sign == '+' || sign == '-') digits++;
            return IsDigit(Next(s, digits)) ? ScanDigits(s, digits + 1) : i;
        }

        // [lL]? [uU] | [uU]? [lL]
        private static int ScanIntegerSuffix(ReadOnlySpan<char> s, int i)
        {
            switch (Next(s, i) | 0x20)
            {
                case 'l': return (Next(s, i + 1) | 0x20) == 'u' ? i + 2 : i + 1;
                case 'u': return (Next(s, i + 1) | 0x20) == 'l' ? i + 2 : i + 1;
                default: return i;
            }
        }

        // A doubled quote doesn't end the text, but it does end a token if what follows is not closed.
        private static int ScanVerbatimString(ReadOnlySpan<char> s, out int type)
        {
            var accepted = -1;
            var i = 2;
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

            type = CSharpToken.VERBATIUM_STRING;
            return accepted;
        }

        private static int ScanRegularString(ReadOnlySpan<char> s, out int type)
        {
            type = 0;
            var i = 1;
            while (i < s.Length)
            {
                var c = s[i];
                if (c == '"')
                {
                    type = CSharpToken.REGULAR_STRING;
                    return i + 1;
                }
                if (IsNewLine(c)) return DropThrough(s, i);
                if (c == '\\')
                {
                    i = ScanCommonCharacter(s, i, out var failIndex);
                    if (i < 0) return DropThrough(s, failIndex);
                }
                else
                {
                    i++;
                }
            }
            return s.Length;
        }

        private static int ScanCharacter(ReadOnlySpan<char> s, out int type)
        {
            type = 0;
            if (s.Length == 1) return 1;

            var c = s[1];
            if (c == '\'' || IsNewLine(c)) return DropThrough(s, 1);

            var i = 2;
            if (c == '\\')
            {
                i = ScanCommonCharacter(s, 1, out var failIndex);
                if (i < 0) return DropThrough(s, failIndex);
            }

            if (Next(s, i) != '\'' || i >= s.Length) return DropThrough(s, i);
            type = CSharpToken.CHARACTER_LITERAL;
            return i + 1;
        }

        // The end of the escape sequence at an index, or -1 with the index of the char that can't continue it
        private static int ScanCommonCharacter(ReadOnlySpan<char> s, int i, out int failIndex)
        {
            failIndex = i + 1;
            if (failIndex == s.Length) return -1;

            var c = s[failIndex];
            if (IsSimpleEscape(c)) return i + 2;
            if (c == 'u' || c == 'U') return ScanUnicodeEscape(s, i, out failIndex) is var end && end > 0 ? end : -1;
            if (c != 'x') return -1;

            failIndex = i + 2;
            if (!IsHexDigit(Next(s, failIndex))) return -1;

            var hex = failIndex + 1;
            while (hex < failIndex + 4 && IsHexDigit(Next(s, hex))) hex++;
            return hex;
        }

        // An identifier that starts with a unicode escape, after an optional '@'
        private static int ScanEscapedIdentifier(ReadOnlySpan<char> s, int i, int identifierType, out int type)
        {
            var end = ScanUnicodeEscape(s, i, out var failIndex);
            if (end < 0)
            {
                type = 0;
                return DropThrough(s, failIndex);
            }
            type = identifierType;
            return ScanIdentifierPart(s, end);
        }

        // Letters, digits and unicode escapes. An escape that isn't complete isn't part of the identifier.
        private static int ScanIdentifierPart(ReadOnlySpan<char> s, int i)
        {
            while (i < s.Length)
            {
                var c = s[i];
                if (IsLetter(c) || IsDigit(c))
                {
                    i++;
                }
                else if (c == '\\')
                {
                    var end = ScanUnicodeEscape(s, i, out _);
                    if (end < 0) break;
                    i = end;
                }
                else
                {
                    break;
                }
            }
            return i;
        }

        // The end of the \uXXXX or \UXXXXXXXX at an index, or -1 with the index of the char that can't continue it
        private static int ScanUnicodeEscape(ReadOnlySpan<char> s, int i, out int failIndex)
        {
            var kind = Next(s, i + 1);
            var count = kind == 'u' ? 4 : kind == 'U' ? 8 : 0;
            if (count == 0)
            {
                failIndex = i + 1;
                return -1;
            }

            var end = i + 2 + count;
            for (var j = i + 2; j < end; j++)
            {
                if (!IsHexDigit(Next(s, j)))
                {
                    failIndex = j;
                    return -1;
                }
            }

            failIndex = -1;
            return end;
        }

        private static int ScanOperator(ReadOnlySpan<char> s, char second, int doubledType, int assignmentType, int singleType, out int type)
        {
            var next = Next(s, 1);
            if (next == second)
            {
                type = doubledType;
                return 2;
            }
            return ScanOperator(s, assignmentType, singleType, out type);
        }

        private static int ScanOperator(ReadOnlySpan<char> s, int assignmentType, int singleType, out int type)
        {
            if (Next(s, 1) == '=')
            {
                type = assignmentType;
                return 2;
            }
            type = singleType;
            return 1;
        }

        private static int ScanDigits(ReadOnlySpan<char> s, int i)
        {
            while (i < s.Length && IsDigit(s[i])) i++;
            return i;
        }

        private static int ScanWhiteSpace(ReadOnlySpan<char> s, int i)
        {
            while (i < s.Length && IsWhiteSpace(s[i])) i++;
            return i;
        }

        private static int ScanLine(ReadOnlySpan<char> s, int i)
        {
            while (i < s.Length && !IsNewLine(s[i])) i++;
            return i;
        }

        private static int FindKeyword(ReadOnlySpan<char> word)
        {
            var first = word[0] - 'a';
            if ((uint)first < 26)
            {
                foreach (var (text, type) in Keywords[first])
                {
                    if (text.Length == word.Length && Is(word, text)) return type;
                }
            }
            return CSharpToken.IDENTIFIER;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Is(ReadOnlySpan<char> word, string text) => word.SequenceEqual(text.AsSpan());

        // The char at an index, or a char no scanner looks for past the end of the text.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char Next(ReadOnlySpan<char> s, int i) => i < s.Length ? s[i] : '\uFFFF';

        // The number of chars to drop when the char at failIndex stops a token that can't be accepted yet
        private static int DropThrough(ReadOnlySpan<char> s, int failIndex) => Math.Min(failIndex + 1, s.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLetter(char c) => (uint)((c | 0x20) - 'a') <= 'z' - 'a';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(char c) => (uint)(c - '0') <= 9;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsHexDigit(char c) => IsDigit(c) || (uint)((c | 0x20) - 'a') <= 'f' - 'a';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRealSuffix(char c)
        {
            var lower = c | 0x20;
            return lower == 'f' || lower == 'd' || lower == 'm';
        }

        private static bool IsSimpleEscape(char c)
        {
            switch (c)
            {
                case '\'':
                case '"':
                case '\\':
                case '0':
                case 'a':
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                case 'v':
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNewLine(char c) => c == '\n' || c == '\r' || c == '\u0085' || c == '\u2028' || c == '\u2029';

        // Zs, which here leaves out U+2007, and tab, vertical tab, form feed and U+200B.
        private static bool IsWhiteSpace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                case '\v':
                case '\f':
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
                case '\u200B':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                    return true;
                default:
                    return false;
            }
        }

        private static (string Text, int Type)[][] BuildKeywords()
        {
            var lists = new List<(string, int)>[26];
            void Add(string text, int type)
            {
                var first = text[0] - 'a';
                (lists[first] ?? (lists[first] = new List<(string, int)>())).Add((text, type));
            }

            Add("abstract", CSharpToken.ABSTRACT);
            Add("add", CSharpToken.ADD);
            Add("alias", CSharpToken.ALIAS);
            Add("as", CSharpToken.AS);
            Add("ascending", CSharpToken.ASCENDING);
            Add("async", CSharpToken.ASYNC);
            Add("await", CSharpToken.AWAIT);
            Add("base", CSharpToken.BASE);
            Add("bool", CSharpToken.BOOL);
            Add("break", CSharpToken.BREAK);
            Add("by", CSharpToken.BY);
            Add("byte", CSharpToken.BYTE);
            Add("case", CSharpToken.CASE);
            Add("catch", CSharpToken.CATCH);
            Add("char", CSharpToken.CHAR);
            Add("checked", CSharpToken.CHECKED);
            Add("class", CSharpToken.CLASS);
            Add("const", CSharpToken.CONST);
            Add("continue", CSharpToken.CONTINUE);
            Add("decimal", CSharpToken.DECIMAL);
            Add("default", CSharpToken.DEFAULT);
            Add("delegate", CSharpToken.DELEGATE);
            Add("descending", CSharpToken.DESCENDING);
            Add("do", CSharpToken.DO);
            Add("double", CSharpToken.DOUBLE);
            Add("dynamic", CSharpToken.DYNAMIC);
            Add("else", CSharpToken.ELSE);
            Add("enum", CSharpToken.ENUM);
            Add("equals", CSharpToken.EQUALS);
            Add("event", CSharpToken.EVENT);
            Add("explicit", CSharpToken.EXPLICIT);
            Add("extern", CSharpToken.EXTERN);
            Add("false", CSharpToken.FALSE);
            Add("finally", CSharpToken.FINALLY);
            Add("fixed", CSharpToken.FIXED);
            Add("float", CSharpToken.FLOAT);
            Add("for", CSharpToken.FOR);
            Add("foreach", CSharpToken.FOREACH);
            Add("from", CSharpToken.FROM);
            Add("get", CSharpToken.GET);
            Add("goto", CSharpToken.GOTO);
            Add("group", CSharpToken.GROUP);
            Add("if", CSharpToken.IF);
            Add("implicit", CSharpToken.IMPLICIT);
            Add("in", CSharpToken.IN);
            Add("int", CSharpToken.INT);
            Add("interface", CSharpToken.INTERFACE);
            Add("internal", CSharpToken.INTERNAL);
            Add("into", CSharpToken.INTO);
            Add("is", CSharpToken.IS);
            Add("join", CSharpToken.JOIN);
            Add("let", CSharpToken.LET);
            Add("lock", CSharpToken.LOCK);
            Add("long", CSharpToken.LONG);
            Add("nameof", CSharpToken.NAMEOF);
            Add("namespace", CSharpToken.NAMESPACE);
            Add("new", CSharpToken.NEW);
            Add("null", CSharpToken.NULL);
            Add("object", CSharpToken.OBJECT);
            Add("on", CSharpToken.ON);
            Add("operator", CSharpToken.OPERATOR);
            Add("orderby", CSharpToken.ORDERBY);
            Add("out", CSharpToken.OUT);
            Add("override", CSharpToken.OVERRIDE);
            Add("params", CSharpToken.PARAMS);
            Add("partial", CSharpToken.PARTIAL);
            Add("private", CSharpToken.PRIVATE);
            Add("protected", CSharpToken.PROTECTED);
            Add("public", CSharpToken.PUBLIC);
            Add("readonly", CSharpToken.READONLY);
            Add("ref", CSharpToken.REF);
            Add("remove", CSharpToken.REMOVE);
            Add("return", CSharpToken.RETURN);
            Add("sbyte", CSharpToken.SBYTE);
            Add("sealed", CSharpToken.SEALED);
            Add("select", CSharpToken.SELECT);
            Add("set", CSharpToken.SET);
            Add("short", CSharpToken.SHORT);
            Add("sizeof", CSharpToken.SIZEOF);
            Add("stackalloc", CSharpToken.STACKALLOC);
            Add("static", CSharpToken.STATIC);
            Add("string", CSharpToken.STRING);
            Add("struct", CSharpToken.STRUCT);
            Add("switch", CSharpToken.SWITCH);
            Add("this", CSharpToken.THIS);
            Add("throw", CSharpToken.THROW);
            Add("true", CSharpToken.TRUE);
            Add("try", CSharpToken.TRY);
            Add("typeof", CSharpToken.TYPEOF);
            Add("uint", CSharpToken.UINT);
            Add("ulong", CSharpToken.ULONG);
            Add("unchecked", CSharpToken.UNCHECKED);
            Add("unsafe", CSharpToken.UNSAFE);
            Add("ushort", CSharpToken.USHORT);
            Add("using", CSharpToken.USING);
            Add("var", CSharpToken.VAR);
            Add("virtual", CSharpToken.VIRTUAL);
            Add("void", CSharpToken.VOID);
            Add("volatile", CSharpToken.VOLATILE);
            Add("when", CSharpToken.WHEN);
            Add("where", CSharpToken.WHERE);
            Add("while", CSharpToken.WHILE);
            Add("yield", CSharpToken.YIELD);

            var result = new (string, int)[26][];
            for (var i = 0; i < 26; i++)
            {
                result[i] = lists[i]?.ToArray() ?? Array.Empty<(string, int)>();
            }
            return result;
        }
    }
}
