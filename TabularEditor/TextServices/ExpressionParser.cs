using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using FastColoredTextBoxNS;
using System.Drawing;
using TabularEditor.UIServices;
using TabularEditor.UI;
using System.Windows.Forms;

namespace TabularEditor.TextServices
{
    public class ExpressionParser
    {
        public static string SemicolonsToCommas(string input)
        {
            var sb = new StringBuilder(input);
            var lexer = new DAXLexer(new DAXCharStream(input ,true));
            foreach(var token in lexer.GetAllTokens())
            {
                if (token.Type == DAXLexer.COMMA) sb[token.StartIndex] = ',';
                else if (token.Type == DAXLexer.REAL_LITERAL)
                {
                    for (var i = token.StartIndex; i <= token.StopIndex; i++)
                    {
                        if (sb[i] == ',')
                        {
                            sb[i] = '.';
                            break;
                        }
                    }
                }
                else if (token.Type == DAXLexer.DATA)
                {
                    for (var i = token.StartIndex; i <= token.StopIndex; i++)
                    {
                        if (sb[i] == ',') sb[i] = '.';
                        else if (sb[i] == ';') sb[i] = ',';
                    }
                }
            }
            return sb.ToString();
        }

        public static string CommasToSemicolons(string input)
        {
            var sb = new StringBuilder(input);
            var lexer = new DAXLexer(new DAXCharStream(input, false));
            foreach (var token in lexer.GetAllTokens().ToList())
            {
                if (token.Type == DAXLexer.COMMA) sb[token.StartIndex] = ';';
                else if (token.Type == DAXLexer.REAL_LITERAL)
                {
                    for (var i = token.StartIndex; i <= token.StopIndex; i++)
                    {
                        if (sb[i] == '.')
                        {
                            sb[i] = ',';
                            break;
                        }
                    }
                }
                else if (token.Type == DAXLexer.DATA)
                {
                    for (var i = token.StartIndex; i <= token.StopIndex; i++)
                    {
                        if (sb[i] == '.') sb[i] = ',';
                        else if (sb[i] == ',') sb[i] = ';';
                    }
                }
            }
            return sb.ToString();
        }

        public static Style PlainStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        public static Style KeywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public static Style CommentStyle = new TextStyle(FromHex("#108313"), null, FontStyle.Regular);
        public static Style LiteralStyle = new TextStyle(FromHex("#EE7F18"), null, FontStyle.Regular);
        public static Style ParensStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);

        class SearchToken : IToken
        {
            public SearchToken(int charIndex)
            {
                StartIndex = charIndex;
            }
            public int Channel
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int Column
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICharStream InputStream
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int Line
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int StartIndex { get; private set; }

            public int StopIndex => StartIndex;

            public string Text
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int TokenIndex
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ITokenSource TokenSource
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int Type
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
        class TokenComparer : IComparer<IToken>
        {
            public int Compare(IToken x, IToken y)
            {
                if (x.StartIndex >= y.StartIndex && x.StartIndex <= y.StopIndex) return 0;
                if (y.StartIndex >= x.StartIndex && y.StartIndex <= x.StopIndex) return 0;
                if (x.StartIndex < y.StartIndex) return -1;
                else return 1;
            }

            public static readonly TokenComparer Default = new TokenComparer();
        }

        public static IToken GetTokenAtPos(List<IToken> tokens, int charIndex)
        {
            var searchToken = new SearchToken(charIndex);
            var index = tokens.BinarySearch(searchToken, TokenComparer.Default);
            if (index < 0) return null;
            return tokens[index];
        }

        public static List<IToken> SyntaxHighlight(FastColoredTextBox textbox)
        {
            List<IToken> tokens;

            // TODO: This is slow on a large DAX expression and may cause flickering...
            //textbox.SuspendDrawing(); // Doesn't work as we might be on a different thread
            try
            {
                var lexer = new DAXLexer(new DAXCharStream(textbox.Text, Preferences.Current.UseSemicolonsAsSeparators));
                lexer.RemoveErrorListeners();
                tokens = lexer.GetAllTokens().ToList();
            }
            catch
            {
                textbox.ClearStyle(StyleIndex.All);
                return new List<IToken>();
            }

            textbox.BeginInvoke((MethodInvoker)(() =>
            {
                textbox.SuspendDrawing();
                textbox.ClearStyle(StyleIndex.All);
                foreach (var tok in tokens)
                {
                    if (tok.Type == DAXLexer.Eof) break;
                    var range = textbox.GetRange(tok.StartIndex, tok.StopIndex + 1);
                    if (tok.Channel == DAXLexer.KEYWORD_CHANNEL) range.SetStyle(KeywordStyle);
                    else if (tok.Channel == DAXLexer.COMMENTS_CHANNEL) range.SetStyle(CommentStyle);
                    else switch (tok.Type)
                        {
                            case DAXLexer.INTEGER_LITERAL:
                            case DAXLexer.REAL_LITERAL:
                            case DAXLexer.STRING_LITERAL:
                                range.SetStyle(LiteralStyle); break;
                            case DAXLexer.OPEN_PARENS:
                            case DAXLexer.CLOSE_PARENS:
                                range.SetStyle(ParensStyle); break;
                            default:
                                range.SetStyle(PlainStyle); break;
                        }
                }
                textbox.ResumeDrawing();
            }));

            return tokens;
        }

        public static Brush FromHex(string hex)
        {
            return new SolidBrush(System.Drawing.ColorTranslator.FromHtml(hex));
        }
    }
}
