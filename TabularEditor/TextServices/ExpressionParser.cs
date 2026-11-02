using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastColoredTextBoxNS;
using System.Drawing;
using TabularEditor.TOMWrapper.Utils;
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
            foreach(var token in DaxTokenizer.GetAllTokens(input, true))
            {
                if (token.Type == DaxToken.COMMA) sb[token.StartIndex] = ',';
                else if (token.Type == DaxToken.REAL_LITERAL)
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
            }
            return sb.ToString();
        }

        public static string CommasToSemicolons(string input)
        {
            var sb = new StringBuilder(input);
            foreach (var token in DaxTokenizer.GetAllTokens(input))
            {
                if (token.Type == DaxToken.COMMA) sb[token.StartIndex] = ';';
                else if (token.Type == DaxToken.REAL_LITERAL)
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
            }
            return sb.ToString();
        }

        public static Style PlainStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        public static Style KeywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public static Style CommentStyle = new TextStyle(FromHex("#108313"), null, FontStyle.Regular);
        public static Style LiteralStyle = new TextStyle(FromHex("#EE7F18"), null, FontStyle.Regular);
        public static Style ParensStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);

        public static DaxLexerToken? GetTokenAtPos(List<DaxLexerToken> tokens, int charIndex)
        {
            var low = 0;
            var high = tokens.Count - 1;
            while (low <= high)
            {
                var middle = low + (high - low) / 2;
                var token = tokens[middle];
                if (charIndex < token.StartIndex) high = middle - 1;
                else if (charIndex > token.StopIndex) low = middle + 1;
                else return token;
            }
            return null;
        }

        public static List<DaxLexerToken> SyntaxHighlight(FastColoredTextBox textbox)
        {
            List<DaxLexerToken> tokens;

            // TODO: This is slow on a large DAX expression and may cause flickering...
            //textbox.SuspendDrawing(); // Doesn't work as we might be on a different thread
            try
            {
                tokens = DaxTokenizer.GetAllTokens(textbox.Text, Preferences.Current.UseSemicolonsAsSeparators);
            }
            catch
            {
                textbox.ClearStyle(StyleIndex.All);
                return new List<DaxLexerToken>();
            }

            textbox.BeginInvoke((MethodInvoker)(() =>
            {
                textbox.SuspendDrawing();
                textbox.ClearStyle(StyleIndex.All);
                foreach (var tok in tokens)
                {
                    var range = textbox.GetRange(tok.StartIndex, tok.StopIndex + 1);
                    if (tok.Channel == DaxTokenizer.KeywordChannel) range.SetStyle(KeywordStyle);
                    else if (tok.Channel == DaxTokenizer.CommentsChannel) range.SetStyle(CommentStyle);
                    else switch (tok.Type)
                        {
                            case DaxToken.INTEGER_LITERAL:
                            case DaxToken.REAL_LITERAL:
                            case DaxToken.STRING_LITERAL:
                                range.SetStyle(LiteralStyle); break;
                            case DaxToken.OPEN_PARENS:
                            case DaxToken.CLOSE_PARENS:
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
