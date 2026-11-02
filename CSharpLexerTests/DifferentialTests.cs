using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Antlr4.Runtime;
using TabularEditor.TextServices;
using Xunit;
using Oracle = CSharpLexerOracle;

namespace CSharpLexerTests
{
    /// <summary>
    /// Compares every token of the Parlot lexer with the one of the ANTLR lexer generated from the original grammar:
    /// type, channel, text, start and stop index, line and column, on real and on adversarial text. An exception must
    /// be of the same type and come after the same tokens.
    /// </summary>
    public class DifferentialTests
    {
        /// <summary>
        /// The input of the grammar, for a colon in an interpolated string with none of <c>}</c>, <c>:</c> or <c>)</c> after
        /// it. The action of the rule looks further and further past the end of the input, until the index overflows after
        /// about 2^31 reads and that throws. This throws right away: <see cref="ColonLooksPastTheEndUntilTheIndexOverflows"/>
        /// checks the real outcome.
        /// </summary>
        internal sealed class GuardedInputStream : AntlrInputStream
        {
            public GuardedInputStream(string input) : base(input) { }

            public override int La(int i)
            {
                if (i > 0 && Index + i - 1 > Size) throw new IndexOutOfRangeException();
                return base.La(i);
            }
        }

        internal static (List<IToken> Tokens, Exception Exception) RunAntlr(string script, ICharStream input = null)
        {
            var lexer = new Oracle.CSharpLexer(input ?? new GuardedInputStream(script));
            lexer.RemoveErrorListeners();
            var stream = new CommonTokenStream(lexer, Oracle.CSharpLexer.DefaultTokenChannel);
            Exception exception = null;
            try
            {
                stream.Fill();
            }
            catch (Exception e)
            {
                exception = e;
            }
            return (stream.GetTokens().ToList(), exception);
        }

        internal static (List<CSharpLexerToken> Tokens, Exception Exception) RunParlot(string script)
        {
            var tokens = new List<CSharpLexerToken>();
            Exception exception = null;
            try
            {
                CSharpTokenizer.GetAllTokens(script, tokens);
            }
            catch (Exception e)
            {
                exception = e;
            }
            return (tokens, exception);
        }

        internal static void AssertSame(string script)
        {
            var (expected, expectedException) = RunAntlr(script);
            var (actual, actualException) = RunParlot(script);

            for (var i = 0; i < Math.Min(expected.Count, actual.Count); i++)
            {
                var e = expected[i];
                var a = actual[i];
                if (e.Type != a.Type || e.Channel != a.Channel || e.StartIndex != a.StartIndex || e.StopIndex != a.StopIndex
                    || e.Line != a.Line || e.Column != a.Column || e.Text != a.Text)
                {
                    Assert.Fail($"Token {i} of {Show(script)}\n  antlr:  {Show(e.Type, e.Channel, e.StartIndex, e.StopIndex, e.Line, e.Column, e.Text)}\n  parlot: {Show(a.Type, a.Channel, a.StartIndex, a.StopIndex, a.Line, a.Column, a.Text)}");
                }
            }
            if (expected.Count != actual.Count)
            {
                Assert.Fail($"{Show(script)}: antlr has {expected.Count} tokens, parlot {actual.Count}\n  antlr:  {string.Join(" ", expected.Select(t => Oracle.CSharpLexer.DefaultVocabulary.GetSymbolicName(t.Type)))} {expectedException?.GetType().Name}\n  parlot: {string.Join(" ", actual.Select(t => Oracle.CSharpLexer.DefaultVocabulary.GetSymbolicName(t.Type)))} {actualException?.GetType().Name}");
            }
            if (expectedException?.GetType() != actualException?.GetType())
            {
                Assert.Fail($"{Show(script)}: antlr throws {expectedException?.GetType().Name}, parlot {actualException?.GetType().Name}");
            }
        }

        private static string Show(int type, int channel, int start, int stop, int line, int column, string text) =>
            $"{Oracle.CSharpLexer.DefaultVocabulary.GetSymbolicName(type)} ch{channel} {start}-{stop} {line}:{column} {Show(text)}";

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
            var fields = typeof(CSharpToken).GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.IsLiteral).ToDictionary(f => f.Name, f => (int)f.GetRawConstantValue());

            Assert.Equal(Oracle.CSharpLexer.COMMENTS_CHANNEL, CSharpTokenizer.CommentsChannel);
            Assert.Equal(Oracle.CSharpLexer.DIRECTIVE, CSharpTokenizer.DirectiveChannel);
            Assert.Equal(Lexer.Hidden, CSharpTokenizer.HiddenChannel);
            Assert.Equal(Lexer.DefaultTokenChannel, CSharpTokenizer.DefaultChannel);
            Assert.Equal(Lexer.Eof, fields["Eof"]);
            fields.Remove("Eof");

            var count = 0;
            foreach (var type in Enumerable.Range(1, Oracle.CSharpLexer.DefaultVocabulary.MaxTokenType))
            {
                Assert.Equal(type, fields[Oracle.CSharpLexer.DefaultVocabulary.GetSymbolicName(type)]);
                count++;
            }
            Assert.Equal(count, fields.Count);
        }

        [Fact]
        public void ColonLooksPastTheEndUntilTheIndexOverflows()
        {
            // The real input stream of the grammar, which takes seconds on this one
            var script = "$\"{a:";
            var (tokens, exception) = RunAntlr(script, new AntlrInputStream(script));
            Assert.IsType<IndexOutOfRangeException>(exception);
            Assert.Equal(2, tokens.Count);
            AssertSame(script);
        }

        [Fact]
        public void EveryLiteralOfTheGrammar()
        {
            var vocabulary = Oracle.CSharpLexer.DefaultVocabulary;
            var literals = Enumerable.Range(1, vocabulary.MaxTokenType).Select(t => vocabulary.GetLiteralName(t)).Where(l => l != null).Select(l => l.Trim('\''))
                .Concat(new[] { "error", "warning", "region", "endregion", "pragma", "true", "false", "if", "else", "default", "hidden", "define", "undef", "elif", "endif", "line" }).Distinct().ToList();
            Assert.True(literals.Count > 150, $"Only {literals.Count} literals");
            var contexts = new[] { "{0}", "@{0}", "{0}x", "x{0}", "{0}1", "{0}_", "{0} ", "{0}\\u0041", "{0}\\u00", "1.{0}", "1.@{0}", "$\"{{0}}", "$\"{{0}:x}", "$@\"{0}\"", "$\"{0}\"", "#{0}", "#{0} ", "#{0}\n", "#{0}x", "#{0}\\u0041x", "#{0} x\ny", "# {0}\t\tx\r\n{0}" };
            foreach (var literal in literals)
            {
                foreach (var context in contexts) AssertSame(context.Replace("{0}", literal));
                for (var length = 1; length < literal.Length; length++)
                {
                    AssertSame(literal.Substring(0, length));
                    AssertSame(literal.Substring(0, length) + "!");
                    AssertSame("#" + literal.Substring(0, length));
                    AssertSame("#" + literal.Substring(0, length) + "!");
                }
            }
        }

        [Fact]
        public void EveryCharInEveryContext()
        {
            var contexts = new[]
            {
                "{0}", "a{0}", "a{0}b", "1{0}", "1.{0}", "1.5{0}", "1e{0}", "0x{0}", ".{0}", "@{0}", "\\{0}", "\\u{0}", "a\\u00{0}", "\"{0}\"", "\"{0}", "\"\\{0}\"", "\"\\x{0}\"", "\"\\u{0}\"",
                "'{0}'", "'{0}", "'\\{0}'", "'\\x{0}'", "'\\x1{0}", "@\"{0}\"", "@\"{0}", "@\"\"{0}\"", "//{0}x\ny", "///{0}x", "/{0}", "/*{0}*/", "/*{0}", "/**{0}*/", "/**/{0}*/", " {0} ",
                "${0}", "$@{0}", "\u00EF{0}", "\u00EF\u00BB{0}", "$\"{0}\"", "$\"{0}", "$\"\\{0}\"", "$\"{{0}}", "$\"{a:{0}}", "$\"{a:}{0}\"", "$@\"{0}\"", "$@\"\\{0}\"", "$@\"{{0}}", "$@\"\"{0}",
                "#{0}", "# {0}\nx", "#a{0}", "#1{0}", "#\"{0}\"", "#\"{0}", "#//{0}\nx", "#/{0}", "#={0}", "#&{0}", "#|{0}", "#!{0}", "#\\{0}", "#error {0}\nx", "#error{0}", "#region{0}", "#region x{0}y\nz", "#pragma{0}x",
            };
            for (var c = 0; c <= char.MaxValue; c++)
            {
                var s = ((char)c).ToString();
                foreach (var context in contexts) AssertSame(context.Replace("{0}", s));
            }
        }

        [Fact]
        public void AllShortStringsOfTheStructuralChars()
        {
            // Comments
            AllStrings(new[] { "/", "*", "x", "\n" }, prefix: "", maxLength: 9);
            AllStrings(new[] { "/", "*", "x" }, prefix: "/*", maxLength: 10);
            AllStrings(new[] { "/", "*", "x" }, prefix: "/**", maxLength: 10);
            // Strings and chars
            AllStrings(new[] { "\"", "'", "\\", "x", "\n", "1", "@" }, prefix: "", maxLength: 7);
            AllStrings(new[] { "\\", "x", "u", "U", "1", "'" }, prefix: "'", maxLength: 7);
            AllStrings(new[] { "\\", "x", "u", "U", "1", "g", "\"" }, prefix: "\"", maxLength: 7);
            // Numbers
            AllStrings(new[] { "0", "1", ".", "e", "+", "x", "f", "u", "L", "@" }, prefix: "", maxLength: 6);
            AllStrings(new[] { "a", "\\u0041", "\\", "_", "1", "@", "." }, prefix: "", maxLength: 6);
            // Interpolated strings, in all their modes
            AllStrings(new[] { "$\"", "{", "}", ":", "\"", "x" }, prefix: "", maxLength: 8);
            AllStrings(new[] { "$\"", "$@\"", "{", "}", "\"", ")" }, prefix: "", maxLength: 8);
            AllStrings(new[] { "$@\"", "{", "}", ":", "\"", "\\" }, prefix: "", maxLength: 8);
            AllStrings(new[] { "{", "}", ":", ")", "\"", "\\n", "\n" }, prefix: "$\"", maxLength: 7);
            AllStrings(new[] { "{", "}", ":", "\"", "\"\"", "\\" }, prefix: "$@\"{\"\"", maxLength: 7);
            AllStrings(new[] { "}", "{", ":", ")", "\"", "$\"", "#", "\n" }, prefix: "$\"{", maxLength: 6);
            // Directives
            AllStrings(new[] { "#", "if", " ", "\n", "\r", "x", "1", "\"" }, prefix: "", maxLength: 6);
            AllStrings(new[] { "#", "error", " ", "\n", "x", "region", "//", "\r\n" }, prefix: "", maxLength: 6);
            AllStrings(new[] { "=", "!", "&", "|", "/", "(", ")", "\"", "\n" }, prefix: "#", maxLength: 5);
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
            "\"", "\"\"", "'", "''", "\\", "\\n", "\\\"", "\\x1", "\\u0041", "\\U00000041", "\\u00", "@\"", "$\"", "$@\"", "{", "}", "{{", "}}", ":", "::", ")", "(", "#", "\n", "\r\n", "\r", " ", "  ", "\t", "\v", "\f",
            "\u0085", "\u2028", "\u2029", "\u2007", "\u00A0", "\u180E", "\u200B", "\u3000", "//", "///", "/*", "/**", "*/", "/", "*", "0", "12", "3", "0x", "0X1f", ".", ",", ";", "1.", "1.5", "1e", "1e5", "1f", "1L", "1ul", "e", "E+", "@", "$",
            "abstract", "__arglist", "_", "x", "a1", "if", "else", "true", "false", "default", "hidden", "define", "undef", "elif", "endif", "line", "error", "warning", "region", "endregion", "pragma", "class", "string", "void",
            "+", "++", "+=", "-", "--", "-=", "->", "*=", "/=", "%", "%=", "&", "&&", "&=", "|", "||", "|=", "^", "^=", "!", "!=", "~", "=", "==", "=>", "<", "<=", "<<", "<<=", ">", ">=", "?", "??", "[", "]",
            "\u00E9", "\u00EF", "\u00BB", "\u00BF", "\u00EF\u00BB\u00BF", "\0", "\uFFFF", "\uFFFE", "\uD83D", "\uDE00", "`"
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

        // Pieces that go deep in interpolated strings
        private static readonly string[] InterpolationPieces = { "$\"", "$@\"", "{", "}", "{{", "}}", ":", "::", "\"", "\"\"", ")", "(", "x", " ", "\\n", "\\", "\n", "#", "'", "//", "/*", "*/", "@\"", "1", "if" };

        [Theory]
        [InlineData(4, 400000, 14)]
        [InlineData(5, 100000, 60)]
        public void RandomInterpolatedStrings(int seed, int iterations, int maxPieces)
        {
            var random = new Random(seed);
            var sb = new StringBuilder();
            for (var i = 0; i < iterations; i++)
            {
                sb.Clear();
                sb.Append(random.Next(2) == 0 ? "$\"" : "$@\"");
                var count = random.Next(1, maxPieces + 1);
                for (var j = 0; j < count; j++) sb.Append(InterpolationPieces[random.Next(InterpolationPieces.Length)]);
                AssertSame(sb.ToString());
            }
        }

        [Fact]
        public void RealSources()
        {
            var corpus = ScriptCorpus.Load();
            Assert.True(corpus.Count > 300, $"Only {corpus.Count} sources");
            foreach (var source in corpus)
            {
                AssertSame(source);
                AssertSame(source.Replace("\r", ""));
            }
        }

        [Fact]
        public void RealSourcesCutAnywhere()
        {
            // Every prefix ends in the middle of something: strings, comments and interpolations that aren't closed
            foreach (var source in ScriptCorpus.Load().Where(d => d.Length < 3000).OrderBy(d => d.Length).Take(100))
            {
                for (var length = 0; length < source.Length; length++) AssertSame(source.Substring(0, length));
            }
            foreach (var script in ScriptCorpus.HandWritten)
            {
                for (var length = 0; length <= script.Length; length++) AssertSame(script.Substring(0, length));
            }
        }
    }

    internal static class ScriptCorpus
    {
        private static List<string> _corpus;

        internal static readonly string[] HandWritten =
        {
            "",
            "foreach(var m in Model.AllMeasures.Where(m => m.Name.Contains(\"x\"))) { m.FormatString = \"0.00\"; m.Output(); }",
            "var t = Selected.Table; var c = t.Columns.First(); Info($\"{t.Name}: {c.Name,10:N0} {{literal}} {(a ? \"x\" : \"y\")}\");",
            "var s = $@\"multi \"\"quoted\"\" {x:yyyy-MM-dd}\nline {{ {y} }}\"; // comment\n/* block */ /** doc */ /// doc\n",
            "$\"{a:x}}y}\" + $\"{$\"{b}\"}\" + $\"{ {c} }\" + $\"{(d ? 1 : 2)}\"",
            "#region Some text here\r\nvar x = 0x1F + 1.5e3f + 1ul.ToString() + .5m; '\\n' + '\\u0041' + '\\x1' + \"\\x12345\";\n#endregion\n",
            "#if DEBUG && !(RELEASE || X == true) // comment\n#define Y\n#error some text\n#pragma warning disable 1591\n#line default\n#line 12 \"file.cs\"\n#else\n#endif\n",
            "class C<T> : I { int P { get; set; } = 1; public static void M() { x?.y ?? z; a <<= 1; b >= c; d => e; f->g; h::i; } }\nenum E { A, B }",
            "@class.@if + \\u0041bc + a\\u0041 + my_var + __arglist + _a + __b;",
        };

        public static List<string> Load()
        {
            if (_corpus != null) return _corpus;

            var result = new List<string>(HandWritten);
            var root = AppContext.BaseDirectory;
            while (!File.Exists(Path.Combine(root, "TabularEditor.sln"))) root = Path.GetDirectoryName(root);

            // The sources of the application, the scripts that come with it and the ones of the documentation
            foreach (var folder in new[] { "TabularEditor", "TOMWrapper", "TOMWrapperTest", "TabularEditorTest", "Scripts", "BPALib", "DaxLexerTests" })
            {
                var path = Path.Combine(root, folder);
                if (!Directory.Exists(path)) continue;
                foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    if (file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar) || file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)) continue;
                    var extension = Path.GetExtension(file);
                    if (extension == ".cs" || extension == ".csx") result.Add(File.ReadAllText(file));
                }
            }
            return _corpus = result;
        }
    }
}
