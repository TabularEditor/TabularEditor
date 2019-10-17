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
            analyzer.SetModel(model);

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
        public void NullExpressionTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.SetModel(model);

            var rule = new BestPracticeRule()
            {
                Scope = RuleScope.Partition,
                Expression = "Expression.IndexOf(\"todo\", StringComparison.InvariantCultureIgnoreCase) >= 0"
            };

            model.Tables["Currency"].Partitions[0].Expression = null;

            var results = analyzer.Analyze(rule).ToList();
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void PerformanceTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.SetModel(model);

            var rule = new BestPracticeRule()
            {
                Scope = RuleScope.Measure | RuleScope.CalculatedColumn | RuleScope.DataColumn | RuleScope.Table | RuleScope.Hierarchy,
                Expression = "Description.Contains(\"todo\")"
            };

            model.Tables["Currency"].Description = null;
            model.Tables["Geography"].Description = "todo";

            var results = new List<AnalyzerResult>();

            // Warm the cache:
            results.AddRange(analyzer.Analyze(rule));

            results.Clear();

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                results.AddRange(analyzer.Analyze(rule));
            }
            sw.Stop(); // 2600
            Assert.IsTrue(sw.ElapsedMilliseconds < 5000);
            Assert.AreEqual(1000, results.Count);
        }

        [TestMethod]
        public void TokenizeTest()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.SetModel(model);

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
        
        [TestMethod]
        public void ComplexTest1()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.SetModel(model);

            var rule = new BestPracticeRule()
            {
                Scope = RuleScope.Measure,
                Expression = "DependsOn.Any(Key.ObjectType = \"Column\")"
            };

            var results = analyzer.Analyze(rule).ToList();
            Assert.IsTrue(results.Count > 0);
            Assert.IsFalse(results[0].RuleHasError);
            Assert.IsInstanceOfType(results[0].Object, typeof(Measure));
            Assert.IsTrue((results[0].Object as Measure).DependsOn.Any(d => d.Key.ObjectType == ObjectType.Column));
        }

        [TestMethod]
        public void ComplexTest2()
        {
            var handler = new TabularModelHandler("localhost", "AdventureWorks");
            var model = handler.Model;
            var analyzer = new Analyzer();
            analyzer.SetModel(model);

            var rule = new BestPracticeRule()
            {
                Scope = RuleScope.Measure,
                Expression = "DependsOn.Any(Value.Any(not FullyQualified))"
            };

            var results = analyzer.Analyze(rule).ToList();
            Assert.IsTrue(results.Count > 0);
            Assert.IsFalse(results[0].RuleHasError);
            Assert.IsInstanceOfType(results[0].Object, typeof(Measure));
            Assert.IsTrue((results[0].Object as Measure).DependsOn.Any(d => d.Value.Any(v => !v.fullyQualified)));
        }
    }
}
