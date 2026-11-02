using System;
using System.Collections.Generic;
using System.Linq;
using TabularEditor.TextServices;
using Xunit;

namespace CSharpLexerTests
{
    /// <summary>
    /// Compares ScriptParser with its copy that uses the ANTLR lexer: the tokens it gives to ScriptEngine, the method calls
    /// and property references found in them, and the type AutoComplete gets at each position of a script.
    /// </summary>
    public class ConsumerTests
    {
        public class SampleColumn
        {
            public string Name { get; set; }
            public SampleTable Table { get; set; }
        }

        public class SampleTable
        {
            public string Name { get; set; }
            public List<SampleColumn> Columns { get; set; }
            public SampleColumn this[string name] => null;
        }

        public class SampleModel
        {
            public List<SampleTable> Tables { get; set; }
            public SampleTable Find(string name) => null;
        }

        private static readonly Dictionary<string, Type> Types = new Dictionary<string, Type> { { "Model", typeof(SampleModel) }, { "Selected", typeof(SampleTable) } };
        private static readonly Dictionary<string, Type> Classes = new Dictionary<string, Type> { { "SampleTable", typeof(SampleTable) }, { "SampleColumn", typeof(SampleColumn) } };

        private static readonly string[] Scripts =
        {
            "var t = Model.Tables.First(); t.Columns.Where(c => c.Name == \"x\").Select(c => c.Table).First().Name; Selected.Columns[0].Name.Length; (a as SampleTable).Name; var s = t as SampleTable; s.Columns; \"x\".Length; 1.ToString(); 'c'.ToString();",
            "Output(); x.Output(1); Output(1, 2); Info(\"a\"); Warning($\"{a}\"); Error(f(1, g(2))); Output(\n1\n);\n//Output();\nSelected.Name; Model; a.Model;",
            "class A { void M() { Output(); } } var x = 1; struct B { } enum C { D } interface I { } x;",
        };

        private static void AssertSameTokens(IList<Antlr4.Runtime.IToken> expected, IList<CSharpLexerToken> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                AssertSameToken(expected[i], actual[i]);
                Assert.Equal(i, actual[i].TokenIndex);
            }
        }

        private static void AssertSameToken(Antlr4.Runtime.IToken expected, CSharpLexerToken actual)
        {
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.Channel, actual.Channel);
            Assert.Equal(expected.StartIndex, actual.StartIndex);
            Assert.Equal(expected.StopIndex, actual.StopIndex);
            Assert.Equal(expected.Line, actual.Line);
            Assert.Equal(expected.Column, actual.Column);
            Assert.Equal(expected.TokenIndex, actual.TokenIndex);
            Assert.Equal(expected.Text, actual.Text);
        }

        private static void AssertSameCalls(List<AntlrMethodCall> expected, List<MethodCall> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].ParamCount, actual[i].ParamCount);
                AssertSameToken(expected[i].StartToken, actual[i].StartToken);
                AssertSameToken(expected[i].StopToken, actual[i].StopToken);
            }
        }

        private static void AssertSame(string script, bool everyPosition)
        {
            var expected = new AntlrScriptParser { Types = Types, Classes = Classes };
            var actual = new ScriptParser { Types = Types, Classes = Classes };

            Exception expectedException = null, actualException = null;
            try { expected.Lex(script); } catch (Exception e) { expectedException = e; }
            try { actual.Lex(script); } catch (Exception e) { actualException = e; }
            Assert.Equal(expectedException?.GetType(), actualException?.GetType());

            var expectedTokens = expected.GetTokens();
            var actualTokens = actual.GetTokens();
            AssertSameTokens(expectedTokens, actualTokens);

            AssertSameCalls(expectedTokens.FindMethodCall(), actualTokens.FindMethodCall());
            foreach (var (name, parameters) in new[] { ("Output", 0), ("Output", 1), ("Info", 1), ("Warning", 1), ("Error", 1) })
            {
                AssertSameCalls(expectedTokens.FindMethodCall(name, parameters), actualTokens.FindMethodCall(name, parameters));
            }
            var names = new HashSet<string> { "Model", "Selected" };
            AssertSameCalls(expectedTokens.FindGlobalPropertyRefs(names), actualTokens.FindGlobalPropertyRefs(names));

            if (!everyPosition) return;
            for (var position = 0; position <= script.Length + 1; position++)
            {
                Assert.Equal(expected.GetTypeAtPos(position), actual.GetTypeAtPos(position));
            }
        }

        [Fact]
        public void TheTypeAtEveryPositionOfAScript()
        {
            foreach (var script in Scripts)
            {
                AssertSame(script, everyPosition: true);
                for (var length = 0; length < script.Length; length += 7) AssertSame(script.Substring(0, length), everyPosition: true);
            }
            foreach (var script in ScriptCorpus.HandWritten) AssertSame(script, everyPosition: true);
        }

        [Fact]
        public void TheTokensOfRealSources()
        {
            foreach (var source in ScriptCorpus.Load()) AssertSame(source, everyPosition: false);
            foreach (var source in ScriptCorpus.Load().Where(s => s.Length < 2000).Take(50)) AssertSame(source, everyPosition: true);
        }

        [Fact]
        public void AnExceptionKeepsTheTokensReadSoFar()
        {
            // Only reached through this colon, whose real lookahead takes seconds: see DifferentialTests
            var script = "var a = 1; x($\"{a:";
            var actual = new ScriptParser();
            Assert.Throws<IndexOutOfRangeException>(() => actual.Lex(script));
            Assert.Equal("a", actual.GetTokens()[1].Text);
            Assert.DoesNotContain(actual.GetTokens(), t => t.Type == CSharpToken.COLON);
            Assert.Null(actual.GetTypeAtPos(script.Length));
        }
    }
}
