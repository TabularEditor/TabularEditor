using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;

namespace TOMWrapperTest.RegressionTests.v2_9_3
{
    [TestClass]
    public class NewDeserializerTests
    {
        [TestMethod]
        public void DeserializeAllTest()
        {
            var handler = new TabularModelHandler("NewDeserializer.bim");
            var model = handler.Model;

            handler.Save("NewDeserializerFolderTest", SaveFormat.TabularEditorFolder, SerializeOptions.DefaultFolder, true);

            handler = new TabularModelHandler("NewDeserializerFolderTest");
            handler.Save("NewDeserializerAfterLoad.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default, true);

            var file1 = File.ReadAllText("NewDeserializer.bim");
            var file2 = File.ReadAllText("NewDeserializerAfterLoad.bim");

            Assert.AreEqual(file1, file2);
        }
    }
}
