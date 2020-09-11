using System;
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
    }

    public class CommandLineResult
    {
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public string[] Output { get; set; }
    }
}
