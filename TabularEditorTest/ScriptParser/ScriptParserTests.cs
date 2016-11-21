using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TabularEditor.UI;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TextServices.Tests
{
    /// <summary>
    /// Tests for AutoComplete features of the <see cref="ScriptParser"/> class.
    /// </summary>
    /// <remarks>
    /// The AutoComplete uses the <see cref="ScriptParser"/> class for determining the symbol type at a specific position within
    /// the written script. This is achieved through the <see cref="ScriptParser.GetTypeAtPos(int)"/> method, after the code has
    /// been lexed, using the ANTLR4 CSharp lexer (in project CSharpGrammar).
    /// 
    /// AutoComplete is intended to work with:
    /// 
    ///   - Type of all properties on known types
    ///   - Primitive types (also when assigned from literals)
    ///   - Return type of default indexed properties of known types
    ///   - Return type of all public methods on known types (exception: method overloads with different return types)
    ///   - Return type of all LINQ extension methods
    ///   - First argument of lambda expressions in LINQ methods having a single Func&lt;TSource,TResult&gt;-style argument.
    ///   - Types of variables that have been assigned without casts (inferred from the assigned value).
    ///   
    /// </remarks>
    [TestClass()]
    public class ScriptParserTests
    {
        [TestMethod()]
        public void SimpleVariableTest()
        {
            Assert.AreEqual(typeof(string), LastPos("var x = \"Test\"; x."));
            Assert.AreEqual(typeof(int), LastPos("var x = 12345; x."));
            Assert.AreEqual(typeof(char), LastPos("var x = 'c'; x."));
            Assert.AreEqual(typeof(Measure), LastPos(@"Measure x = Selected.Measure; x."));
            Assert.AreEqual(typeof(Measure), LastPos(@"var x = Selected.Measure; x."));
            Assert.AreEqual(typeof(Measure), LastPos(@"var x = Selected.Measure; var y = x; y."));
            Assert.AreEqual(typeof(Measure), LastPos(@"var x = Selected.Measure; var y = x; var z = y; z."));
            Assert.AreEqual(typeof(string), LastPos(@"var df = Model.Tables[].Measures[].DisplayFolder; df."));
        }

        [TestMethod()]
        public void AdvancedVariableTest()
        {
            Assert.AreEqual(typeof(Column), LastPos("var x = Selected.Measures.First().DisplayFolder; Selected.Columns.ForEach(c => c"));
            Assert.AreEqual(typeof(string), LastPos("var x = Selected.Measures.First().DisplayFolder; Selected.Columns.ForEach(c => c.DisplayFolder"));
            Assert.AreEqual(typeof(string), LastPos("var x = Selected.Measures.First().DisplayFolder; Selected.Columns.ForEach(c => c.DisplayFolder = x"));
        }

        [TestMethod()]
        public void SimpleReferenceTest()
        {
            Assert.AreEqual(typeof(UITreeSelection), LastPos("Selected."));
            Assert.AreEqual(typeof(UISelectionList<Measure>), LastPos("Selected.Measures."));
            Assert.AreEqual(typeof(Measure), LastPos("Selected.Measure."));
            Assert.AreEqual(typeof(string), LastPos("Selected.Measure.Name."));

            // Where() has been overridden on UISelectionList. Normally, it would be a LINQ extension method, that
            // would return IEnumerable, but not in this particular case:
            Assert.AreEqual(typeof(UISelectionList<Measure>), LastPos("Selected.Measures.Where()."));
        }

        [TestMethod()]
        public void IndexedPropertyTest()
        {
            Assert.AreEqual(typeof(Table), LastPos("Model.Tables[\"Test\"]."));
            Assert.AreEqual(typeof(Measure), LastPos("Model.Tables[\"Test\"].Measures[\"Test\"]."));
        }

        [TestMethod()]
        public void ComplexTest()
        {
            Assert.AreEqual(typeof(bool), LastPos(@"Selected.Measure.Model.Tables
                            .Where().First()
                            .Columns[""Test""].Model.Tables[""Test""]
                            .Hierarchies.Any()."));
        }

        [TestMethod()]
        public void LambdaTest()
        {
            Assert.AreEqual(typeof(string), LastPos("Selected.Measures.First(m => m.Name."));
            Assert.AreEqual(typeof(string), LastPos("Selected.Measures.First((m) => m.Name."));
            Assert.AreEqual(typeof(string), LastPos("Selected.Measures.First((m) => { return m.Name."));
            Assert.AreEqual(typeof(MeasureCollection), LastPos("Selected.Measures.First(m => m.Table.Measures."));
            Assert.AreEqual(typeof(TabularNamedObject), LastPos("Selected.ForEach(obj => obj."));
        }

        [TestMethod()]
        public void LinqReferenceTest()
        {
            Assert.AreEqual(typeof(Measure), LastPos("Selected.Measures.First()."));
            Assert.AreEqual(typeof(IEnumerable<Measure>), LastPos("Selected.Measures.Take(3)."));
            Assert.AreEqual(typeof(bool), LastPos("Selected.Measures.Any()."));
            Assert.AreEqual(typeof(int), LastPos("Selected.Measures.Count()."));
            Assert.AreEqual(typeof(IEnumerable<Table>), LastPos("Model.Tables.Where(t => t.Name == \"test\")."));
        }

        private Type LastPos(string code)
        {
            var parser = new ScriptParser();
            parser.Lex(code);
            parser.Types.Add("Selected", typeof(UITreeSelection));
            parser.Types.Add("Model", typeof(Model));

            return parser.GetTypeAtPos(code.Length);
        }
    }


}