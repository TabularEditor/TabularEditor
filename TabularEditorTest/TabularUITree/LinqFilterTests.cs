using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aga.Controls.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;

namespace TabularEditor
{
    [TestClass]
    public class LinqFilterTests
    {
        [TestMethod]
        public void BasicLinqFilterTest()
        {
            var handler = new TabularModelHandler(Constants.ServerName, "TomWrapperTest");
            var uiTree = new TabularUITree(handler);

            uiTree.FilterMode = FilterMode.Flat;

            uiTree.Filter = null;
            uiTree.Filter = ":ObjectType=\"Table\"";
            var items = uiTree.GetChildrenInternal(TreePath.Empty).ToList();

            Assert.AreEqual(15, items.Count);

        }

        [TestMethod]
        public void NullSafetyTest()
        {
            var handler = new TabularModelHandler(Constants.ServerName, "TomWrapperTest");
            var uiTree = new TabularUITree(handler);
            handler.Model.Tables["Currency"].SetAnnotation("NotNull", "This is a test");

            uiTree.FilterMode = FilterMode.Flat;

            uiTree.Filter = null;
            uiTree.Filter = ":GetAnnotation(\"NotNull\").Contains(\"test\")";
            var items = uiTree.GetChildrenInternal(TreePath.Empty).ToList();

            Assert.AreEqual(1, items.Count);
        }

        [TestMethod]
        public void PerformanceTest()
        {
            var handler = new TabularModelHandler(Constants.ServerName, "TomWrapperTest");
            var uiTree = new TabularUITree(handler);
            handler.Model.Tables["Currency"].SetAnnotation("NotNull", "This is a test");

            uiTree.FilterMode = FilterMode.Flat;

            var items = new List<ITabularNamedObject>();

            uiTree.Filter = null;
            uiTree.Filter = ":GetAnnotation(\"NotNull\").Contains(\"test\")";


            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                items.AddRange(uiTree.GetChildrenInternal(TreePath.Empty));
            }
            sw.Stop();

            // Run 1: 3935
            // Run 2: 4042

            Assert.IsTrue(sw.ElapsedMilliseconds < 1000);
            Assert.AreEqual(1000, items.Count);
        }
    }
}
