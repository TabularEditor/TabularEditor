using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using System.Linq;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    [TestClass]
    public class UndoRedoTests
    {
        [TestMethod]
        public void DependenciesUndoTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddCalculatedTable("New table");
            var m1 = t1.AddMeasure("Measure 1", "123");
            var m2 = t1.AddMeasure("Measure 2", "234");
            var m3 = t1.AddMeasure("Measure 3", "[Measure 1] + [Measure 2]");

            Assert.AreEqual(1, m1.ReferencedBy.Measures.Count());
            Assert.AreEqual(m3, m1.ReferencedBy.Measures.First());

            Assert.AreEqual(2, m3.DependsOn.Measures.Count());

            m3.Delete();

            Assert.AreEqual(0, m1.ReferencedBy.Measures.Count());

            handler.UndoManager.Undo();

            m3 = t1.Measures["Measure 3"];
            Assert.AreEqual(1, m1.ReferencedBy.Measures.Count());
            Assert.AreEqual(m3, m1.ReferencedBy.Measures.First());

            Assert.AreEqual(2, m3.DependsOn.Measures.Count());

        }

        [TestMethod]
        public void DeleteColumnRoleDependencyTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddCalculatedTable("New table");
            var c1 = t1.AddCalculatedColumn("Column 1");

            var r1 = model.AddRole("Role 1");
            r1.RowLevelSecurity[t1] = "[Column 1] = 123";

            Assert.AreEqual(1, c1.ReferencedBy.Roles.Count());
            Assert.AreEqual(1, r1.TablePermissions[t1].DependsOn.Columns.Count());

            c1.Delete();

            Assert.AreEqual(0, r1.TablePermissions[t1].DependsOn.Columns.Count());

            handler.UndoManager.Undo();

            c1 = t1.Columns["Column 1"] as CalculatedColumn;
            Assert.AreEqual(1, c1.ReferencedBy.Roles.Count());
            Assert.AreEqual(1, r1.TablePermissions[t1].DependsOn.Columns.Count());

        }

        [TestMethod]
        public void DeleteRoleColumnDependencyTest()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var t1 = model.AddCalculatedTable("New table");
            var c1 = t1.AddCalculatedColumn("Column 1");

            var r1 = model.AddRole("Role 1");
            r1.RowLevelSecurity[t1] = "[Column 1] = 123";

            Assert.AreEqual(1, c1.ReferencedBy.Roles.Count());
            Assert.AreEqual(1, r1.TablePermissions[t1].DependsOn.Columns.Count());

            r1.Delete();

            Assert.AreEqual(0, c1.ReferencedBy.Roles.Count());

            handler.UndoManager.Undo();

            r1 = model.Roles["Role 1"];
            Assert.AreEqual(1, c1.ReferencedBy.Roles.Count());
            Assert.AreEqual(1, r1.TablePermissions[t1].DependsOn.Columns.Count());

        }
    }
}
