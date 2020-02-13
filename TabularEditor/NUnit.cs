using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TabularEditor
{
    /// <summary>
    /// Represents a test run
    /// </summary>
    public class TestRun
    {
        private int id = 1;

        public Dictionary<string, TestSuite> TestSuites { get; } = new Dictionary<string, TestSuite>();

        public void StartSuite(string testSuite)
        {
            GetSuite(testSuite);
        }

        public void Fail(string testSuite, string testCase, string failureMessage, string stackTrace, IReadOnlyDictionary<string, string> properties)
        {
            GetSuite(testSuite).Fail(id++, testCase, failureMessage, stackTrace, properties);
        }
        public void Fail(string testSuite, string testCase, string failureMessage, string stackTrace)
        {
            Fail(testSuite, testCase, failureMessage, stackTrace, null);
        }
        public void Fail(string testSuite, string testCase, string failureMessage, IReadOnlyDictionary<string, string> properties)
        {
            Fail(testSuite, testCase, failureMessage, null, properties);
        }

        public void Fail(string testSuite, string testCase, string failureMessage)
        {
            Fail(testSuite, testCase, failureMessage, null, null);
        }

        public void Pass(string testSuite, string testCase, IReadOnlyDictionary<string, string> properties)
        {
            GetSuite(testSuite).Pass(id++, testCase, properties);
        }
        public void Pass(string testSuite, string testCase)
        {
            Pass(testSuite, testCase, null);
        }
        public void Skip(string testSuite, string testCase, IReadOnlyDictionary<string, string> properties)
        {
            GetSuite(testSuite).Skip(id++, testCase, properties);
        }
        public void Skip(string testSuite, string testCase)
        {
            Skip(testSuite, testCase, null);
        }
        public void Inconclude(string testSuite, string testCase, IReadOnlyDictionary<string, string> properties)
        {
            GetSuite(testSuite).Inconclusion(id++, testCase, properties);
        }
        public void Inconclude(string testSuite, string testCase)
        {
            Inconclude(testSuite, testCase, null);
        }

        private TestSuite GetSuite(string suiteName)
        {
            if (!TestSuites.TryGetValue(suiteName, out TestSuite suite))
            {
                suite = new TestSuite { Name = suiteName, Id = id++ };
                TestSuites.Add(suiteName, suite);
            }
            return suite;
        }

        public TestRun(string runName)
        {
            this.RunName = runName;
            this.StartTime = DateTime.Now;
        }
        public string RunName { get; private set; }

        public DateTime StartTime { get; private set; }

        public int Passed => TestSuites.Values.Sum(s => s.Passed);
        public int Failed => TestSuites.Values.Sum(s => s.Failed);
        public int Inconclusive => TestSuites.Values.Sum(s => s.Inconclusive);
        public int Skipped => TestSuites.Values.Sum(s => s.Skipped);
        public int Total => TestSuites.Values.Sum(s => s.Total);
        public int TestCaseCount => TestSuites.Values.Sum(s => s.TestCaseCount);

        public TestResult Result => Failed == 0 ? TestResult.Passed : TestResult.Failed;

        public void SerializeAsNUnit(string xmlFile)
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "no");
            var root = doc.DocumentElement;
            doc.InsertBefore(declaration, root);
            var run = doc.CreateElement(string.Empty, "test-run", string.Empty);
            run.SetAttribute("id", "2"); // Hardcoded for NUnit tests
            run.SetAttribute("name", RunName);
            run.SetAttribute("testcasecount", TestCaseCount.ToString());
            run.SetAttribute("result", Result.ToString());
            run.SetAttribute("time", ((double)((DateTime.Now - StartTime).TotalMilliseconds) / 1000.0).ToString("0.000"));
            run.SetAttribute("total", Total.ToString());
            run.SetAttribute("passed", Passed.ToString());
            run.SetAttribute("failed", Failed.ToString());
            run.SetAttribute("inconclusive", Inconclusive.ToString());
            run.SetAttribute("skipped", Skipped.ToString());
            run.SetAttribute("asserts", "0");
            run.SetAttribute("run-date", StartTime.ToString("yyyy-MM-dd"));
            run.SetAttribute("start-time", StartTime.ToString("hh:mm:ss"));

            doc.AppendChild(run);


            var env = doc.CreateElement(string.Empty, "environment", string.Empty);
            env.SetAttribute("nunit-version", "1.0.0.0");
            run.AppendChild(env);

            foreach(var testSuite in TestSuites.Values)
            {
                var suite = doc.CreateElement(string.Empty, "test-suite", string.Empty);
                suite.SetAttribute("type", "TestSuite");
                suite.SetAttribute("id", testSuite.Id.ToString());
                suite.SetAttribute("name", testSuite.Name);
                suite.SetAttribute("testcasecount", testSuite.TestCaseCount.ToString());
                suite.SetAttribute("total", testSuite.Total.ToString());
                suite.SetAttribute("result", testSuite.Result.ToString());
                suite.SetAttribute("time", ((double)((testSuite.EndTime - testSuite.StartTime).TotalMilliseconds) / 1000.0).ToString("0.000"));
                suite.SetAttribute("passed", testSuite.Passed.ToString());
                suite.SetAttribute("failed", testSuite.Failed.ToString());
                suite.SetAttribute("inconclusive", testSuite.Inconclusive.ToString());
                suite.SetAttribute("skipped", testSuite.Skipped.ToString());
                suite.SetAttribute("asserts", "0");

                run.AppendChild(suite);

                foreach(var testCase in testSuite.TestCases)
                {
                    var tc = doc.CreateElement(string.Empty, "test-case", string.Empty);
                    tc.SetAttribute("id", testCase.Id.ToString());
                    tc.SetAttribute("name", testCase.Name);
                    tc.SetAttribute("result", testCase.Result.ToString());
                    tc.SetAttribute("time", "0.000");
                    tc.SetAttribute("asserts", "0");

                    suite.AppendChild(tc);

                    if(testCase.Properties != null && testCase.Properties.Count > 0)
                    {
                        var props = doc.CreateElement(string.Empty, "properties", string.Empty);
                        tc.AppendChild(props);
                        foreach(var property in testCase.Properties)
                        {
                            var prop = doc.CreateElement(string.Empty, "property", string.Empty);
                            prop.SetAttribute("name", property.Key);
                            prop.SetAttribute("value", property.Value);
                            props.AppendChild(prop);
                        }
                    }

                    if(testCase.Failure != null)
                    {
                        var failure = doc.CreateElement(string.Empty, "failure", string.Empty);
                        tc.AppendChild(failure);
                        if (testCase.Failure.Message != null)
                        {
                            var msg = doc.CreateElement(string.Empty, "message", string.Empty);
                            var msgData = doc.CreateCDataSection(testCase.Failure.Message);
                            msg.AppendChild(msgData);
                            failure.AppendChild(msg);
                        }
                        if (testCase.Failure.StackTrace != null)
                        {
                            var stck = doc.CreateElement(string.Empty, "stack-trace", string.Empty);
                            var stckData = doc.CreateCDataSection(testCase.Failure.StackTrace);
                            stck.AppendChild(stckData);
                            failure.AppendChild(stck);
                        }
                    }
                }
            }

            doc.Save(xmlFile);
        }
        public void SerializeAsVSTest(string xmlFile)
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            var root = doc.DocumentElement;
            doc.InsertBefore(declaration, root);
            var run = doc.CreateElement(string.Empty, "TestRun", string.Empty);
            run.SetAttribute("id", Guid.NewGuid().ToString("D"));
            run.SetAttribute("name", RunName);
            run.SetAttribute("runUser", Environment.UserDomainName);
            run.SetAttribute("xmlns", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            doc.AppendChild(run);

            var times = doc.CreateElement(string.Empty, "Times", string.Empty);
            times.SetAttribute("creation", Process.GetCurrentProcess().StartTime.ToString("o"));
            times.SetAttribute("queuing", Process.GetCurrentProcess().StartTime.ToString("o"));
            times.SetAttribute("start", this.StartTime.ToString("o"));
            times.SetAttribute("finish", DateTime.Now.ToString("o"));
            run.AppendChild(times);

            var summary = doc.CreateElement(string.Empty, "ResultSummary", string.Empty);
            summary.SetAttribute("outcome", Failed > 0 ? "Failed" : Inconclusive > 0 ? "Inconclusive" : "Passed");
            run.AppendChild(summary);

            var counters = doc.CreateElement(string.Empty, "Counters", string.Empty);
            counters.SetAttribute("total", this.TestCaseCount.ToString());
            counters.SetAttribute("executed", this.Total.ToString());
            counters.SetAttribute("passed", this.Passed.ToString());
            counters.SetAttribute("failed", this.Failed.ToString());
            counters.SetAttribute("inconclusive", this.Inconclusive.ToString());
            counters.SetAttribute("notExecuted", this.Skipped.ToString());
            summary.AppendChild(counters);
            
            var defs = doc.CreateElement(string.Empty, "TestDefinitions", string.Empty);
            run.AppendChild(defs);

            var lists = doc.CreateElement(string.Empty, "TestLists", string.Empty);
            run.AppendChild(lists);

            var entries = doc.CreateElement(string.Empty, "TestEntries", string.Empty);
            run.AppendChild(entries);

            var results = doc.CreateElement(string.Empty, "Results", string.Empty);
            run.AppendChild(results);

            foreach (var s in TestSuites.Values)
            {
                var listId = s.Id.ToString();
                var list = doc.CreateElement(string.Empty, "TestList", string.Empty);
                list.SetAttribute("name", s.Name);
                list.SetAttribute("id", listId);
                lists.AppendChild(list);

                foreach (var t in s.TestCases)
                {
                    var testId = t.Id.ToString();
                    var executionId = Guid.NewGuid().ToString("D");

                    var def = doc.CreateElement(string.Empty, "UnitTest", string.Empty);
                    def.SetAttribute("name", t.Name);
                    def.SetAttribute("id", testId);
                    if (t.Properties != null && t.Properties.Count > 0)
                    {
                        var props = doc.CreateElement(string.Empty, "Properties", string.Empty);
                        def.AppendChild(props);
                        foreach (var property in t.Properties)
                        {
                            var prop = doc.CreateElement(string.Empty, "Property", string.Empty);
                            props.AppendChild(prop);
                            var key = doc.CreateElement(string.Empty, "Key", string.Empty);
                            key.InnerText = property.Key;
                            prop.AppendChild(key);
                            var value = doc.CreateElement(string.Empty, "Value", string.Empty);
                            value.InnerText = property.Value;
                            prop.AppendChild(value);
                        }
                    }
                    defs.AppendChild(def);

                    var entry = doc.CreateElement(string.Empty, "TestEntry", string.Empty);
                    entry.SetAttribute("testId", testId);
                    entry.SetAttribute("executionId", executionId);
                    entry.SetAttribute("testListId", listId);
                    entries.AppendChild(entry);

                    var result = doc.CreateElement(string.Empty, "UnitTestResult", string.Empty);
                    result.SetAttribute("executionId", executionId);
                    result.SetAttribute("testId", testId);
                    result.SetAttribute("testName", t.Name);
                    result.SetAttribute("testType", "unitTest");
                    result.SetAttribute("testListId", listId);
                    result.SetAttribute("outcome", t.Result.ToString());
                    results.AppendChild(result);

                    var output = doc.CreateElement(string.Empty, "Output", string.Empty);
                    result.AppendChild(output);

                    if (t.Failure != null)
                    {
                        var errInfo = doc.CreateElement(string.Empty, "ErrorInfo", string.Empty);
                        output.AppendChild(errInfo);

                        if (t.Failure.Message != null)
                        {
                            var msg = doc.CreateElement(string.Empty, "Message", string.Empty);
                            msg.InnerText = t.Failure.Message;
                            errInfo.AppendChild(msg);
                        }
                        if (t.Failure.StackTrace != null)
                        {
                            var st = doc.CreateElement(string.Empty, "StackTrace", string.Empty);
                            st.InnerText = t.Failure.StackTrace;
                            errInfo.AppendChild(st);
                        }
                    }
                }
            }

            doc.Save(xmlFile);
        }
    }

    public class TestSuite
    {
        public DateTime StartTime { get; } = DateTime.Now;
        public DateTime EndTime { get; private set; } = DateTime.Now;
        public int Id { get; set; }
        public int Passed => TestCases.Count(c => c.Result == TestResult.Passed);
        public int Failed => TestCases.Count(c => c.Result == TestResult.Failed);
        public int Inconclusive => TestCases.Count(c => c.Result == TestResult.Inconclusive);
        public int Skipped => TestCases.Count(c => c.Result == TestResult.Skipped);
        public int TestCaseCount => TestCases.Count;
        public int Total => TestCases.Count(c => c.Result != TestResult.Skipped);
        public TestResult Result => Failed == 0 ? TestResult.Passed : TestResult.Failed;

        public string Name { get; set; }
        public List<TestCase> TestCases { get; } = new List<TestCase>();

        public void Pass(int id, string caseName, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new TestCase { Id = id.ToString(), Name = caseName, Result = TestResult.Passed };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }

        public void Skip(int id, string caseName, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new TestCase { Id = id.ToString(), Name = caseName, Result = TestResult.Skipped };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }

        public void Inconclusion(int id, string caseName, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new TestCase { Id = id.ToString(), Name = caseName, Result = TestResult.Inconclusive };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }

        public void Fail(int id, string caseName, string message, string stackTrace, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new TestCase { Id = id.ToString(), Name = caseName, Result = TestResult.Failed };
            testCase.Failure = new TestFailure { Message = message, StackTrace = stackTrace };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }
    }

    public class TestCase
    {
        public TestResult Result { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        public TestFailure Failure { get; set; }
    }

    public enum TestResult
    {
        Passed,
        Failed,
        Inconclusive,
        Skipped
    }

    public class TestFailure
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
