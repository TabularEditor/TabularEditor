using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TabularEditor
{
    /// <summary>
    /// Represents an NUnit test run
    /// </summary>
    public class NUnit
    {
        private int id = 1;

        public Dictionary<string, NUnitSuite> TestSuites { get; } = new Dictionary<string, NUnitSuite>();

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

        private NUnitSuite GetSuite(string suiteName)
        {
            if (!TestSuites.TryGetValue(suiteName, out NUnitSuite suite))
            {
                suite = new NUnitSuite { Name = suiteName, Id = id++ };
                TestSuites.Add(suiteName, suite);
            }
            return suite;
        }

        public DateTime StartTime { get; private set; } = DateTime.Now;

        public int Passed => TestSuites.Values.Sum(s => s.Passed);
        public int Failed => TestSuites.Values.Sum(s => s.Failed);
        public int Inconclusive => TestSuites.Values.Sum(s => s.Inconclusive);
        public int Skipped => TestSuites.Values.Sum(s => s.Skipped);
        public int Total => TestSuites.Values.Sum(s => s.Total);
        public int TestCaseCount => TestSuites.Values.Sum(s => s.TestCaseCount);

        public NUnitTestResult Result => Failed == 0 ? NUnitTestResult.Passed : NUnitTestResult.Failed;

        public void Serialize(string xmlFile)
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = doc.DocumentElement;
            doc.InsertBefore(declaration, root);
            var run = doc.CreateElement(string.Empty, "test-run", string.Empty);
            run.SetAttribute("id", "2"); // Hardcoded for NUnit tests
            run.SetAttribute("testcasecount", TestCaseCount.ToString());
            run.SetAttribute("result", Result.ToString());
            run.SetAttribute("total", Total.ToString());
            run.SetAttribute("passed", Passed.ToString());
            run.SetAttribute("failed", Failed.ToString());
            run.SetAttribute("skipped", Skipped.ToString());
            run.SetAttribute("inconclusive", Inconclusive.ToString());
            run.SetAttribute("run-date", StartTime.ToString("yyyy-MM-dd"));
            run.SetAttribute("start-time", StartTime.ToString("hh:mm:ss"));
            run.SetAttribute("end-time", DateTime.Now.ToString("hh:mm:ss"));
            run.SetAttribute("duration", ((double)((DateTime.Now - StartTime).TotalMilliseconds) / 1000.0).ToString("0.000"));

            doc.AppendChild(run);

            foreach(var testSuite in TestSuites.Values)
            {
                var suite = doc.CreateElement(string.Empty, "test-suite", string.Empty);
                suite.SetAttribute("type", "TestFixture");
                suite.SetAttribute("id", testSuite.Id.ToString());
                suite.SetAttribute("name", testSuite.Name);
                suite.SetAttribute("testcasecount", testSuite.TestCaseCount.ToString());
                suite.SetAttribute("total", testSuite.Total.ToString());
                suite.SetAttribute("result", testSuite.Result.ToString());
                suite.SetAttribute("passed", testSuite.Passed.ToString());
                suite.SetAttribute("failed", testSuite.Failed.ToString());
                suite.SetAttribute("skipped", testSuite.Skipped.ToString());
                suite.SetAttribute("inconclusive", testSuite.Inconclusive.ToString());
                suite.SetAttribute("run-date", testSuite.StartTime.ToString("yyyy-MM-dd"));
                suite.SetAttribute("start-time", testSuite.StartTime.ToString("hh:mm:ss"));
                suite.SetAttribute("end-time", testSuite.EndTime.ToString("hh:mm:ss"));
                suite.SetAttribute("duration", ((double)((testSuite.EndTime - testSuite.StartTime).TotalMilliseconds) / 1000.0).ToString("0.000"));

                run.AppendChild(suite);

                foreach(var testCase in testSuite.TestCases)
                {
                    var tc = doc.CreateElement(string.Empty, "test-case", string.Empty);
                    tc.SetAttribute("id", testCase.Id.ToString());
                    tc.SetAttribute("name", testCase.Name);
                    tc.SetAttribute("result", testCase.Result.ToString());

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
    }

    public class NUnitSuite
    {
        public DateTime StartTime { get; } = DateTime.Now;
        public DateTime EndTime { get; private set; } = DateTime.Now;
        public int Id { get; set; }
        public int Passed => TestCases.Count(c => c.Result == NUnitTestResult.Passed);
        public int Failed => TestCases.Count(c => c.Result == NUnitTestResult.Failed);
        public int Inconclusive => TestCases.Count(c => c.Result == NUnitTestResult.Inconclusive);
        public int Skipped => TestCases.Count(c => c.Result == NUnitTestResult.Skipped);
        public int TestCaseCount => TestCases.Count;
        public int Total => TestCases.Count(c => c.Result != NUnitTestResult.Skipped);
        public NUnitTestResult Result => Failed == 0 ? NUnitTestResult.Passed : NUnitTestResult.Failed;

        public string Name { get; set; }
        public List<NUnitTestCase> TestCases { get; } = new List<NUnitTestCase>();

        public void Pass(int id, string caseName, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new NUnitTestCase { Id = id.ToString(), Name = caseName, Result = NUnitTestResult.Passed };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }

        public void Skip(int id, string caseName, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new NUnitTestCase { Id = id.ToString(), Name = caseName, Result = NUnitTestResult.Skipped };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }

        public void Inconclusion(int id, string caseName, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new NUnitTestCase { Id = id.ToString(), Name = caseName, Result = NUnitTestResult.Inconclusive };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }

        public void Fail(int id, string caseName, string message, string stackTrace, IReadOnlyDictionary<string, string> properties)
        {
            var testCase = new NUnitTestCase { Id = id.ToString(), Name = caseName, Result = NUnitTestResult.Failed };
            testCase.Failure = new NUnitFailure { Message = message, StackTrace = stackTrace };
            if (properties != null) foreach (var prop in properties) testCase.Properties.Add(prop.Key, prop.Value);
            this.TestCases.Add(testCase);
            EndTime = DateTime.Now;
        }
    }

    public class NUnitTestCase
    {
        public NUnitTestResult Result { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        public NUnitFailure Failure { get; set; }
    }

    public enum NUnitTestResult
    {
        Passed,
        Failed,
        Inconclusive,
        Skipped
    }

    public class NUnitFailure
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
