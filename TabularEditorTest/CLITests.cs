using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TabularEditor
{
    [TestClass]
    public class CLITests
    {
        [TestMethod]
        public void BasicPQCredentialScript()
        {
            var result = CommandLine("TestData\\PQSourceModel.bim", "-S", "TestData\\SetPQSourceCredentials.csx");
            Assert.AreEqual(0, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void Issue606_PQCredentialScript()
        {
            Environment.SetEnvironmentVariable("Name", "New PQ DS Name");
            var result = CommandLine("TestData\\PQSourceModel.bim", "-S", "TestData\\SetPQSourceCredentials2.csx");

            Assert.AreEqual(0, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        public static CommandLineResult CommandLine(params string[] args)
        {
            var handler = new CommandLineHandler();
            Program.CommandLine = handler;
            var argsList = new List<string>();
            argsList.Add(Environment.CurrentDirectory);
            argsList.AddRange(args);

            var stringWriter = new StringWriter();
            var originalOutput = Console.Out;
            Console.SetOut(stringWriter);

            handler.HandleCommandLine(argsList.ToArray());
            return new CommandLineResult {
                ErrorCount = handler.ErrorCount,
                WarningCount = handler.WarningCount,
                Output = stringWriter.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
            };
        }

        [TestMethod]
        public void MultipleScripts()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S", "Model.AllMeasures.Count().Output();", "Model.Tables[0].AddMeasure(\"Test123\", \"1+1\");", "Model.AllMeasures.Count().Output();");

            Assert.IsTrue(result.Output.Any(s => s == "Executing script 0..."));
            Assert.IsTrue(result.Output.Any(s => s == "Executing script 1..."));
            Assert.IsTrue(result.Output.Any(s => s == "Executing script 2..."));

            var i0 = Array.IndexOf(result.Output, "Executing script 0...");
            Assert.IsTrue(result.Output[i0 + 1].StartsWith("Script output"));

            Assert.AreEqual(0, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void MultipleScriptsFileNotFound()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S", "Model.AllMeasures.Count().Output();", "InvalidFile.Name", "Model.AllMeasures.Count().Output();");

            Assert.IsFalse(result.Output.Any(s => s == "Executing script 0..."));
            Assert.IsFalse(result.Output.Any(s => s == "Executing script 1..."));
            Assert.IsFalse(result.Output.Any(s => s == "Executing script 2..."));
            Assert.IsTrue(result.Output.Any(s => s == "Script file not found: InvalidFile.Name"));

            Assert.AreEqual(1, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void MultipleScriptsInvalidScript()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S", "Model.AllMeasures.Count().Output();", "DoesNotCompile();", "Model.AllMeasures.Count().Output();");

            Assert.IsTrue(result.Output.Any(s => s == "Executing script 0..."));
            Assert.IsTrue(result.Output.Any(s => s == "Executing script 1..."));
            Assert.IsTrue(result.Output.Any(s => s == "Script compilation errors:"));
            Assert.IsFalse(result.Output.Any(s => s == "Executing script 2..."));

            Assert.IsTrue(result.ErrorCount > 0);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void ScriptFromFile()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S", "TestData\\Silly01.csx");

            Assert.IsTrue(result.Output.Any(s => s == "Executing script 0..."));
            var i0 = Array.IndexOf(result.Output, "Executing script 0...");
            Assert.IsTrue(result.Output[i0 + 1].StartsWith("Script output"));

            Assert.AreEqual(0, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void Deployment()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-D", "localhost", "AdventureWorks", "-O", "-C", "-P", "-R", "-M", "-E", "-W");

            Assert.IsTrue(result.Output.Any(s => s == "Deploying..."));
        }

        [TestMethod]
        public void NoScripts()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S");

            Assert.IsTrue(result.Output.Any(s => s == "No scripts / script files provided"));

            Assert.AreEqual(1, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void ScriptFromFiles()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S", "TestData\\Silly01.csx", "TestData\\Silly01.csx");

            Assert.IsTrue(result.Output.Any(s => s == "Executing script 0..."));
            var i0 = Array.IndexOf(result.Output, "Executing script 0...");
            Assert.IsTrue(result.Output[i0 + 1].StartsWith("Script output"));

            Assert.IsTrue(result.Output.Any(s => s == "Executing script 1..."));
            var i1 = Array.IndexOf(result.Output, "Executing script 1...");
            Assert.IsTrue(result.Output[i1 + 1].StartsWith("Script output"));

            Assert.AreEqual(0, result.ErrorCount);
            Assert.AreEqual(0, result.WarningCount);
        }

        [TestMethod]
        public void ScriptFromFilesWithError()
        {
            var result = CommandLine("TestData\\AdventureWorks.bim", "-S", "TestData\\Silly01.csx", "TestData\\Silly02-WithErrors.csx");

            Assert.IsTrue(result.Output.Any(s => s == "Executing script 0..."));
            var i0 = Array.IndexOf(result.Output, "Executing script 0...");
            Assert.IsTrue(result.Output[i0 + 1].StartsWith("Script output"));

            Assert.IsTrue(result.Output.Any(s => s == "Script compilation errors:"));

            Assert.IsTrue(result.ErrorCount > 1);
            Assert.AreEqual(0, result.WarningCount);
        }
    }

    public class CommandLineResult
    {
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public string[] Output { get; set; }
    }
}
