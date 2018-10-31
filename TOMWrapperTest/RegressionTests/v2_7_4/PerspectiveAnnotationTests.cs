using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;

namespace TOMWrapperTest.RegressionTests.v2_7_4
{
    /// <summary>
    /// https://github.com/otykier/TabularEditor/issues/214
    /// </summary>
    [TestClass]
    public class PerspectiveAnnotationTests
    {
        [TestMethod]
        public void CheckPerspectiveAnnotationAfterSerialization()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var p1 = model.AddPerspective("TestPerspective");
            p1.SetAnnotation("TestAnnotation", "TestAnnotationValue");

            Directory.CreateDirectory("test_2_7_4_perspective_annotation");
            handler.Save("test_2_7_4_perspective_annotation", SaveFormat.TabularEditorFolder, new SerializeOptions { LocalPerspectives = true });

            Assert.AreEqual("TestAnnotationValue", p1.GetAnnotation("TestAnnotation"));

            // Load from folder:
            handler = new TabularModelHandler("test_2_7_4_perspective_annotation");
            model = handler.Model;
            p1 = model.Perspectives["TestPerspective"];
            Assert.AreEqual("TestAnnotationValue", p1.GetAnnotation("TestAnnotation"));
        }

        [TestMethod]
        public void CheckPerspectiveAnnotationAfterSave()
        {
            var handler = new TabularModelHandler();
            var model = handler.Model;
            var p1 = model.AddPerspective("TestPerspective");
            p1.SetAnnotation("TestAnnotation", "TestAnnotationValue");

            Directory.CreateDirectory("test_2_7_4_perspective_annotation_2");
            handler.Save("test_2_7_4_perspective_annotation_2", SaveFormat.TabularEditorFolder, SerializeOptions.Default);

            Assert.AreEqual("TestAnnotationValue", p1.GetAnnotation("TestAnnotation"));

            // Load from folder:
            handler = new TabularModelHandler("test_2_7_4_perspective_annotation_2");
            model = handler.Model;
            p1 = model.Perspectives["TestPerspective"];
            Assert.AreEqual("TestAnnotationValue", p1.GetAnnotation("TestAnnotation"));
        }
    }
}
