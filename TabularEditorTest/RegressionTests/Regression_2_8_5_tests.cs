using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TabularEditor.RegressionTests
{
    [TestClass]
    public class Regression_2_8_5_tests
    {
        [TestMethod]
        public void CreateNewMeasureSameTableUsingHandlerTest()
        {
            UiTester.EnsureCorrectExceptionMode();
            using (var fm = new FormMain())
            {
                var ui = fm.UI;
                fm.Show();

                ui.Database_Open(Constants.ServerName, "TomWrapperTest");
                var model = ui.Handler.Model;

                ui.Goto(model.Tables["Employee"]);
                ui.ShowDependencies(model.Tables["Employee"]);

                Application.DoEvents();

                var treeView = (Application.OpenForms["DependencyForm"] as DependencyForm).treeObjects;
                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreSame(model.Tables["Employee"], treeView.Nodes[0].Tag);

                var childrenBefore = treeView.Nodes[0].Nodes.Count;

                var m1 = model.Tables["Employee"].AddMeasure("TestEmployeeTableRef");
                m1.Expression = "COUNTROWS('Employee')";
                Assert.AreEqual(childrenBefore + 1, treeView.Nodes[0].Nodes.Count);
                Assert.IsTrue(treeView.Nodes[0].Nodes.OfType<TreeNode>().Any(n => n.Tag == m1));

                var m2 = model.Tables["Employee"].AddMeasure("TestEmployeeTableRef");
                m2.Expression = "SUM('Employee'[Phone])";
                Assert.AreEqual(childrenBefore + 2, treeView.Nodes[0].Nodes.Count);
                Assert.IsTrue(treeView.Nodes[0].Nodes.OfType<TreeNode>().Any(n => n.Tag == m2));
            }
        }

        [TestMethod]
        public void CreateNewMeasureOtherTableUsingHandlerTest()
        {
            UiTester.EnsureCorrectExceptionMode();
            using (var fm = new FormMain())
            {
                var ui = fm.UI;
                fm.Show();

                ui.Database_Open(Constants.ServerName, "TomWrapperTest");
                var model = ui.Handler.Model;

                ui.Goto(model.Tables["Employee"]);
                ui.ShowDependencies(model.Tables["Employee"]);

                Application.DoEvents();

                var treeView = (Application.OpenForms["DependencyForm"] as DependencyForm).treeObjects;
                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreSame(model.Tables["Employee"], treeView.Nodes[0].Tag);

                var childrenBefore = treeView.Nodes[0].Nodes.Count;

                var m1 = model.Tables["Customer"].AddMeasure("TestEmployeeTableRef");
                m1.Expression = "COUNTROWS('Employee')";
                Assert.AreEqual(childrenBefore + 1, treeView.Nodes[0].Nodes.Count);
                Assert.IsTrue(treeView.Nodes[0].Nodes.OfType<TreeNode>().Any(n => n.Tag == m1));

                var m2 = model.Tables["Customer"].AddMeasure("TestEmployeeTableRef");
                m2.Expression = "SUM('Employee'[Phone])";
                Assert.AreEqual(childrenBefore + 2, treeView.Nodes[0].Nodes.Count);
                Assert.IsTrue(treeView.Nodes[0].Nodes.OfType<TreeNode>().Any(n => n.Tag == m2));
            }
        }
        
        [TestMethod]
        public void CreateNewMeasureSameTableUsingUITest()
        {
            UiTester.EnsureCorrectExceptionMode();
            using (var fm = new FormMain())
            {
                fm.Show();
                var ui = fm.UI;

                ui.Database_Open(Constants.ServerName, "TomWrapperTest");
                var model = ui.Handler.Model;

                ui.Goto(model.Tables["Employee"]);
                ui.ShowDependencies(model.Tables["Employee"]);

                Application.DoEvents();

                var treeView = (Application.OpenForms["DependencyForm"] as DependencyForm).treeObjects;
                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreSame(model.Tables["Employee"], treeView.Nodes[0].Tag);
                var childrenBefore = treeView.Nodes[0].Nodes.Count;

                ui.Actions.OfType<UI.Actions.Action>().First(a => a.Name == "Create New\\Measure").Execute(null);
                Application.DoEvents();
                ui.ExpressionEditor_BeginEdit();
                SendKeys.SendWait("COUNTROWS{(}'Employee'{)}");
                Application.DoEvents();
                ui.ExpressionEditor_AcceptEdit();

                Assert.IsTrue(model.Tables["Employee"].Measures.Contains("New measure"));
                var m1 = model.Tables["Employee"].Measures["New measure"];
                Assert.AreEqual("COUNTROWS('Employee')", m1.Expression);
                Assert.AreEqual(childrenBefore + 1, treeView.Nodes[0].Nodes.Count);
                Assert.IsTrue(treeView.Nodes[0].Nodes.OfType<TreeNode>().Any(n => n.Tag == m1));
            }
        }

        [TestMethod]
        public void CreateNewMeasureSameTableUsingScriptTest()
        {
            UiTester.EnsureCorrectExceptionMode();
            using (var fm = new FormMain())
            {
                fm.Show();
                var ui = fm.UI;

                ui.Database_Open(Constants.ServerName, "TomWrapperTest");
                var model = ui.Handler.Model;

                ui.Goto(model.Tables["Employee"]);
                ui.ShowDependencies(model.Tables["Employee"]);

                Application.DoEvents();

                var treeView = (Application.OpenForms["DependencyForm"] as DependencyForm).treeObjects;
                Assert.AreEqual(1, treeView.Nodes.Count);
                Assert.AreSame(model.Tables["Employee"], treeView.Nodes[0].Tag);
                var childrenBefore = treeView.Nodes[0].Nodes.Count;

                ui.ExecuteScript(@"
var m1 = Model.Tables[""Employee""].AddMeasure(""TestEmployee"");
m1.Expression = ""COUNTROWS('Employee')"";");

                Assert.IsTrue(model.Tables["Employee"].Measures.Contains("TestEmployee"));
                var m1 = model.Tables["Employee"].Measures["TestEmployee"];
                Assert.AreEqual("COUNTROWS('Employee')", m1.Expression);
                Assert.AreEqual(childrenBefore + 1, treeView.Nodes[0].Nodes.Count);
                Assert.IsTrue(treeView.Nodes[0].Nodes.OfType<TreeNode>().Any(n => n.Tag == m1));
            }
        }
    }
}
