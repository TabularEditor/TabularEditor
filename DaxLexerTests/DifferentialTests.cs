using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Antlr4.Runtime;
using Newtonsoft.Json.Linq;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper.Utils;
using Xunit;
using Oracle = DaxLexerOracle;

namespace DaxLexerTests
{
    /// <summary>
    /// Compares every token of the Parlot lexer with the one of the ANTLR lexer generated from the original grammar:
    /// type, channel, text, start and stop index, line and column, on real and on adversarial text.
    /// </summary>
    public class DifferentialTests
    {
        private static void AssertSame(string dax, bool nonUs)
        {
            var lexer = new Oracle.DAXLexer(new Oracle.DAXCharStream(dax, nonUs));
            lexer.RemoveErrorListeners();
            var expected = lexer.GetAllTokens();
            var actual = DaxTokenizer.GetAllTokens(dax, nonUs);

            for (var i = 0; i < Math.Min(expected.Count, actual.Count); i++)
            {
                var e = expected[i];
                var a = actual[i];
                if (e.Type != a.Type || e.Channel != a.Channel || e.StartIndex != a.StartIndex || e.StopIndex != a.StopIndex
                    || e.Line != a.Line || e.Column != a.Column || e.Text != a.Text)
                {
                    Assert.Fail($"Token {i} of {Show(dax)} nonUs={nonUs}\n  antlr:  {Show(e.Type, e.Channel, e.StartIndex, e.StopIndex, e.Line, e.Column, e.Text)}\n  parlot: {Show(a.Type, a.Channel, a.StartIndex, a.StopIndex, a.Line, a.Column, a.Text)}");
                }
            }
            if (expected.Count != actual.Count)
            {
                Assert.Fail($"{Show(dax)} nonUs={nonUs}: antlr has {expected.Count} tokens, parlot {actual.Count}");
            }
        }

        private static void AssertSame(string dax)
        {
            AssertSame(dax, false);
            AssertSame(dax, true);
        }

        private static string Show(int type, int channel, int start, int stop, int line, int column, string text) =>
            $"{DaxTokenizer.GetSymbolicName(type)} ch{channel} {start}-{stop} {line}:{column} {Show(text)}";

        private static string Show(string s)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in s)
            {
                if (c < 32 || c > 126) sb.Append($"\\u{(int)c:X4}");
                else sb.Append(c);
            }
            return sb.Append('"').ToString();
        }

        [Fact]
        public void TokenTypesMatchTheGrammar()
        {
            var fields = typeof(DaxToken).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.IsLiteral).ToDictionary(f => f.Name, f => (int)f.GetRawConstantValue());

            Assert.Equal(Oracle.DAXLexer.COMMENTS_CHANNEL, DaxTokenizer.CommentsChannel);
            Assert.Equal(Oracle.DAXLexer.KEYWORD_CHANNEL, DaxTokenizer.KeywordChannel);
            Assert.Equal(Lexer.Hidden, DaxTokenizer.HiddenChannel);
            Assert.Equal(Lexer.DefaultTokenChannel, DaxTokenizer.DefaultChannel);

            var count = 0;
            foreach (var type in Enumerable.Range(1, Oracle.DAXLexer.DefaultVocabulary.MaxTokenType))
            {
                var name = Oracle.DAXLexer.DefaultVocabulary.GetSymbolicName(type);
                Assert.Equal(type, fields[name]);
                Assert.Equal(name, DaxTokenizer.GetSymbolicName(type));
                count++;
            }
            Assert.Equal(count, fields.Count);
        }

        [Fact]
        public void EveryKeywordInAnyCase()
        {
            for (var type = 1; type <= Oracle.DAXLexer.DefaultVocabulary.MaxTokenType; type++)
            {
                var literal = Oracle.DAXLexer.DefaultVocabulary.GetLiteralName(type);
                if (literal == null) continue;
                var text = literal.Trim('\'');
                var mixed = new string(text.Select((c, i) => i % 2 == 0 ? char.ToLowerInvariant(c) : c).ToArray());
                foreach (var keyword in new[] { text, text.ToLowerInvariant(), mixed })
                {
                    AssertSame(keyword);
                    AssertSame(keyword + "(1)");
                    AssertSame(keyword + "x");
                    AssertSame(keyword + "_");
                    AssertSame("x" + keyword);
                    AssertSame(keyword + ".");
                    AssertSame(keyword + ".x");
                    AssertSame(keyword + ".5");
                    AssertSame(keyword + ",5");
                    AssertSame(keyword.Replace('.', ','));
                    AssertSame(keyword.Replace('.', ',') + "x");
                    for (var length = 1; length < keyword.Length; length++)
                    {
                        AssertSame(keyword.Substring(0, length));
                        AssertSame(keyword.Substring(0, length) + "!");
                    }
                }
            }
        }

        [Fact]
        public void EveryCharInEveryContext()
        {
            var contexts = new[] { "{0}", "a{0}", "a{0}b", "1{0}", "1.{0}", ".{0}", "|{0}", "\"{0}\"", "\"{0}", "'{0}'", "'{0}", "[{0}]", "[{0}", "//{0}", "--{0}x", "/*{0}*/", "/*{0}", "/{0}", "/*/{0}", "/*/*{0}*/*/", "DT\"{0}\"", "DT{0}", " {0} " };
            for (var c = 0; c <= char.MaxValue; c++)
            {
                var s = ((char)c).ToString();
                foreach (var context in contexts) AssertSame(context.Replace("{0}", s));
            }
        }

        [Fact]
        public void AllShortStringsOfTheStructuralChars()
        {
            AllStrings(new[] { "/", "*", "x", "\n", "\uFFFF" }, prefix: "/*", maxLength: 8);
            AllStrings(new[] { "/", "*", "x" }, prefix: "/*/*", maxLength: 11);
            AllStrings(new[] { "/", "*", "-" }, prefix: "", maxLength: 9);
            AllStrings(new[] { "\"", "'", "x", "\n", "\uFFFF" }, prefix: "", maxLength: 7);
            AllStrings(new[] { "[", "]", "\"", "x", "\n" }, prefix: "", maxLength: 7);
            AllStrings(new[] { "'", "\"", "x", "\r" }, prefix: "DT\"", maxLength: 7);
            AllStrings(new[] { "1", ".", ",", ";", "x" }, prefix: "", maxLength: 7);
            AllStrings(new[] { "|", "&", "<", ">", "=", "-" }, prefix: "", maxLength: 6);
            AllStrings(new[] { "T", ".", ",", "DIST", "2T", "RT", "x" }, prefix: "", maxLength: 6);
            AllStrings(new[] { "D", "T", "d", "\"", "x" }, prefix: "", maxLength: 6);
        }

        private static void AllStrings(string[] alphabet, string prefix, int maxLength)
        {
            var indexes = new int[maxLength];
            for (var length = 0; length <= maxLength; length++)
            {
                Array.Clear(indexes, 0, indexes.Length);
                while (true)
                {
                    var sb = new StringBuilder(prefix);
                    for (var i = 0; i < length; i++) sb.Append(alphabet[indexes[i]]);
                    AssertSame(sb.ToString());

                    var position = length - 1;
                    while (position >= 0 && ++indexes[position] == alphabet.Length) indexes[position--] = 0;
                    if (position < 0) break;
                }
            }
        }

        private static readonly string[] Pieces =
        {
            "\"", "\"\"", "'", "''", "[", "]", "]]", "//", "--", "/*", "*/", "/", "*", "-", "\n", "\r\n", "\r", " ", "  ", "\t",
            "\v", "\f", "\u0085", "\u2028", "\u2029", "\u2007", "\u00A0", "\u180E", "\u3000", "0", "12", "3", ".", ",", ";", ":",
            "|", "||", "&", "&&", "<", ">", "=", "<>", "<=", ">=", "=>", "^", "+", "(", ")", "{", "}", "DT\"", "dt\"", "DT", "dt",
            "d", "t", "STDEV", "STDEV.P", "stdev,s", "stdev.", "T.DIST", "T.DIST.2T", "t.dist.rt", "t.", "VAR", "var.", "NORM.S.",
            "NORM.S.DIST", "IF.EAGER", "IF", "_", "A1", "x", "SUM", "CALCULATE", "Sales", "\u00E9", "\u0131", "\u017F", "\u212A",
            "\u00DF", "\0", "\uFFFF", "\uFFFE", "\u1F600", "#", "@", "?", "$", "!", "%", "\\", "~", "`"
        };

        [Theory]
        [InlineData(1, 400000, 12)]
        [InlineData(2, 100000, 60)]
        [InlineData(3, 2000, 2000)]
        public void RandomStrings(int seed, int iterations, int maxPieces)
        {
            var random = new Random(seed);
            var sb = new StringBuilder();
            for (var i = 0; i < iterations; i++)
            {
                sb.Clear();
                var count = random.Next(1, maxPieces + 1);
                for (var j = 0; j < count; j++) sb.Append(Pieces[random.Next(Pieces.Length)]);
                AssertSame(sb.ToString());
            }
        }

        [Fact]
        public void RealExpressions()
        {
            var corpus = DaxCorpus.Load();
            Assert.True(corpus.Count > 300, $"Only {corpus.Count} expressions");
            foreach (var dax in corpus)
            {
                AssertSame(dax);
                AssertSame(dax.Replace("\r", ""));
            }
        }

        [Fact]
        public void RealExpressionsCutAnywhere()
        {
            // Every prefix ends in the middle of something: strings, names, comments and keywords that aren't closed
            foreach (var dax in DaxCorpus.Load().Where(d => d.Length < 400).Distinct().Take(300))
            {
                for (var length = 0; length < dax.Length; length++) AssertSame(dax.Substring(0, length));
            }
        }

        [Fact]
        public void HandWrittenExpressions()
        {
            foreach (var dax in HandWritten) AssertSame(dax);
        }

        internal static readonly string[] HandWritten =
        {
            "",
            "SUM('Internet Sales'[Sales Amount])",
            "VAR x = 1 RETURN x",
            "DEFINE MEASURE 'T'[M] = 1 EVALUATE ROW(\"a\", [M])",
            "'It''s'[a]]b]",
            "\"a \"\"quoted\"\" string\"",
            "dt\"2020-01-01\"",
            "dT\"a\"\"b\"",
            "1.5 + .5 + 2",
            "CALCULATE ( [Sales], FILTER ( ALL ( T[A] ), T[A] > 1,5 ) ); 1;2",
            "STDEV.P(T[A]) + stdevx.s(T, [A]) + T.DIST.2T(1;2) + T.INV(1,5;2)",
            "a // comment\r\nb -- another\nc /* multi\nline */ d /* nested /* comment */ e */ f",
            "/* never closed",
            "/* /* never closed */",
            "\"never closed",
            "'never closed\nb",
            "[never closed\nb",
            "1.x",
            "|x",
            "a;b",
            "a!b",
            "\u0131F \u017FUM",
        };
    }

    internal static class DaxCorpus
    {
        private static readonly HashSet<string> Properties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "expression", "filterExpression", "formatStringExpression", "statusExpression", "targetExpression", "trendExpression",
            "detailRowsExpression", "defaultDetailRowsExpression", "query"
        };

        private static List<string> _corpus;

        public static List<string> Load()
        {
            if (_corpus != null) return _corpus;

            var result = new List<string>();
            foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Corpus"), "*.bim"))
            {
                Collect(JToken.Parse(File.ReadAllText(file)), result);
            }
            return _corpus = result;
        }

        private static void Collect(JToken token, List<string> result)
        {
            if (token is JProperty p && Properties.Contains(p.Name))
            {
                if (p.Value is JValue v && v.Type == JTokenType.String) result.Add((string)v);
                else if (p.Value is JArray a) result.Add(string.Join("\n", a.Select(x => (string)x)));
            }
            foreach (var child in token.Children()) Collect(child, result);
        }
    }
}
