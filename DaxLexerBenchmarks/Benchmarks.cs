using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using Oracle = DaxLexerOracle;

namespace DaxLexerBenchmarks
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
    /// Lexes an expression into a list of all its tokens, as Tokenize and the syntax highlighting do.
    /// </summary>
    [Config(typeof(Config))]
    public class LexBenchmarks
    {
        [Params("Small", "Keystroke", "Large")]
        public string Size { get; set; }

        private string _dax;

        [GlobalSetup]
        public void Setup()
        {
            _dax = Size == "Small" ? DaxSamples.Small : Size == "Keystroke" ? DaxSamples.Keystroke : DaxSamples.Large;
        }

        [Benchmark(Baseline = true)]
        public int Antlr()
        {
            var lexer = new Oracle.DAXLexer(new Oracle.DAXCharStream(_dax, false));
            lexer.RemoveErrorListeners();
            return lexer.GetAllTokens().Count;
        }

        // The smallest change for the consumers: IToken objects from an ITokenSource, as ANTLR gives them
        [Benchmark]
        public int ParlotITokenAdapter()
        {
            var source = new ParlotTokenSource(_dax);
            var tokens = new List<IToken>();
            for (var token = source.NextToken(); token.Type != TokenConstants.Eof; token = source.NextToken()) tokens.Add(token);
            return tokens.Count;
        }

        [Benchmark]
        public int ParlotStruct() => DaxTokenizer.GetAllTokens(_dax).Count;

        // What FormulaFixup does with the text of the tokens it looks at, and what the keystroke path reads
        [Benchmark]
        public int ParlotNoList()
        {
            var count = 0;
            var tokenizer = new DaxTokenizer(_dax);
            while (tokenizer.Read(out _)) count++;
            return count;
        }
    }

    /// <summary>
    /// The lexing part of a keystroke in the expression editor: the tokens, then their channels and types for the styles.
    /// </summary>
    [Config(typeof(Config))]
    public class KeystrokeBenchmarks
    {
        private readonly string _dax = DaxSamples.Keystroke;

        [Benchmark(Baseline = true)]
        public int Antlr()
        {
            var lexer = new Oracle.DAXLexer(new Oracle.DAXCharStream(_dax, false));
            lexer.RemoveErrorListeners();
            var tokens = lexer.GetAllTokens().ToList();
            var keywords = 0;
            foreach (var token in tokens)
            {
                if (token.Channel == Oracle.DAXLexer.KEYWORD_CHANNEL) keywords++;
            }
            return keywords;
        }

        [Benchmark]
        public int Parlot()
        {
            var tokens = DaxTokenizer.GetAllTokens(_dax);
            var keywords = 0;
            foreach (var token in tokens)
            {
                if (token.Channel == DaxTokenizer.KeywordChannel) keywords++;
            }
            return keywords;
        }
    }

    /// <summary>
    /// What loading a model costs in FormulaFixup: the dependencies of each DAX property, on a real TOM model extended
    /// with 2,000 generated measures.
    /// </summary>
    [Config(typeof(Config))]
    public class FormulaFixupBenchmarks
    {
        [GlobalSetup]
        public void Setup() => DaxModel.Load(generateMeasures: true);

        [Benchmark(Baseline = true)]
        public void BuildDependencyTree_Antlr()
        {
            foreach (var obj in DaxModel.Objects) AntlrFormulaFixup.BuildDependencyTree(obj);
        }

        [Benchmark]
        public void BuildDependencyTree_Parlot()
        {
            foreach (var obj in DaxModel.Objects) FormulaFixup.BuildDependencyTree(obj, true);
        }

        [Benchmark]
        public int GetDependencies_Antlr()
        {
            var count = 0;
            foreach (var (obj, property, dax) in DaxModel.Expressions) count += AntlrFormulaFixup.GetDependencies(obj, dax, property).Count;
            return count;
        }

        [Benchmark]
        public int GetDependencies_Parlot()
        {
            var count = 0;
            foreach (var (obj, property, dax) in DaxModel.Expressions) count += FormulaFixup.GetDependencies(obj, dax, property).Count;
            return count;
        }
    }

    /// <summary>
    /// The Parlot lexer behind the interfaces of ANTLR.
    /// </summary>
    internal sealed class ParlotTokenSource : ITokenSource
    {
        private readonly DaxTokenizer _tokenizer;
        private readonly string _dax;

        public ParlotTokenSource(string dax)
        {
            _dax = dax;
            _tokenizer = new DaxTokenizer(dax);
        }

        public IToken NextToken()
        {
            return _tokenizer.Read(out var token) ? new AdapterToken(token) : new CommonToken(TokenConstants.Eof);
        }

        public int Line => throw new NotSupportedException();
        public int Column => throw new NotSupportedException();
        public ICharStream InputStream => throw new NotSupportedException();
        public string SourceName => "";
        public ITokenFactory TokenFactory { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        private sealed class AdapterToken : IToken
        {
            private readonly DaxLexerToken _token;

            public AdapterToken(in DaxLexerToken token) => _token = token;

            public string Text => _token.Text;
            public int Type => _token.Type;
            public int Line => _token.Line;
            public int Column => _token.Column;
            public int Channel => _token.Channel;
            public int TokenIndex => -1;
            public int StartIndex => _token.StartIndex;
            public int StopIndex => _token.StopIndex;
            public ITokenSource TokenSource => null;
            public ICharStream InputStream => null;
        }
    }
}
