using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using TabularEditor.TextServices;
using Oracle = CSharpLexerOracle;

namespace CSharpLexerBenchmarks
{
    internal class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default.WithWarmupCount(3).WithIterationCount(8));
            AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);
            AddColumn(BaselineRatioColumn.RatioMean);
            SummaryStyle = BenchmarkDotNet.Reports.SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        }
    }

    /// <summary>
    /// Lexes a script into a list of all its tokens, as ScriptParser.Lex does.
    /// </summary>
    [Config(typeof(Config))]
    public class LexBenchmarks
    {
        [Params("Small", "Medium", "Large")]
        public string Size { get; set; }

        private string Script => Size == "Small" ? ScriptSamples.Small : Size == "Medium" ? ScriptSamples.Medium : ScriptSamples.Large;

        private string _script;

        [GlobalSetup]
        public void Setup() => _script = Script;

        [Benchmark(Baseline = true)]
        public int Antlr()
        {
            var lexer = new Oracle.CSharpLexer(new AntlrInputStream(_script));
            lexer.RemoveErrorListeners();
            var stream = new CommonTokenStream(lexer, Lexer.DefaultTokenChannel);
            stream.Fill();
            return stream.Size;
        }

        [Benchmark]
        public int ParlotList()
        {
            var tokens = new List<CSharpLexerToken>();
            CSharpTokenizer.GetAllTokens(_script, tokens);
            return tokens.Count;
        }

        [Benchmark]
        public int ParlotNoList()
        {
            var tokenizer = new CSharpTokenizer(_script);
            var count = 1;
            while (tokenizer.Read().Type != CSharpToken.Eof) count++;
            return count;
        }

        internal int Count()
        {
            var tokens = new List<CSharpLexerToken>();
            CSharpTokenizer.GetAllTokens(Script, tokens);
            return tokens.Count;
        }
    }

    internal static class ScriptSamples
    {
        // A script of the kind Tabular Editor runs, with what the modes of the lexer are for
        public const string Small = @"// Formats the selected measures
foreach(var m in Selected.Measures.Where(m => m.Name.Contains(""Amount"")))
{
    m.FormatString = ""#,0.00"";
    m.Description = $""Sum of {m.Table.Name} at {m.DaxObjectFullName,10}: {1.5e3:N0}"";
    m.Output();
}
";

        public static readonly string Medium = Repeat(40);

        public static readonly string Large = Repeat(400);

        private static string Repeat(int count)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#region Helpers");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("#endregion");
            for (var i = 0; i < count; i++)
            {
                sb.Append(Small.Replace("Selected.Measures", "Model.Tables[\"T" + i + "\"].Measures"));
                sb.AppendLine("/* Another step of the script */ var x" + i + " = @\"verbatim \"\"text\"\"\"; var y" + i + " = 0x1F + " + i + ".5f;");
                sb.AppendLine("if (x" + i + ".Length > y" + i + ") { Info($@\"too long: {x" + i + "}\"); } else { Error(string.Format(\"{0}\", '\\n')); }");
            }
            return sb.ToString();
        }
    }
}
