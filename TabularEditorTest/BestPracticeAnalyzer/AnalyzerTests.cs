using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.BestPracticeAnalyzer;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    [TestClass]
    public class AnalyzerTests
    {
        [TestMethod]
        public void BasicTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.Model = model;

            var rule = new BestPracticeRule() {
                Scope = RuleScope.Measure | RuleScope.CalculatedColumn | RuleScope.DataColumn | RuleScope.Table | RuleScope.Hierarchy,
                Expression = "Description.Contains(\"todo\")"
            };

            model.Tables["Currency"].Description = null;
            model.Tables["Geography"].Description = "todo";

            var results = analyzer.Analyze(rule).ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreSame(model.Tables["Geography"], results[0].Object);
        }

        [TestMethod]
        public void PerformanceTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.Model = model;

            var rule = new BestPracticeRule()
            {
                Scope = RuleScope.Measure | RuleScope.CalculatedColumn | RuleScope.DataColumn | RuleScope.Table | RuleScope.Hierarchy,
                Expression = "Description.Contains(\"todo\")"
            };

            model.Tables["Currency"].Description = null;
            model.Tables["Geography"].Description = "todo";

            var results = new List<AnalyzerResult>();

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                results.AddRange(analyzer.Analyze(rule));
            }
            sw.Stop(); // 2600
            Assert.AreEqual(1000, results.Count);
        }

        [TestMethod]
        public void TokenizeTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.Model = model;

            var rule = new BestPracticeRule()
            {
                Scope = RuleScope.Measure,
                Expression = "Tokenize().Any(Type = DIV)"
            };

            var results = analyzer.Analyze(rule).ToList();
            Assert.IsTrue(results.Count > 0);
            Assert.IsInstanceOfType(results[0].Object, typeof(Measure));
            Assert.IsTrue((results[0].Object as Measure).Expression.Contains("/"));
        }
    }
}
