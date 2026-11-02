using System;
using System.IO;
using System.Linq;
using TabularEditor.BestPracticeAnalyzer;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using Xunit;

namespace DaxLexerTests
{
    /// <summary>
    /// The public contract of the tokens, on a real model that doesn't need a server: the Windows-only tests of
    /// Best Practice Analyzer use a deployed model.
    /// </summary>
    public class ConsumerTests
    {
        private static Model LoadModel()
        {
            return new TabularModelHandler(Path.Combine(AppContext.BaseDirectory, "Corpus", "TE_AdventureWorks.bim")).Model;
        }

        [Fact]
        public void BestPracticeRulesUseTokenTypesByName()
        {
            var model = LoadModel();
            var analyzer = new Analyzer();
            analyzer.SetModel(model);

            var rule = new BestPracticeRule
            {
                Scope = RuleScope.Measure,
                Expression = "Tokenize().Any(Type = DIV)"
            };

            var results = analyzer.Analyze(rule).ToList();

            Assert.NotEmpty(results);
            Assert.All(results, r => Assert.Contains("/", ((Measure)r.Object).Expression));
            Assert.Equal(model.AllMeasures.Count(m => m.Tokenize().Any(t => t.Type == DaxToken.DIV)), results.Count);
        }

        [Fact]
        public void TokensDescribeThemselves()
        {
            var tokens = DaxDependencyHelper.Tokenize("SUM ( 'A''b'[c]]d] ) /* x */", includeHidden: true);

            Assert.Equal(new[]
            {
                "Token #0, 0-2: DaxToken.SUM: \"SUM\"",
                "Token #1, *** 3-3: DaxToken.WHITESPACES: \" \"",
                "Token #2, 4-4: DaxToken.OPEN_PARENS: \"(\"",
                "Token #3, *** 5-5: DaxToken.WHITESPACES: \" \"",
                "Token #4, 6-11: DaxToken.TABLE: \"A'b\"",
                "Token #5, 12-17: DaxToken.COLUMN_OR_MEASURE: \"c]d\"",
                "Token #6, *** 18-18: DaxToken.WHITESPACES: \" \"",
                "Token #7, 19-19: DaxToken.CLOSE_PARENS: \")\"",
                "Token #8, *** 20-20: DaxToken.WHITESPACES: \" \"",
                "Token #9, *** 21-27: DaxToken.DELIMITED_COMMENT: \"/* x */\"",
            }, tokens.Select(t => t.ToString()));

            Assert.Equal("(", tokens[0].Next.Text);
            Assert.Equal("SUM", tokens[2].Prev.Text);
            Assert.Null(tokens[9].Next);

            Assert.Equal(new[] { 0, 1, 2, 3, 4 }, DaxDependencyHelper.Tokenize("SUM ( 'A''b'[c]]d] ) /* x */", includeHidden: false).Select(t => t.TokenIndex));
        }
    }
}
