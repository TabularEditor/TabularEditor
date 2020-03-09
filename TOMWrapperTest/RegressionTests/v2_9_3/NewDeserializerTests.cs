using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest.RegressionTests.v2_9_3
{
    [TestClass]
    public class NewDeserializerTests
    {
        [TestMethod]
        public void DeserializeAllTest()
        {
            if (Directory.Exists("NewDeserializer")) Directory.Delete("NewDeserializer", true);
            ZipFile.ExtractToDirectory("NewDeserializer.zip", ".");
            var handler = new TabularModelHandler("NewDeserializer");
            var model = handler.Model;

        }
    }
}
