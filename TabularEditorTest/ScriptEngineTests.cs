using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TabularEditor
{
    [TestClass]
    public class ScriptEngineTests
    {
        [TestMethod]
        public void AddOutputLineNumbersNoArgsMultiline2()
        {
            var original = @"var x = ""test"";
x.Output();
// This is a comment";

            var expected = @"var x = ""test"";
x.Output(2);
// This is a comment";

            Assert.AreEqual(expected, ScriptEngine.AddOutputLineNumbers(original));
        }

        [TestMethod]
        public void AddOutputLineNumbersNoArgsSingleLine()
        {
            var original = "Output();";
            var expected = "Output(1);";
            Assert.AreEqual(expected, ScriptEngine.AddOutputLineNumbers(original));
        }
        [TestMethod]
        public void AddOutputLineNumbersNoArgsMultiline1()
        {
            var original = @"Output(
);";
            var expected = @"Output(
1);";
            Assert.AreEqual(expected, ScriptEngine.AddOutputLineNumbers(original));
        }

        [TestMethod]
        public void AddOutputLineNumbersMultiline2()
        {
            var original = @"var x = ""test"";
Output(x);
// This is a comment";

            var expected = @"var x = ""test"";
Output(x,2);
// This is a comment";

            Assert.AreEqual(expected, ScriptEngine.AddOutputLineNumbers(original));
        }

        [TestMethod]
        public void AddOutputLineNumbersSingleLine()
        {
            var original = "Output(\"Test\");";
            var expected = "Output(\"Test\",1);";
            Assert.AreEqual(expected, ScriptEngine.AddOutputLineNumbers(original));
        }
        [TestMethod]
        public void AddOutputLineNumbersMultiline1()
        {
            var original = @"Output(
    ""Test""
);";
            var expected = @"Output(
    ""Test""
,1);";
            Assert.AreEqual(expected, ScriptEngine.AddOutputLineNumbers(original));
        }

        [TestMethod]
        public void ReplaceGlobalMethodCallsNoChange()
        {
            var original = @"var x = ""test"";
x.Output();
// This is a comment";

            var expected = @"var x = ""test"";
x.Output();
// This is a comment";

            Assert.AreEqual(expected, ScriptEngine.ReplaceGlobalMethodCalls(original));
        }

        [TestMethod]
        public void ReplaceGlobalMethodCallsNoArgsSingleLine()
        {
            ScriptEngine.InitScriptEngine(new List<Assembly>());

            var original = "Output();";
            var expected = "TabularEditor.Scripting.ScriptHelper.Output();";
            Assert.AreEqual(expected, ScriptEngine.ReplaceGlobalMethodCalls(original));
        }
        [TestMethod]
        public void ReplaceGlobalMethodCallsNoArgsMultiline1()
        {
            var original = @"Output(
);";
            var expected = @"TabularEditor.Scripting.ScriptHelper.Output(
);";
            Assert.AreEqual(expected, ScriptEngine.ReplaceGlobalMethodCalls(original));
        }

        [TestMethod]
        public void ReplaceGlobalMethodCallsMultiline2()
        {
            var original = @"var x = ""test"";
Output(x);
// This is a comment";

            var expected = @"var x = ""test"";
TabularEditor.Scripting.ScriptHelper.Output(x);
// This is a comment";

            Assert.AreEqual(expected, ScriptEngine.ReplaceGlobalMethodCalls(original));
        }

        [TestMethod]
        public void ReplaceGlobalMethodCallsSingleLine()
        {
            var original = "Output(\"Test\");";
            var expected = "TabularEditor.Scripting.ScriptHelper.Output(\"Test\");";
            Assert.AreEqual(expected, ScriptEngine.ReplaceGlobalMethodCalls(original));
        }
        [TestMethod]
        public void ReplaceGlobalMethodCallsMultiline1()
        {
            var original = @"Output(
    ""Test""
);";
            var expected = @"TabularEditor.Scripting.ScriptHelper.Output(
    ""Test""
);";
            Assert.AreEqual(expected, ScriptEngine.ReplaceGlobalMethodCalls(original));
        }
    }
}
