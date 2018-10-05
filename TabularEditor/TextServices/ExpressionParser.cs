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

        public static void SyntaxHighlight(FastColoredTextBox textbox)
        {
            // TODO: This is slow on a large DAX expression and may cause flickering...
            //textbox.SuspendDrawing(); // Doesn't work as we might be on a different thread
            try
            {
                var lexer = new DAXLexer(new DAXCharStream(textbox.Text, Preferences.Current.UseSemicolonsAsSeparators));
                lexer.RemoveErrorListeners();
                textbox.ClearStyle(StyleIndex.All);
                
                var tok = lexer.NextToken();
                while (tok != null && tok.Type != DAXLexer.Eof)
                {
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

                    tok = lexer.NextToken();
                }
            }
            catch
            {
                // Ignore all errors encountered while doing async syntax highlighting
            }
            finally
            {
                //textbox.ResumeDrawing(); // Doesn't work, as we might be on a different thread
            }
            
        }

        public static Brush FromHex(string hex)
        {
            return new SolidBrush(System.Drawing.ColorTranslator.FromHtml(hex));
        }
    }
}
