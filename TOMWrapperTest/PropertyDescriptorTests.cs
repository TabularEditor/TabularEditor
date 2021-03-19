using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TOMWrapperTest
{
    [TestClass]
    public class PropertyDescriptorTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void CL1200ModelTest()
        {

            var handler = new TabularModelHandler("TestData\\AdventureWorks.bim");
            handler.Model.SetAnnotation("test", "value");
            handler.Model.PropertyGridTest()
                .IsHidden(nameof(Model.Handler))
                .IsReadWrite(nameof(Model.Name))
                .IsReadWrite(nameof(Model.DeploymentMetadata))
                .IsReadWrite(nameof(Model.Description))
                .IsReadOnly(nameof(Model.Database))

                .IsReadOnly(nameof(Model.Perspectives))
                .IsReadOnly(nameof(Model.ObjectTypeName))

                .IsReadOnly(nameof(Model.Annotations), "The annotations property is read only but existing annotations should be writable")
                .IsReadWrite($@"{nameof(Model.Annotations)}\test")

                .IsHidden(nameof(Model.ObjectType))
                .IsHidden(nameof(Model.ExtendedProperties))
                .IsHidden(nameof(Model.AllColumns))
                .IsHidden(nameof(Model.Expressions), "Expressions should not be available for a CL1200 model.")
                .IsHidden(nameof(Model.CalculationGroups), "Calculation Groups should not be available as a property.")
                ;

            var measure = handler.Model.AllMeasures.First(m => m.FormatString == @"\$#,0.00;(\$#,0.00);\$#,0.00");

            measure.PropertyGridTest()
                .AreReadWrite(nameof(Measure.Name), nameof(Measure.Description))
                .Assert(nameof(Measure.ObjectTypeName), true, "Object Type")
                .AreReadOnly(nameof(Measure.State), nameof(Measure.ErrorMessage), nameof(Measure.DaxObjectFullName), nameof(Measure.DataType))
                .AreHidden(nameof(Measure.DaxObjectName), nameof(Measure.DaxTableName), nameof(Measure.ReferencedBy), nameof(Measure.Handler), nameof(Measure.Model))

                .IsHidden(nameof(Measure.TranslatedNames), "Model does not contain any cultures")

                .Assert(nameof(Measure.InPerspective), readOnly: true, displayName: "Shown in Perspective")
                .IsReadWrite($@"{nameof(Measure.InPerspective)}\Inventory")

                .IsReadWrite(nameof(Measure.FormatString))
                .IsReadOnly($@"{nameof(Measure.FormatString)}\Example")
                .IsReadWrite($@"{nameof(Measure.FormatString)}\NumberFormat")
                .IsHidden($@"{nameof(Measure.FormatString)}\ParenthesisForNegative")
                .IsHidden($@"{nameof(Measure.FormatString)}\ThousandSeparators")
                .IsReadWrite(nameof(Measure.FormatString))
                ;
            measure.FormatString = "#,##0";
            measure.PropertyGridTest()
                .Dump()
                .IsReadWrite(nameof(Measure.FormatString))
                .IsReadOnly($@"{nameof(Measure.FormatString)}\Example")
                .IsReadWrite($@"{nameof(Measure.FormatString)}\NumberFormat")
                .IsReadWrite($@"{nameof(Measure.FormatString)}\ParenthesisForNegative")
                .IsReadWrite($@"{nameof(Measure.FormatString)}\ThousandSeparators")
                .IsReadWrite(nameof(Measure.FormatString))
                ;

            foreach (var item in handler.Model.GetChildrenRecursive(false))
                item.PropertyGridTest().AreHidden("Model", "Handler").NotHasCategory("Misc");
        }

        [TestMethod]
        public void PBIUnrestrictedTest()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1520v3.pbit", new TabularModelHandlerSettings { PBIFeaturesOnly = false });

            handler.Model.SetAnnotation("test", "value");
            handler.Model.PropertyGridTest()
                .IsHidden(nameof(Model.Handler))
                .IsReadWrite(nameof(Model.Name))
                .IsReadWrite(nameof(Model.DeploymentMetadata))
                .IsReadWrite(nameof(Model.Description))
                .IsReadOnly(nameof(Model.Database))

                .IsReadOnly(nameof(Model.Perspectives))
                .IsReadOnly(nameof(Model.ObjectTypeName))

                .IsReadOnly(nameof(Model.Annotations), "The annotations property is read only but existing annotations should be writable")
                .IsReadWrite($@"{nameof(Model.Annotations)}\test")

                .IsReadOnly(nameof(Model.ExtendedProperties))
                .IsReadOnly(nameof(Model.Expressions), "Expressions should not be available for a CL1520 model.")

                .IsHidden(nameof(Model.ObjectType))
                .IsHidden(nameof(Model.AllColumns))
                .IsHidden(nameof(Model.CalculationGroups), "Calculation Groups should not be available as a property.")
                ;

            foreach (var item in handler.Model.GetChildrenRecursive(false))
                item.PropertyGridTest().AreHidden("Model", "Handler").NotHasCategory("Misc");
        }

        [TestMethod]
        public void PBIRestrictedTest()
        {
            var handler = new TabularModelHandler("TestData\\AdvWorks1520v3.pbit", new TabularModelHandlerSettings { PBIFeaturesOnly = true });

            handler.Model.SetAnnotation("test", "value");
            handler.Model.PropertyGridTest()
                .IsHidden(nameof(Model.Handler))
                .IsReadOnly(nameof(Model.Name))
                .IsHidden(nameof(Model.DeploymentMetadata))
                .IsReadWrite(nameof(Model.Description))
                .IsReadOnly(nameof(Model.Database))

                .IsHidden(nameof(Model.Perspectives))
                .IsReadOnly(nameof(Model.ObjectTypeName))

                .IsHidden(nameof(Model.Annotations), "The annotations property is read only but existing annotations should be writable")
                .IsHidden($@"{nameof(Model.Annotations)}\test")

                .IsHidden(nameof(Model.ExtendedProperties))
                .IsHidden(nameof(Model.Expressions), "Expressions should not be available for a CL1520 model.")

                .IsHidden(nameof(Model.ObjectType))
                .IsHidden(nameof(Model.AllColumns))
                .IsHidden(nameof(Model.CalculationGroups), "Calculation Groups should not be available as a property.")
                ;

            foreach (var item in handler.Model.GetChildrenRecursive(false))
                item.PropertyGridTest().AreHidden("Model", "Handler").NotHasCategory("Misc");
        }
    }
}
