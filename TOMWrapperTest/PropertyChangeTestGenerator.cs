using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper.Serialization;

namespace TabularEditor.TOMWrapper.GeneratedTests
{
	[TestClass]
	public class ModelGeneratedTests
	{
        [TestMethod]
        public void ModelDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void ModelStorageLocationTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.StorageLocation;
            var value = orgValue;

            value = "fr-FR";
            if(obj.StorageLocation != value)
            {
                obj.StorageLocation = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StorageLocation);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.StorageLocation);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StorageLocation);
            }
        }
        [TestMethod]
        public void ModelDefaultModeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.DefaultMode;

            if(obj.DefaultMode != ModeType.Import)
            {
                obj.DefaultMode = ModeType.Import;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModeType.Import, obj.DefaultMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultMode);
            }

            if(obj.DefaultMode != ModeType.DirectQuery)
            {
                obj.DefaultMode = ModeType.DirectQuery;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModeType.DirectQuery, obj.DefaultMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultMode);
            }
        }    
        [TestMethod]
        public void ModelDefaultDataViewTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.DefaultDataView;

            if(obj.DefaultDataView != DataViewType.Full)
            {
                obj.DefaultDataView = DataViewType.Full;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDataView);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataViewType.Full, obj.DefaultDataView);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDataView);
            }

            if(obj.DefaultDataView != DataViewType.Sample)
            {
                obj.DefaultDataView = DataViewType.Sample;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDataView);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataViewType.Sample, obj.DefaultDataView);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDataView);
            }
        }    
        [TestMethod]
        public void ModelCultureTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.Culture;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Culture != value)
            {
                obj.Culture = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Culture);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Culture);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Culture);
            }
        }
        [TestMethod]
        public void ModelCollationTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.Collation;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Collation != value)
            {
                obj.Collation = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Collation);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Collation);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Collation);
            }
        }
        [TestMethod]
        public void ModelDefaultPowerBIDataSourceVersionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.DefaultPowerBIDataSourceVersion;

            if(obj.DefaultPowerBIDataSourceVersion != PowerBIDataSourceVersion.PowerBI_V1)
            {
                obj.DefaultPowerBIDataSourceVersion = PowerBIDataSourceVersion.PowerBI_V1;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultPowerBIDataSourceVersion);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(PowerBIDataSourceVersion.PowerBI_V1, obj.DefaultPowerBIDataSourceVersion);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultPowerBIDataSourceVersion);
            }

            if(obj.DefaultPowerBIDataSourceVersion != PowerBIDataSourceVersion.PowerBI_V2)
            {
                obj.DefaultPowerBIDataSourceVersion = PowerBIDataSourceVersion.PowerBI_V2;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultPowerBIDataSourceVersion);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(PowerBIDataSourceVersion.PowerBI_V2, obj.DefaultPowerBIDataSourceVersion);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultPowerBIDataSourceVersion);
            }

            if(obj.DefaultPowerBIDataSourceVersion != PowerBIDataSourceVersion.PowerBI_V3)
            {
                obj.DefaultPowerBIDataSourceVersion = PowerBIDataSourceVersion.PowerBI_V3;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultPowerBIDataSourceVersion);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(PowerBIDataSourceVersion.PowerBI_V3, obj.DefaultPowerBIDataSourceVersion);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DefaultPowerBIDataSourceVersion);
            }
        }    
        [TestMethod]
        public void ModelForceUniqueNamesTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.ForceUniqueNames;
            var value = orgValue;

            value = true;
            if(obj.ForceUniqueNames != value)
            {
                obj.ForceUniqueNames = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ForceUniqueNames);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ForceUniqueNames);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ForceUniqueNames);
            }

            value = false;
            if(obj.ForceUniqueNames != value)
            {
                obj.ForceUniqueNames = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ForceUniqueNames);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ForceUniqueNames);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ForceUniqueNames);
            }
        }
        [TestMethod]
        public void ModelDataSourceDefaultMaxConnectionsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.DataSourceDefaultMaxConnections;
            var value = orgValue;

            value = 123;
            if(obj.DataSourceDefaultMaxConnections != value)
            {
                obj.DataSourceDefaultMaxConnections = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataSourceDefaultMaxConnections);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataSourceDefaultMaxConnections);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataSourceDefaultMaxConnections);
            }
        }
        [TestMethod]
        public void ModelSourceQueryCultureTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.SourceQueryCulture;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceQueryCulture != value)
            {
                obj.SourceQueryCulture = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceQueryCulture);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceQueryCulture);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceQueryCulture);
            }
        }
        [TestMethod]
        public void ModelMAttributesTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.MAttributes;
            var value = orgValue;

            value = "fr-FR";
            if(obj.MAttributes != value)
            {
                obj.MAttributes = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MAttributes);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.MAttributes);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MAttributes);
            }
        }
        [TestMethod]
        public void ModelDiscourageCompositeModelsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.DiscourageCompositeModels;
            var value = orgValue;

            value = true;
            if(obj.DiscourageCompositeModels != value)
            {
                obj.DiscourageCompositeModels = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DiscourageCompositeModels);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DiscourageCompositeModels);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DiscourageCompositeModels);
            }

            value = false;
            if(obj.DiscourageCompositeModels != value)
            {
                obj.DiscourageCompositeModels = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DiscourageCompositeModels);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DiscourageCompositeModels);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DiscourageCompositeModels);
            }
        }
        [TestMethod]
        public void ModelFastCombineTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.FastCombine;
            var value = orgValue;

            value = true;
            if(obj.FastCombine != value)
            {
                obj.FastCombine = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FastCombine);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FastCombine);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FastCombine);
            }

            value = false;
            if(obj.FastCombine != value)
            {
                obj.FastCombine = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FastCombine);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FastCombine);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FastCombine);
            }
        }
        [TestMethod]
        public void ModelLegacyRedirectsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.LegacyRedirects;
            var value = orgValue;

            value = true;
            if(obj.LegacyRedirects != value)
            {
                obj.LegacyRedirects = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LegacyRedirects);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LegacyRedirects);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LegacyRedirects);
            }

            value = false;
            if(obj.LegacyRedirects != value)
            {
                obj.LegacyRedirects = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LegacyRedirects);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LegacyRedirects);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LegacyRedirects);
            }
        }
        [TestMethod]
        public void ModelReturnErrorValuesAsNullTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.ReturnErrorValuesAsNull;
            var value = orgValue;

            value = true;
            if(obj.ReturnErrorValuesAsNull != value)
            {
                obj.ReturnErrorValuesAsNull = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ReturnErrorValuesAsNull);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ReturnErrorValuesAsNull);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ReturnErrorValuesAsNull);
            }

            value = false;
            if(obj.ReturnErrorValuesAsNull != value)
            {
                obj.ReturnErrorValuesAsNull = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ReturnErrorValuesAsNull);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ReturnErrorValuesAsNull);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ReturnErrorValuesAsNull);
            }
        }
        [TestMethod]
        public void ModelNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model;
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class PerspectiveGeneratedTests
	{
        [TestMethod]
        public void PerspectiveDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Perspectives["Perspective"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void PerspectiveNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Perspectives["Perspective"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class CultureGeneratedTests
	{
        [TestMethod]
        public void CultureContentTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Cultures["da-DK"];
            var orgValue = obj.Content;
            var value = orgValue;

            value = "<xml></xml>";
            if(obj.Content != value)
            {
                obj.Content = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Content);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Content);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Content);
            }
        }
        [TestMethod]
        public void CultureNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Cultures["da-DK"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class ProviderDataSourceGeneratedTests
	{
        [TestMethod]
        public void ProviderDataSourceConnectionStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.ConnectionString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.ConnectionString != value)
            {
                obj.ConnectionString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ConnectionString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ConnectionString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ConnectionString);
            }
        }
        [TestMethod]
        public void ProviderDataSourceImpersonationModeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.ImpersonationMode;

            if(obj.ImpersonationMode != ImpersonationMode.ImpersonateAccount)
            {
                obj.ImpersonationMode = ImpersonationMode.ImpersonateAccount;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ImpersonationMode.ImpersonateAccount, obj.ImpersonationMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);
            }

            if(obj.ImpersonationMode != ImpersonationMode.ImpersonateAnonymous)
            {
                obj.ImpersonationMode = ImpersonationMode.ImpersonateAnonymous;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ImpersonationMode.ImpersonateAnonymous, obj.ImpersonationMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);
            }

            if(obj.ImpersonationMode != ImpersonationMode.ImpersonateCurrentUser)
            {
                obj.ImpersonationMode = ImpersonationMode.ImpersonateCurrentUser;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ImpersonationMode.ImpersonateCurrentUser, obj.ImpersonationMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);
            }

            if(obj.ImpersonationMode != ImpersonationMode.ImpersonateServiceAccount)
            {
                obj.ImpersonationMode = ImpersonationMode.ImpersonateServiceAccount;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ImpersonationMode.ImpersonateServiceAccount, obj.ImpersonationMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);
            }

            if(obj.ImpersonationMode != ImpersonationMode.ImpersonateUnattendedAccount)
            {
                obj.ImpersonationMode = ImpersonationMode.ImpersonateUnattendedAccount;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ImpersonationMode.ImpersonateUnattendedAccount, obj.ImpersonationMode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ImpersonationMode);
            }
        }    
        [TestMethod]
        public void ProviderDataSourceAccountTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Account;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Account != value)
            {
                obj.Account = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Account);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Account);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Account);
            }
        }
        [TestMethod]
        public void ProviderDataSourcePasswordTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Password;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Password != value)
            {
                obj.Password = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Password);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Password);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Password);
            }
        }
        [TestMethod]
        public void ProviderDataSourceIsolationTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Isolation;

            if(obj.Isolation != DatasourceIsolation.ReadCommitted)
            {
                obj.Isolation = DatasourceIsolation.ReadCommitted;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Isolation);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DatasourceIsolation.ReadCommitted, obj.Isolation);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Isolation);
            }

            if(obj.Isolation != DatasourceIsolation.Snapshot)
            {
                obj.Isolation = DatasourceIsolation.Snapshot;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Isolation);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DatasourceIsolation.Snapshot, obj.Isolation);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Isolation);
            }
        }    
        [TestMethod]
        public void ProviderDataSourceTimeoutTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Timeout;
            var value = orgValue;

            value = 123;
            if(obj.Timeout != value)
            {
                obj.Timeout = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Timeout);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Timeout);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Timeout);
            }
        }
        [TestMethod]
        public void ProviderDataSourceProviderTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Provider;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Provider != value)
            {
                obj.Provider = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Provider);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Provider);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Provider);
            }
        }
        [TestMethod]
        public void ProviderDataSourceNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
        [TestMethod]
        public void ProviderDataSourceDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void ProviderDataSourceMaxConnectionsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["LegacyDataSource"] as ProviderDataSource);
            var orgValue = obj.MaxConnections;
            var value = orgValue;

            value = 123;
            if(obj.MaxConnections != value)
            {
                obj.MaxConnections = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MaxConnections);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.MaxConnections);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MaxConnections);
            }
        }
    }
	[TestClass]
	public class StructuredDataSourceGeneratedTests
	{
        [TestMethod]
        public void StructuredDataSourceContextExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.ContextExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.ContextExpression != value)
            {
                obj.ContextExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ContextExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ContextExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ContextExpression);
            }
        }
        [TestMethod]
        public void StructuredDataSourceProtocolTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Protocol;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Protocol != value)
            {
                obj.Protocol = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Protocol);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Protocol);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Protocol);
            }
        }
        [TestMethod]
        public void StructuredDataSourceUsernameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Username;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Username != value)
            {
                obj.Username = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Username);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Username);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Username);
            }
        }
        [TestMethod]
        public void StructuredDataSourcePasswordTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Password;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Password != value)
            {
                obj.Password = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Password);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Password);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Password);
            }
        }
        [TestMethod]
        public void StructuredDataSourcePrivacySettingTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.PrivacySetting;
            var value = orgValue;

            value = "fr-FR";
            if(obj.PrivacySetting != value)
            {
                obj.PrivacySetting = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.PrivacySetting);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.PrivacySetting);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.PrivacySetting);
            }
        }
        [TestMethod]
        public void StructuredDataSourceAuthenticationKindTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.AuthenticationKind;
            var value = orgValue;

            value = "fr-FR";
            if(obj.AuthenticationKind != value)
            {
                obj.AuthenticationKind = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AuthenticationKind);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.AuthenticationKind);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AuthenticationKind);
            }
        }
        [TestMethod]
        public void StructuredDataSourceEncryptConnectionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.EncryptConnection;
            var value = orgValue;

            value = true;
            if(obj.EncryptConnection != value)
            {
                obj.EncryptConnection = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EncryptConnection);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EncryptConnection);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EncryptConnection);
            }

            value = false;
            if(obj.EncryptConnection != value)
            {
                obj.EncryptConnection = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EncryptConnection);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EncryptConnection);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EncryptConnection);
            }
        }
        [TestMethod]
        public void StructuredDataSourceAccountTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Account;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Account != value)
            {
                obj.Account = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Account);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Account);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Account);
            }
        }
        [TestMethod]
        public void StructuredDataSourceConnectionStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.ConnectionString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.ConnectionString != value)
            {
                obj.ConnectionString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ConnectionString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ConnectionString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ConnectionString);
            }
        }
        [TestMethod]
        public void StructuredDataSourceContentTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.ContentType;
            var value = orgValue;

            value = "fr-FR";
            if(obj.ContentType != value)
            {
                obj.ContentType = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ContentType);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ContentType);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ContentType);
            }
        }
        [TestMethod]
        public void StructuredDataSourceDatabaseTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Database;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Database != value)
            {
                obj.Database = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Database);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Database);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Database);
            }
        }
        [TestMethod]
        public void StructuredDataSourceDomainTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Domain;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Domain != value)
            {
                obj.Domain = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Domain);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Domain);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Domain);
            }
        }
        [TestMethod]
        public void StructuredDataSourceEmailAddressTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.EmailAddress;
            var value = orgValue;

            value = "fr-FR";
            if(obj.EmailAddress != value)
            {
                obj.EmailAddress = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EmailAddress);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EmailAddress);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EmailAddress);
            }
        }
        [TestMethod]
        public void StructuredDataSourceAddressModelTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.AddressModel;
            var value = orgValue;

            value = "fr-FR";
            if(obj.AddressModel != value)
            {
                obj.AddressModel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AddressModel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.AddressModel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AddressModel);
            }
        }
        [TestMethod]
        public void StructuredDataSourceObjectTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Object;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Object != value)
            {
                obj.Object = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Object);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Object);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Object);
            }
        }
        [TestMethod]
        public void StructuredDataSourcePathTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Path;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Path != value)
            {
                obj.Path = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Path);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Path);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Path);
            }
        }
        [TestMethod]
        public void StructuredDataSourcePropertyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Property;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Property != value)
            {
                obj.Property = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Property);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Property);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Property);
            }
        }
        [TestMethod]
        public void StructuredDataSourceResourceTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Resource;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Resource != value)
            {
                obj.Resource = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Resource);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Resource);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Resource);
            }
        }
        [TestMethod]
        public void StructuredDataSourceSchemaTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Schema;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Schema != value)
            {
                obj.Schema = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Schema);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Schema);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Schema);
            }
        }
        [TestMethod]
        public void StructuredDataSourceServerTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Server;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Server != value)
            {
                obj.Server = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Server);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Server);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Server);
            }
        }
        [TestMethod]
        public void StructuredDataSourceUrlTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Url;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Url != value)
            {
                obj.Url = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Url);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Url);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Url);
            }
        }
        [TestMethod]
        public void StructuredDataSourceViewTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.View;
            var value = orgValue;

            value = "fr-FR";
            if(obj.View != value)
            {
                obj.View = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.View);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.View);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.View);
            }
        }
        [TestMethod]
        public void StructuredDataSourceDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void StructuredDataSourceMaxConnectionsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.MaxConnections;
            var value = orgValue;

            value = 123;
            if(obj.MaxConnections != value)
            {
                obj.MaxConnections = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MaxConnections);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.MaxConnections);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MaxConnections);
            }
        }
        [TestMethod]
        public void StructuredDataSourceNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.DataSources["PowerQueryDataSource"] as StructuredDataSource);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class ModelRoleGeneratedTests
	{
        [TestMethod]
        public void ModelRoleDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Roles["Role1"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void ModelRoleModelPermissionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Roles["Role1"];
            var orgValue = obj.ModelPermission;

            if(obj.ModelPermission != ModelPermission.None)
            {
                obj.ModelPermission = ModelPermission.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModelPermission.None, obj.ModelPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);
            }

            if(obj.ModelPermission != ModelPermission.Read)
            {
                obj.ModelPermission = ModelPermission.Read;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModelPermission.Read, obj.ModelPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);
            }

            if(obj.ModelPermission != ModelPermission.ReadRefresh)
            {
                obj.ModelPermission = ModelPermission.ReadRefresh;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModelPermission.ReadRefresh, obj.ModelPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);
            }

            if(obj.ModelPermission != ModelPermission.Refresh)
            {
                obj.ModelPermission = ModelPermission.Refresh;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModelPermission.Refresh, obj.ModelPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);
            }

            if(obj.ModelPermission != ModelPermission.Administrator)
            {
                obj.ModelPermission = ModelPermission.Administrator;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModelPermission.Administrator, obj.ModelPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ModelPermission);
            }
        }    
        [TestMethod]
        public void ModelRoleRoleMembersTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Roles["Role1"];
            var orgValue = obj.RoleMembers;
            var value = orgValue;

            value = "fr-FR";
            if(obj.RoleMembers != value)
            {
                obj.RoleMembers = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RoleMembers);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.RoleMembers);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RoleMembers);
            }
        }
        [TestMethod]
        public void ModelRoleNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Roles["Role1"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class TablePermissionGeneratedTests
	{
        [TestMethod]
        public void TablePermissionFilterExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Roles["Role1"].TablePermissions["CalcTable"];
            var orgValue = obj.FilterExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.FilterExpression != value)
            {
                obj.FilterExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FilterExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FilterExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FilterExpression);
            }
        }
        [TestMethod]
        public void TablePermissionMetadataPermissionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Roles["Role1"].TablePermissions["CalcTable"];
            var orgValue = obj.MetadataPermission;

            if(obj.MetadataPermission != MetadataPermission.None)
            {
                obj.MetadataPermission = MetadataPermission.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.MetadataPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(MetadataPermission.None, obj.MetadataPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.MetadataPermission);
            }

            if(obj.MetadataPermission != MetadataPermission.Read)
            {
                obj.MetadataPermission = MetadataPermission.Read;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.MetadataPermission);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(MetadataPermission.Read, obj.MetadataPermission);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.MetadataPermission);
            }
        }    
    }
	[TestClass]
	public class TableGeneratedTests
	{
        [TestMethod]
        public void TableDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void TableDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void TableIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void TableShowAsVariationsOnlyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.ShowAsVariationsOnly;
            var value = orgValue;

            value = true;
            if(obj.ShowAsVariationsOnly != value)
            {
                obj.ShowAsVariationsOnly = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ShowAsVariationsOnly);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);
            }

            value = false;
            if(obj.ShowAsVariationsOnly != value)
            {
                obj.ShowAsVariationsOnly = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ShowAsVariationsOnly);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);
            }
        }
        [TestMethod]
        public void TableIsPrivateTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.IsPrivate;
            var value = orgValue;

            value = true;
            if(obj.IsPrivate != value)
            {
                obj.IsPrivate = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsPrivate);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);
            }

            value = false;
            if(obj.IsPrivate != value)
            {
                obj.IsPrivate = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsPrivate);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);
            }
        }
        [TestMethod]
        public void TableAlternateSourcePrecedenceTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.AlternateSourcePrecedence;
            var value = orgValue;

            value = 123;
            if(obj.AlternateSourcePrecedence != value)
            {
                obj.AlternateSourcePrecedence = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AlternateSourcePrecedence);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.AlternateSourcePrecedence);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AlternateSourcePrecedence);
            }
        }
        [TestMethod]
        public void TableExcludeFromModelRefreshTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.ExcludeFromModelRefresh;
            var value = orgValue;

            value = true;
            if(obj.ExcludeFromModelRefresh != value)
            {
                obj.ExcludeFromModelRefresh = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);
            }

            value = false;
            if(obj.ExcludeFromModelRefresh != value)
            {
                obj.ExcludeFromModelRefresh = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);
            }
        }
        [TestMethod]
        public void TableLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void TableSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void TableSystemManagedTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.SystemManaged;
            var value = orgValue;

            value = true;
            if(obj.SystemManaged != value)
            {
                obj.SystemManaged = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SystemManaged);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);
            }

            value = false;
            if(obj.SystemManaged != value)
            {
                obj.SystemManaged = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SystemManaged);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);
            }
        }
        [TestMethod]
        public void TableDefaultDetailRowsExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.DefaultDetailRowsExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DefaultDetailRowsExpression != value)
            {
                obj.DefaultDetailRowsExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDetailRowsExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DefaultDetailRowsExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDetailRowsExpression);
            }
        }
        [TestMethod]
        public void TableRollingWindowGranularityTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.RollingWindowGranularity;

            if(obj.RollingWindowGranularity != RefreshGranularityType.Day)
            {
                obj.RollingWindowGranularity = RefreshGranularityType.Day;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Day, obj.RollingWindowGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);
            }

            if(obj.RollingWindowGranularity != RefreshGranularityType.Month)
            {
                obj.RollingWindowGranularity = RefreshGranularityType.Month;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Month, obj.RollingWindowGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);
            }

            if(obj.RollingWindowGranularity != RefreshGranularityType.Quarter)
            {
                obj.RollingWindowGranularity = RefreshGranularityType.Quarter;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Quarter, obj.RollingWindowGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);
            }

            if(obj.RollingWindowGranularity != RefreshGranularityType.Year)
            {
                obj.RollingWindowGranularity = RefreshGranularityType.Year;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Year, obj.RollingWindowGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);
            }

            if(obj.RollingWindowGranularity != RefreshGranularityType.Invalid)
            {
                obj.RollingWindowGranularity = RefreshGranularityType.Invalid;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Invalid, obj.RollingWindowGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowGranularity);
            }
        }    
        [TestMethod]
        public void TableRollingWindowPeriodsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.RollingWindowPeriods;
            var value = orgValue;

            value = 123;
            if(obj.RollingWindowPeriods != value)
            {
                obj.RollingWindowPeriods = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowPeriods);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.RollingWindowPeriods);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RollingWindowPeriods);
            }
        }
        [TestMethod]
        public void TableIncrementalGranularityTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.IncrementalGranularity;

            if(obj.IncrementalGranularity != RefreshGranularityType.Day)
            {
                obj.IncrementalGranularity = RefreshGranularityType.Day;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Day, obj.IncrementalGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);
            }

            if(obj.IncrementalGranularity != RefreshGranularityType.Month)
            {
                obj.IncrementalGranularity = RefreshGranularityType.Month;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Month, obj.IncrementalGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);
            }

            if(obj.IncrementalGranularity != RefreshGranularityType.Quarter)
            {
                obj.IncrementalGranularity = RefreshGranularityType.Quarter;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Quarter, obj.IncrementalGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);
            }

            if(obj.IncrementalGranularity != RefreshGranularityType.Year)
            {
                obj.IncrementalGranularity = RefreshGranularityType.Year;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Year, obj.IncrementalGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);
            }

            if(obj.IncrementalGranularity != RefreshGranularityType.Invalid)
            {
                obj.IncrementalGranularity = RefreshGranularityType.Invalid;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RefreshGranularityType.Invalid, obj.IncrementalGranularity);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalGranularity);
            }
        }    
        [TestMethod]
        public void TableIncrementalPeriodsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.IncrementalPeriods;
            var value = orgValue;

            value = 123;
            if(obj.IncrementalPeriods != value)
            {
                obj.IncrementalPeriods = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalPeriods);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IncrementalPeriods);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalPeriods);
            }
        }
        [TestMethod]
        public void TableIncrementalPeriodsOffsetTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.IncrementalPeriodsOffset;
            var value = orgValue;

            value = 123;
            if(obj.IncrementalPeriodsOffset != value)
            {
                obj.IncrementalPeriodsOffset = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalPeriodsOffset);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IncrementalPeriodsOffset);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IncrementalPeriodsOffset);
            }
        }
        [TestMethod]
        public void TablePollingExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.PollingExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.PollingExpression != value)
            {
                obj.PollingExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.PollingExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.PollingExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.PollingExpression);
            }
        }
        [TestMethod]
        public void TableSourceExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.SourceExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceExpression != value)
            {
                obj.SourceExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceExpression);
            }
        }
        [TestMethod]
        public void TableEnableRefreshPolicyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.EnableRefreshPolicy;
            var value = orgValue;

            value = true;
            if(obj.EnableRefreshPolicy != value)
            {
                obj.EnableRefreshPolicy = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EnableRefreshPolicy);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);
            }

            value = false;
            if(obj.EnableRefreshPolicy != value)
            {
                obj.EnableRefreshPolicy = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EnableRefreshPolicy);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);
            }
        }
        [TestMethod]
        public void TableNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"] as Table);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class PartitionGeneratedTests
	{
        [TestMethod]
        public void PartitionDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New Table"] as Partition);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void PartitionModeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New Table"] as Partition);
            var orgValue = obj.Mode;

            if(obj.Mode != ModeType.Import)
            {
                obj.Mode = ModeType.Import;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModeType.Import, obj.Mode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);
            }

            if(obj.Mode != ModeType.DirectQuery)
            {
                obj.Mode = ModeType.DirectQuery;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModeType.DirectQuery, obj.Mode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);
            }
        }    
        [TestMethod]
        public void PartitionDataViewTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New Table"] as Partition);
            var orgValue = obj.DataView;

            if(obj.DataView != DataViewType.Full)
            {
                obj.DataView = DataViewType.Full;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataViewType.Full, obj.DataView);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);
            }

            if(obj.DataView != DataViewType.Sample)
            {
                obj.DataView = DataViewType.Sample;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataViewType.Sample, obj.DataView);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);
            }
        }    
        [TestMethod]
        public void PartitionQueryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New Table"] as Partition);
            var orgValue = obj.Query;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Query != value)
            {
                obj.Query = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Query);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Query);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Query);
            }
        }
        [TestMethod]
        public void PartitionExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New Table"] as Partition);
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void PartitionNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New Table"] as Partition);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class MPartitionGeneratedTests
	{
        [TestMethod]
        public void MPartitionMExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.MExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.MExpression != value)
            {
                obj.MExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.MExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MExpression);
            }
        }
        [TestMethod]
        public void MPartitionDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void MPartitionModeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.Mode;

            if(obj.Mode != ModeType.Import)
            {
                obj.Mode = ModeType.Import;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModeType.Import, obj.Mode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);
            }

            if(obj.Mode != ModeType.DirectQuery)
            {
                obj.Mode = ModeType.DirectQuery;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ModeType.DirectQuery, obj.Mode);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Mode);
            }
        }    
        [TestMethod]
        public void MPartitionDataViewTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.DataView;

            if(obj.DataView != DataViewType.Full)
            {
                obj.DataView = DataViewType.Full;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataViewType.Full, obj.DataView);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);
            }

            if(obj.DataView != DataViewType.Sample)
            {
                obj.DataView = DataViewType.Sample;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataViewType.Sample, obj.DataView);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataView);
            }
        }    
        [TestMethod]
        public void MPartitionQueryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.Query;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Query != value)
            {
                obj.Query = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Query);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Query);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Query);
            }
        }
        [TestMethod]
        public void MPartitionExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void MPartitionNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Partitions["New M Partition"] as MPartition);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class DataColumnGeneratedTests
	{
        [TestMethod]
        public void DataColumnSourceColumnTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.SourceColumn;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceColumn != value)
            {
                obj.SourceColumn = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceColumn);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceColumn);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceColumn);
            }
        }
        [TestMethod]
        public void DataColumnDataTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.DataType;

            if(obj.DataType != DataType.String)
            {
                obj.DataType = DataType.String;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.String, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Int64)
            {
                obj.DataType = DataType.Int64;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Int64, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Double)
            {
                obj.DataType = DataType.Double;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Double, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.DateTime)
            {
                obj.DataType = DataType.DateTime;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.DateTime, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Decimal)
            {
                obj.DataType = DataType.Decimal;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Decimal, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Boolean)
            {
                obj.DataType = DataType.Boolean;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Boolean, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Binary)
            {
                obj.DataType = DataType.Binary;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Binary, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Variant)
            {
                obj.DataType = DataType.Variant;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Variant, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }
        }    
        [TestMethod]
        public void DataColumnDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void DataColumnDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void DataColumnIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void DataColumnIsUniqueTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsUnique;
            var value = orgValue;

            value = true;
            if(obj.IsUnique != value)
            {
                obj.IsUnique = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsUnique);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);
            }

            value = false;
            if(obj.IsUnique != value)
            {
                obj.IsUnique = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsUnique);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);
            }
        }
        [TestMethod]
        public void DataColumnIsKeyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsKey;
            var value = orgValue;

            value = true;
            if(obj.IsKey != value)
            {
                obj.IsKey = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsKey);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);
            }

            value = false;
            if(obj.IsKey != value)
            {
                obj.IsKey = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsKey);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);
            }
        }
        [TestMethod]
        public void DataColumnIsNullableTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsNullable;
            var value = orgValue;

            value = true;
            if(obj.IsNullable != value)
            {
                obj.IsNullable = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNullable);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);
            }

            value = false;
            if(obj.IsNullable != value)
            {
                obj.IsNullable = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNullable);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);
            }
        }
        [TestMethod]
        public void DataColumnAlignmentTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.Alignment;

            if(obj.Alignment != Alignment.Left)
            {
                obj.Alignment = Alignment.Left;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Left, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }

            if(obj.Alignment != Alignment.Right)
            {
                obj.Alignment = Alignment.Right;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Right, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }

            if(obj.Alignment != Alignment.Center)
            {
                obj.Alignment = Alignment.Center;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Center, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }
        }    
        [TestMethod]
        public void DataColumnTableDetailPositionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.TableDetailPosition;
            var value = orgValue;

            value = 123;
            if(obj.TableDetailPosition != value)
            {
                obj.TableDetailPosition = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TableDetailPosition);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TableDetailPosition);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TableDetailPosition);
            }
        }
        [TestMethod]
        public void DataColumnIsDefaultLabelTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsDefaultLabel;
            var value = orgValue;

            value = true;
            if(obj.IsDefaultLabel != value)
            {
                obj.IsDefaultLabel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultLabel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);
            }

            value = false;
            if(obj.IsDefaultLabel != value)
            {
                obj.IsDefaultLabel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultLabel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);
            }
        }
        [TestMethod]
        public void DataColumnIsDefaultImageTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsDefaultImage;
            var value = orgValue;

            value = true;
            if(obj.IsDefaultImage != value)
            {
                obj.IsDefaultImage = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultImage);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);
            }

            value = false;
            if(obj.IsDefaultImage != value)
            {
                obj.IsDefaultImage = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultImage);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);
            }
        }
        [TestMethod]
        public void DataColumnSummarizeByTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.SummarizeBy;

            if(obj.SummarizeBy != AggregateFunction.None)
            {
                obj.SummarizeBy = AggregateFunction.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.None, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Sum)
            {
                obj.SummarizeBy = AggregateFunction.Sum;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Sum, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Min)
            {
                obj.SummarizeBy = AggregateFunction.Min;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Min, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Max)
            {
                obj.SummarizeBy = AggregateFunction.Max;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Max, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Count)
            {
                obj.SummarizeBy = AggregateFunction.Count;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Count, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Average)
            {
                obj.SummarizeBy = AggregateFunction.Average;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Average, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.DistinctCount)
            {
                obj.SummarizeBy = AggregateFunction.DistinctCount;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.DistinctCount, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }
        }    
        [TestMethod]
        public void DataColumnFormatStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.FormatString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.FormatString != value)
            {
                obj.FormatString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FormatString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);
            }
        }
        [TestMethod]
        public void DataColumnIsAvailableInMDXTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.IsAvailableInMDX;
            var value = orgValue;

            value = true;
            if(obj.IsAvailableInMDX != value)
            {
                obj.IsAvailableInMDX = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsAvailableInMDX);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);
            }

            value = false;
            if(obj.IsAvailableInMDX != value)
            {
                obj.IsAvailableInMDX = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsAvailableInMDX);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);
            }
        }
        [TestMethod]
        public void DataColumnKeepUniqueRowsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.KeepUniqueRows;
            var value = orgValue;

            value = true;
            if(obj.KeepUniqueRows != value)
            {
                obj.KeepUniqueRows = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.KeepUniqueRows);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);
            }

            value = false;
            if(obj.KeepUniqueRows != value)
            {
                obj.KeepUniqueRows = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.KeepUniqueRows);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);
            }
        }
        [TestMethod]
        public void DataColumnDisplayOrdinalTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.DisplayOrdinal;
            var value = orgValue;

            value = 123;
            if(obj.DisplayOrdinal != value)
            {
                obj.DisplayOrdinal = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayOrdinal);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayOrdinal);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayOrdinal);
            }
        }
        [TestMethod]
        public void DataColumnSourceProviderTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.SourceProviderType;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceProviderType != value)
            {
                obj.SourceProviderType = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceProviderType);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceProviderType);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceProviderType);
            }
        }
        [TestMethod]
        public void DataColumnDisplayFolderTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.DisplayFolder;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DisplayFolder != value)
            {
                obj.DisplayFolder = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayFolder);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);
            }
        }
        [TestMethod]
        public void DataColumnEncodingHintTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.EncodingHint;

            if(obj.EncodingHint != EncodingHintType.Hash)
            {
                obj.EncodingHint = EncodingHintType.Hash;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(EncodingHintType.Hash, obj.EncodingHint);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);
            }

            if(obj.EncodingHint != EncodingHintType.Value)
            {
                obj.EncodingHint = EncodingHintType.Value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(EncodingHintType.Value, obj.EncodingHint);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);
            }
        }    
        [TestMethod]
        public void DataColumnLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void DataColumnSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void DataColumnNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"].Columns["Ordinal"] as DataColumn);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class AlternateOfGeneratedTests
	{
        [TestMethod]
        public void AlternateOfSummarizationTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Columns["DC1"].AlternateOf;
            var orgValue = obj.Summarization;

            if(obj.Summarization != SummarizationType.GroupBy)
            {
                obj.Summarization = SummarizationType.GroupBy;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SummarizationType.GroupBy, obj.Summarization);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);
            }

            if(obj.Summarization != SummarizationType.Sum)
            {
                obj.Summarization = SummarizationType.Sum;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SummarizationType.Sum, obj.Summarization);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);
            }

            if(obj.Summarization != SummarizationType.Count)
            {
                obj.Summarization = SummarizationType.Count;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SummarizationType.Count, obj.Summarization);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);
            }

            if(obj.Summarization != SummarizationType.Min)
            {
                obj.Summarization = SummarizationType.Min;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SummarizationType.Min, obj.Summarization);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);
            }

            if(obj.Summarization != SummarizationType.Max)
            {
                obj.Summarization = SummarizationType.Max;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SummarizationType.Max, obj.Summarization);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Summarization);
            }
        }    
    }
	[TestClass]
	public class CalculatedColumnGeneratedTests
	{
        [TestMethod]
        public void CalculatedColumnIsDataTypeInferredTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsDataTypeInferred;
            var value = orgValue;

            value = true;
            if(obj.IsDataTypeInferred != value)
            {
                obj.IsDataTypeInferred = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDataTypeInferred);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);
            }

            value = false;
            if(obj.IsDataTypeInferred != value)
            {
                obj.IsDataTypeInferred = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDataTypeInferred);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);
            }
        }
        [TestMethod]
        public void CalculatedColumnExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void CalculatedColumnDataTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.DataType;

            if(obj.DataType != DataType.String)
            {
                obj.DataType = DataType.String;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.String, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Int64)
            {
                obj.DataType = DataType.Int64;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Int64, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Double)
            {
                obj.DataType = DataType.Double;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Double, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.DateTime)
            {
                obj.DataType = DataType.DateTime;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.DateTime, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Decimal)
            {
                obj.DataType = DataType.Decimal;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Decimal, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Boolean)
            {
                obj.DataType = DataType.Boolean;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Boolean, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Binary)
            {
                obj.DataType = DataType.Binary;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Binary, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Variant)
            {
                obj.DataType = DataType.Variant;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Variant, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }
        }    
        [TestMethod]
        public void CalculatedColumnDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void CalculatedColumnDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsUniqueTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsUnique;
            var value = orgValue;

            value = true;
            if(obj.IsUnique != value)
            {
                obj.IsUnique = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsUnique);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);
            }

            value = false;
            if(obj.IsUnique != value)
            {
                obj.IsUnique = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsUnique);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsKeyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsKey;
            var value = orgValue;

            value = true;
            if(obj.IsKey != value)
            {
                obj.IsKey = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsKey);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);
            }

            value = false;
            if(obj.IsKey != value)
            {
                obj.IsKey = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsKey);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsNullableTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsNullable;
            var value = orgValue;

            value = true;
            if(obj.IsNullable != value)
            {
                obj.IsNullable = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNullable);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);
            }

            value = false;
            if(obj.IsNullable != value)
            {
                obj.IsNullable = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNullable);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);
            }
        }
        [TestMethod]
        public void CalculatedColumnAlignmentTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.Alignment;

            if(obj.Alignment != Alignment.Left)
            {
                obj.Alignment = Alignment.Left;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Left, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }

            if(obj.Alignment != Alignment.Right)
            {
                obj.Alignment = Alignment.Right;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Right, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }

            if(obj.Alignment != Alignment.Center)
            {
                obj.Alignment = Alignment.Center;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Center, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }
        }    
        [TestMethod]
        public void CalculatedColumnTableDetailPositionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.TableDetailPosition;
            var value = orgValue;

            value = 123;
            if(obj.TableDetailPosition != value)
            {
                obj.TableDetailPosition = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TableDetailPosition);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TableDetailPosition);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TableDetailPosition);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsDefaultLabelTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsDefaultLabel;
            var value = orgValue;

            value = true;
            if(obj.IsDefaultLabel != value)
            {
                obj.IsDefaultLabel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultLabel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);
            }

            value = false;
            if(obj.IsDefaultLabel != value)
            {
                obj.IsDefaultLabel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultLabel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsDefaultImageTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsDefaultImage;
            var value = orgValue;

            value = true;
            if(obj.IsDefaultImage != value)
            {
                obj.IsDefaultImage = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultImage);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);
            }

            value = false;
            if(obj.IsDefaultImage != value)
            {
                obj.IsDefaultImage = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultImage);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);
            }
        }
        [TestMethod]
        public void CalculatedColumnSummarizeByTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.SummarizeBy;

            if(obj.SummarizeBy != AggregateFunction.None)
            {
                obj.SummarizeBy = AggregateFunction.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.None, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Sum)
            {
                obj.SummarizeBy = AggregateFunction.Sum;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Sum, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Min)
            {
                obj.SummarizeBy = AggregateFunction.Min;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Min, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Max)
            {
                obj.SummarizeBy = AggregateFunction.Max;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Max, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Count)
            {
                obj.SummarizeBy = AggregateFunction.Count;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Count, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Average)
            {
                obj.SummarizeBy = AggregateFunction.Average;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Average, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.DistinctCount)
            {
                obj.SummarizeBy = AggregateFunction.DistinctCount;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.DistinctCount, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }
        }    
        [TestMethod]
        public void CalculatedColumnFormatStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.FormatString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.FormatString != value)
            {
                obj.FormatString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FormatString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);
            }
        }
        [TestMethod]
        public void CalculatedColumnIsAvailableInMDXTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.IsAvailableInMDX;
            var value = orgValue;

            value = true;
            if(obj.IsAvailableInMDX != value)
            {
                obj.IsAvailableInMDX = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsAvailableInMDX);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);
            }

            value = false;
            if(obj.IsAvailableInMDX != value)
            {
                obj.IsAvailableInMDX = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsAvailableInMDX);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);
            }
        }
        [TestMethod]
        public void CalculatedColumnKeepUniqueRowsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.KeepUniqueRows;
            var value = orgValue;

            value = true;
            if(obj.KeepUniqueRows != value)
            {
                obj.KeepUniqueRows = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.KeepUniqueRows);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);
            }

            value = false;
            if(obj.KeepUniqueRows != value)
            {
                obj.KeepUniqueRows = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.KeepUniqueRows);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);
            }
        }
        [TestMethod]
        public void CalculatedColumnDisplayOrdinalTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.DisplayOrdinal;
            var value = orgValue;

            value = 123;
            if(obj.DisplayOrdinal != value)
            {
                obj.DisplayOrdinal = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayOrdinal);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayOrdinal);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayOrdinal);
            }
        }
        [TestMethod]
        public void CalculatedColumnSourceProviderTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.SourceProviderType;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceProviderType != value)
            {
                obj.SourceProviderType = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceProviderType);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceProviderType);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceProviderType);
            }
        }
        [TestMethod]
        public void CalculatedColumnDisplayFolderTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.DisplayFolder;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DisplayFolder != value)
            {
                obj.DisplayFolder = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayFolder);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);
            }
        }
        [TestMethod]
        public void CalculatedColumnEncodingHintTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.EncodingHint;

            if(obj.EncodingHint != EncodingHintType.Hash)
            {
                obj.EncodingHint = EncodingHintType.Hash;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(EncodingHintType.Hash, obj.EncodingHint);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);
            }

            if(obj.EncodingHint != EncodingHintType.Value)
            {
                obj.EncodingHint = EncodingHintType.Value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(EncodingHintType.Value, obj.EncodingHint);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);
            }
        }    
        [TestMethod]
        public void CalculatedColumnLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void CalculatedColumnSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void CalculatedColumnNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["Table"].Columns["CC1"] as CalculatedColumn);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class VariationGeneratedTests
	{
        [TestMethod]
        public void VariationDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Columns["CC1"].Variations["Variation1"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void VariationIsDefaultTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Columns["CC1"].Variations["Variation1"];
            var orgValue = obj.IsDefault;
            var value = orgValue;

            value = true;
            if(obj.IsDefault != value)
            {
                obj.IsDefault = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefault);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefault);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefault);
            }

            value = false;
            if(obj.IsDefault != value)
            {
                obj.IsDefault = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefault);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefault);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefault);
            }
        }
        [TestMethod]
        public void VariationNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Columns["CC1"].Variations["Variation1"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class HierarchyGeneratedTests
	{
        [TestMethod]
        public void HierarchyDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void HierarchyIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void HierarchyDisplayFolderTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.DisplayFolder;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DisplayFolder != value)
            {
                obj.DisplayFolder = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayFolder);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);
            }
        }
        [TestMethod]
        public void HierarchyHideMembersTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.HideMembers;

            if(obj.HideMembers != HierarchyHideMembersType.HideBlankMembers)
            {
                obj.HideMembers = HierarchyHideMembersType.HideBlankMembers;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.HideMembers);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(HierarchyHideMembersType.HideBlankMembers, obj.HideMembers);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.HideMembers);
            }
        }    
        [TestMethod]
        public void HierarchyLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void HierarchySourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void HierarchyNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class LevelGeneratedTests
	{
        [TestMethod]
        public void LevelDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"].Levels["L1"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void LevelLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"].Levels["L1"];
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void LevelSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"].Levels["L1"];
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void LevelNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Hierarchies["H1"].Levels["L1"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class MeasureGeneratedTests
	{
        [TestMethod]
        public void MeasureDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void MeasureExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void MeasureFormatStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.FormatString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.FormatString != value)
            {
                obj.FormatString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FormatString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);
            }
        }
        [TestMethod]
        public void MeasureIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void MeasureIsSimpleMeasureTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.IsSimpleMeasure;
            var value = orgValue;

            value = true;
            if(obj.IsSimpleMeasure != value)
            {
                obj.IsSimpleMeasure = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsSimpleMeasure);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsSimpleMeasure);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsSimpleMeasure);
            }

            value = false;
            if(obj.IsSimpleMeasure != value)
            {
                obj.IsSimpleMeasure = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsSimpleMeasure);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsSimpleMeasure);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsSimpleMeasure);
            }
        }
        [TestMethod]
        public void MeasureDisplayFolderTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.DisplayFolder;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DisplayFolder != value)
            {
                obj.DisplayFolder = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayFolder);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);
            }
        }
        [TestMethod]
        public void MeasureDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void MeasureLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void MeasureSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void MeasureDetailRowsExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.DetailRowsExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DetailRowsExpression != value)
            {
                obj.DetailRowsExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DetailRowsExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DetailRowsExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DetailRowsExpression);
            }
        }
        [TestMethod]
        public void MeasureNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class KPIGeneratedTests
	{
        [TestMethod]
        public void KPIDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void KPITargetDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.TargetDescription;
            var value = orgValue;

            value = "fr-FR";
            if(obj.TargetDescription != value)
            {
                obj.TargetDescription = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TargetDescription);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TargetDescription);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TargetDescription);
            }
        }
        [TestMethod]
        public void KPITargetExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.TargetExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.TargetExpression != value)
            {
                obj.TargetExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TargetExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TargetExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TargetExpression);
            }
        }
        [TestMethod]
        public void KPITargetFormatStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.TargetFormatString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.TargetFormatString != value)
            {
                obj.TargetFormatString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TargetFormatString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TargetFormatString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TargetFormatString);
            }
        }
        [TestMethod]
        public void KPIStatusGraphicTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.StatusGraphic;
            var value = orgValue;

            value = "fr-FR";
            if(obj.StatusGraphic != value)
            {
                obj.StatusGraphic = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StatusGraphic);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.StatusGraphic);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StatusGraphic);
            }
        }
        [TestMethod]
        public void KPIStatusDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.StatusDescription;
            var value = orgValue;

            value = "fr-FR";
            if(obj.StatusDescription != value)
            {
                obj.StatusDescription = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StatusDescription);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.StatusDescription);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StatusDescription);
            }
        }
        [TestMethod]
        public void KPIStatusExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.StatusExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.StatusExpression != value)
            {
                obj.StatusExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StatusExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.StatusExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.StatusExpression);
            }
        }
        [TestMethod]
        public void KPITrendGraphicTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.TrendGraphic;
            var value = orgValue;

            value = "fr-FR";
            if(obj.TrendGraphic != value)
            {
                obj.TrendGraphic = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TrendGraphic);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TrendGraphic);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TrendGraphic);
            }
        }
        [TestMethod]
        public void KPITrendDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.TrendDescription;
            var value = orgValue;

            value = "fr-FR";
            if(obj.TrendDescription != value)
            {
                obj.TrendDescription = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TrendDescription);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TrendDescription);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TrendDescription);
            }
        }
        [TestMethod]
        public void KPITrendExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.TrendExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.TrendExpression != value)
            {
                obj.TrendExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TrendExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TrendExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TrendExpression);
            }
        }
        [TestMethod]
        public void KPIExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Tables["Table"].Measures["Measure"].KPI;
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
    }
	[TestClass]
	public class CalculatedTableGeneratedTests
	{
        [TestMethod]
        public void CalculatedTableExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void CalculatedTableDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void CalculatedTableDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void CalculatedTableIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void CalculatedTableShowAsVariationsOnlyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.ShowAsVariationsOnly;
            var value = orgValue;

            value = true;
            if(obj.ShowAsVariationsOnly != value)
            {
                obj.ShowAsVariationsOnly = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ShowAsVariationsOnly);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);
            }

            value = false;
            if(obj.ShowAsVariationsOnly != value)
            {
                obj.ShowAsVariationsOnly = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ShowAsVariationsOnly);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);
            }
        }
        [TestMethod]
        public void CalculatedTableIsPrivateTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.IsPrivate;
            var value = orgValue;

            value = true;
            if(obj.IsPrivate != value)
            {
                obj.IsPrivate = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsPrivate);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);
            }

            value = false;
            if(obj.IsPrivate != value)
            {
                obj.IsPrivate = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsPrivate);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);
            }
        }
        [TestMethod]
        public void CalculatedTableAlternateSourcePrecedenceTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.AlternateSourcePrecedence;
            var value = orgValue;

            value = 123;
            if(obj.AlternateSourcePrecedence != value)
            {
                obj.AlternateSourcePrecedence = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AlternateSourcePrecedence);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.AlternateSourcePrecedence);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AlternateSourcePrecedence);
            }
        }
        [TestMethod]
        public void CalculatedTableExcludeFromModelRefreshTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.ExcludeFromModelRefresh;
            var value = orgValue;

            value = true;
            if(obj.ExcludeFromModelRefresh != value)
            {
                obj.ExcludeFromModelRefresh = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);
            }

            value = false;
            if(obj.ExcludeFromModelRefresh != value)
            {
                obj.ExcludeFromModelRefresh = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);
            }
        }
        [TestMethod]
        public void CalculatedTableLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void CalculatedTableSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void CalculatedTableSystemManagedTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.SystemManaged;
            var value = orgValue;

            value = true;
            if(obj.SystemManaged != value)
            {
                obj.SystemManaged = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SystemManaged);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);
            }

            value = false;
            if(obj.SystemManaged != value)
            {
                obj.SystemManaged = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SystemManaged);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);
            }
        }
        [TestMethod]
        public void CalculatedTableDefaultDetailRowsExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.DefaultDetailRowsExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DefaultDetailRowsExpression != value)
            {
                obj.DefaultDetailRowsExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDetailRowsExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DefaultDetailRowsExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDetailRowsExpression);
            }
        }
        [TestMethod]
        public void CalculatedTableEnableRefreshPolicyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.EnableRefreshPolicy;
            var value = orgValue;

            value = true;
            if(obj.EnableRefreshPolicy != value)
            {
                obj.EnableRefreshPolicy = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EnableRefreshPolicy);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);
            }

            value = false;
            if(obj.EnableRefreshPolicy != value)
            {
                obj.EnableRefreshPolicy = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EnableRefreshPolicy);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);
            }
        }
        [TestMethod]
        public void CalculatedTableNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"] as CalculatedTable);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class CalculatedTableColumnGeneratedTests
	{
        [TestMethod]
        public void CalculatedTableColumnIsNameInferredTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsNameInferred;
            var value = orgValue;

            value = true;
            if(obj.IsNameInferred != value)
            {
                obj.IsNameInferred = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNameInferred);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNameInferred);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNameInferred);
            }

            value = false;
            if(obj.IsNameInferred != value)
            {
                obj.IsNameInferred = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNameInferred);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNameInferred);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNameInferred);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsDataTypeInferredTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsDataTypeInferred;
            var value = orgValue;

            value = true;
            if(obj.IsDataTypeInferred != value)
            {
                obj.IsDataTypeInferred = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDataTypeInferred);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);
            }

            value = false;
            if(obj.IsDataTypeInferred != value)
            {
                obj.IsDataTypeInferred = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDataTypeInferred);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDataTypeInferred);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnSourceColumnTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.SourceColumn;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceColumn != value)
            {
                obj.SourceColumn = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceColumn);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceColumn);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceColumn);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnDataTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.DataType;

            if(obj.DataType != DataType.String)
            {
                obj.DataType = DataType.String;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.String, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Int64)
            {
                obj.DataType = DataType.Int64;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Int64, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Double)
            {
                obj.DataType = DataType.Double;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Double, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.DateTime)
            {
                obj.DataType = DataType.DateTime;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.DateTime, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Decimal)
            {
                obj.DataType = DataType.Decimal;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Decimal, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Boolean)
            {
                obj.DataType = DataType.Boolean;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Boolean, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Binary)
            {
                obj.DataType = DataType.Binary;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Binary, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }

            if(obj.DataType != DataType.Variant)
            {
                obj.DataType = DataType.Variant;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DataType.Variant, obj.DataType);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.DataType);
            }
        }    
        [TestMethod]
        public void CalculatedTableColumnDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsUniqueTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsUnique;
            var value = orgValue;

            value = true;
            if(obj.IsUnique != value)
            {
                obj.IsUnique = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsUnique);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);
            }

            value = false;
            if(obj.IsUnique != value)
            {
                obj.IsUnique = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsUnique);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsUnique);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsKeyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsKey;
            var value = orgValue;

            value = true;
            if(obj.IsKey != value)
            {
                obj.IsKey = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsKey);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);
            }

            value = false;
            if(obj.IsKey != value)
            {
                obj.IsKey = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsKey);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsKey);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsNullableTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsNullable;
            var value = orgValue;

            value = true;
            if(obj.IsNullable != value)
            {
                obj.IsNullable = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNullable);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);
            }

            value = false;
            if(obj.IsNullable != value)
            {
                obj.IsNullable = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsNullable);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsNullable);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnAlignmentTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.Alignment;

            if(obj.Alignment != Alignment.Left)
            {
                obj.Alignment = Alignment.Left;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Left, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }

            if(obj.Alignment != Alignment.Right)
            {
                obj.Alignment = Alignment.Right;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Right, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }

            if(obj.Alignment != Alignment.Center)
            {
                obj.Alignment = Alignment.Center;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(Alignment.Center, obj.Alignment);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Alignment);
            }
        }    
        [TestMethod]
        public void CalculatedTableColumnTableDetailPositionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.TableDetailPosition;
            var value = orgValue;

            value = 123;
            if(obj.TableDetailPosition != value)
            {
                obj.TableDetailPosition = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TableDetailPosition);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.TableDetailPosition);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.TableDetailPosition);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsDefaultLabelTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsDefaultLabel;
            var value = orgValue;

            value = true;
            if(obj.IsDefaultLabel != value)
            {
                obj.IsDefaultLabel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultLabel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);
            }

            value = false;
            if(obj.IsDefaultLabel != value)
            {
                obj.IsDefaultLabel = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultLabel);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultLabel);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsDefaultImageTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsDefaultImage;
            var value = orgValue;

            value = true;
            if(obj.IsDefaultImage != value)
            {
                obj.IsDefaultImage = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultImage);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);
            }

            value = false;
            if(obj.IsDefaultImage != value)
            {
                obj.IsDefaultImage = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsDefaultImage);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsDefaultImage);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnSummarizeByTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.SummarizeBy;

            if(obj.SummarizeBy != AggregateFunction.None)
            {
                obj.SummarizeBy = AggregateFunction.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.None, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Sum)
            {
                obj.SummarizeBy = AggregateFunction.Sum;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Sum, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Min)
            {
                obj.SummarizeBy = AggregateFunction.Min;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Min, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Max)
            {
                obj.SummarizeBy = AggregateFunction.Max;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Max, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Count)
            {
                obj.SummarizeBy = AggregateFunction.Count;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Count, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.Average)
            {
                obj.SummarizeBy = AggregateFunction.Average;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.Average, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }

            if(obj.SummarizeBy != AggregateFunction.DistinctCount)
            {
                obj.SummarizeBy = AggregateFunction.DistinctCount;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(AggregateFunction.DistinctCount, obj.SummarizeBy);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SummarizeBy);
            }
        }    
        [TestMethod]
        public void CalculatedTableColumnFormatStringTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.FormatString;
            var value = orgValue;

            value = "fr-FR";
            if(obj.FormatString != value)
            {
                obj.FormatString = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FormatString);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatString);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnIsAvailableInMDXTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.IsAvailableInMDX;
            var value = orgValue;

            value = true;
            if(obj.IsAvailableInMDX != value)
            {
                obj.IsAvailableInMDX = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsAvailableInMDX);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);
            }

            value = false;
            if(obj.IsAvailableInMDX != value)
            {
                obj.IsAvailableInMDX = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsAvailableInMDX);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsAvailableInMDX);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnKeepUniqueRowsTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.KeepUniqueRows;
            var value = orgValue;

            value = true;
            if(obj.KeepUniqueRows != value)
            {
                obj.KeepUniqueRows = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.KeepUniqueRows);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);
            }

            value = false;
            if(obj.KeepUniqueRows != value)
            {
                obj.KeepUniqueRows = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.KeepUniqueRows);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.KeepUniqueRows);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnDisplayOrdinalTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.DisplayOrdinal;
            var value = orgValue;

            value = 123;
            if(obj.DisplayOrdinal != value)
            {
                obj.DisplayOrdinal = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayOrdinal);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayOrdinal);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayOrdinal);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnSourceProviderTypeTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.SourceProviderType;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceProviderType != value)
            {
                obj.SourceProviderType = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceProviderType);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceProviderType);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceProviderType);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnDisplayFolderTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.DisplayFolder;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DisplayFolder != value)
            {
                obj.DisplayFolder = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DisplayFolder);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DisplayFolder);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnEncodingHintTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.EncodingHint;

            if(obj.EncodingHint != EncodingHintType.Hash)
            {
                obj.EncodingHint = EncodingHintType.Hash;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(EncodingHintType.Hash, obj.EncodingHint);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);
            }

            if(obj.EncodingHint != EncodingHintType.Value)
            {
                obj.EncodingHint = EncodingHintType.Value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(EncodingHintType.Value, obj.EncodingHint);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.EncodingHint);
            }
        }    
        [TestMethod]
        public void CalculatedTableColumnLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void CalculatedTableColumnNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalcTable"].Columns["Value"] as CalculatedTableColumn);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class CalculationGroupTableGeneratedTests
	{
        [TestMethod]
        public void CalculationGroupTableCalculationGroupPrecedenceTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.CalculationGroupPrecedence;
            var value = orgValue;

            value = 123;
            if(obj.CalculationGroupPrecedence != value)
            {
                obj.CalculationGroupPrecedence = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.CalculationGroupPrecedence);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.CalculationGroupPrecedence);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.CalculationGroupPrecedence);
            }
        }
        [TestMethod]
        public void CalculationGroupTableCalculationGroupDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.CalculationGroupDescription;
            var value = orgValue;

            value = "fr-FR";
            if(obj.CalculationGroupDescription != value)
            {
                obj.CalculationGroupDescription = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.CalculationGroupDescription);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.CalculationGroupDescription);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.CalculationGroupDescription);
            }
        }
        [TestMethod]
        public void CalculationGroupTableDataCategoryTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.DataCategory;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DataCategory != value)
            {
                obj.DataCategory = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DataCategory);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DataCategory);
            }
        }
        [TestMethod]
        public void CalculationGroupTableDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void CalculationGroupTableIsHiddenTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.IsHidden;
            var value = orgValue;

            value = true;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }

            value = false;
            if(obj.IsHidden != value)
            {
                obj.IsHidden = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsHidden);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsHidden);
            }
        }
        [TestMethod]
        public void CalculationGroupTableShowAsVariationsOnlyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.ShowAsVariationsOnly;
            var value = orgValue;

            value = true;
            if(obj.ShowAsVariationsOnly != value)
            {
                obj.ShowAsVariationsOnly = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ShowAsVariationsOnly);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);
            }

            value = false;
            if(obj.ShowAsVariationsOnly != value)
            {
                obj.ShowAsVariationsOnly = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ShowAsVariationsOnly);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ShowAsVariationsOnly);
            }
        }
        [TestMethod]
        public void CalculationGroupTableIsPrivateTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.IsPrivate;
            var value = orgValue;

            value = true;
            if(obj.IsPrivate != value)
            {
                obj.IsPrivate = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsPrivate);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);
            }

            value = false;
            if(obj.IsPrivate != value)
            {
                obj.IsPrivate = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsPrivate);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsPrivate);
            }
        }
        [TestMethod]
        public void CalculationGroupTableAlternateSourcePrecedenceTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.AlternateSourcePrecedence;
            var value = orgValue;

            value = 123;
            if(obj.AlternateSourcePrecedence != value)
            {
                obj.AlternateSourcePrecedence = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AlternateSourcePrecedence);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.AlternateSourcePrecedence);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.AlternateSourcePrecedence);
            }
        }
        [TestMethod]
        public void CalculationGroupTableExcludeFromModelRefreshTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.ExcludeFromModelRefresh;
            var value = orgValue;

            value = true;
            if(obj.ExcludeFromModelRefresh != value)
            {
                obj.ExcludeFromModelRefresh = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);
            }

            value = false;
            if(obj.ExcludeFromModelRefresh != value)
            {
                obj.ExcludeFromModelRefresh = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.ExcludeFromModelRefresh);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.ExcludeFromModelRefresh);
            }
        }
        [TestMethod]
        public void CalculationGroupTableLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void CalculationGroupTableSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void CalculationGroupTableSystemManagedTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.SystemManaged;
            var value = orgValue;

            value = true;
            if(obj.SystemManaged != value)
            {
                obj.SystemManaged = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SystemManaged);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);
            }

            value = false;
            if(obj.SystemManaged != value)
            {
                obj.SystemManaged = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SystemManaged);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SystemManaged);
            }
        }
        [TestMethod]
        public void CalculationGroupTableDefaultDetailRowsExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.DefaultDetailRowsExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.DefaultDetailRowsExpression != value)
            {
                obj.DefaultDetailRowsExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDetailRowsExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.DefaultDetailRowsExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.DefaultDetailRowsExpression);
            }
        }
        [TestMethod]
        public void CalculationGroupTableEnableRefreshPolicyTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.EnableRefreshPolicy;
            var value = orgValue;

            value = true;
            if(obj.EnableRefreshPolicy != value)
            {
                obj.EnableRefreshPolicy = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EnableRefreshPolicy);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);
            }

            value = false;
            if(obj.EnableRefreshPolicy != value)
            {
                obj.EnableRefreshPolicy = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.EnableRefreshPolicy);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.EnableRefreshPolicy);
            }
        }
        [TestMethod]
        public void CalculationGroupTableNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable);
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class CalculationGroupGeneratedTests
	{
        [TestMethod]
        public void CalculationGroupDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable).CalculationGroup;
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void CalculationGroupPrecedenceTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable).CalculationGroup;
            var orgValue = obj.Precedence;
            var value = orgValue;

            value = 123;
            if(obj.Precedence != value)
            {
                obj.Precedence = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Precedence);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Precedence);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Precedence);
            }
        }
    }
	[TestClass]
	public class CalculationItemGeneratedTests
	{
        [TestMethod]
        public void CalculationItemFormatStringExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable).CalculationItems["CalcItem"];
            var orgValue = obj.FormatStringExpression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.FormatStringExpression != value)
            {
                obj.FormatStringExpression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatStringExpression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.FormatStringExpression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.FormatStringExpression);
            }
        }
        [TestMethod]
        public void CalculationItemDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable).CalculationItems["CalcItem"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void CalculationItemExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable).CalculationItems["CalcItem"];
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void CalculationItemNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Tables["CalculationGroup"] as CalculationGroupTable).CalculationItems["CalcItem"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
	[TestClass]
	public class SingleColumnRelationshipGeneratedTests
	{
        [TestMethod]
        public void SingleColumnRelationshipFromCardinalityTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.FromCardinality;

            if(obj.FromCardinality != RelationshipEndCardinality.None)
            {
                obj.FromCardinality = RelationshipEndCardinality.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.FromCardinality);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RelationshipEndCardinality.None, obj.FromCardinality);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.FromCardinality);
            }

            if(obj.FromCardinality != RelationshipEndCardinality.One)
            {
                obj.FromCardinality = RelationshipEndCardinality.One;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.FromCardinality);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RelationshipEndCardinality.One, obj.FromCardinality);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.FromCardinality);
            }

            if(obj.FromCardinality != RelationshipEndCardinality.Many)
            {
                obj.FromCardinality = RelationshipEndCardinality.Many;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.FromCardinality);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RelationshipEndCardinality.Many, obj.FromCardinality);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.FromCardinality);
            }
        }    
        [TestMethod]
        public void SingleColumnRelationshipToCardinalityTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.ToCardinality;

            if(obj.ToCardinality != RelationshipEndCardinality.None)
            {
                obj.ToCardinality = RelationshipEndCardinality.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ToCardinality);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RelationshipEndCardinality.None, obj.ToCardinality);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ToCardinality);
            }

            if(obj.ToCardinality != RelationshipEndCardinality.One)
            {
                obj.ToCardinality = RelationshipEndCardinality.One;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ToCardinality);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RelationshipEndCardinality.One, obj.ToCardinality);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ToCardinality);
            }

            if(obj.ToCardinality != RelationshipEndCardinality.Many)
            {
                obj.ToCardinality = RelationshipEndCardinality.Many;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ToCardinality);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(RelationshipEndCardinality.Many, obj.ToCardinality);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.ToCardinality);
            }
        }    
        [TestMethod]
        public void SingleColumnRelationshipIsActiveTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.IsActive;
            var value = orgValue;

            value = true;
            if(obj.IsActive != value)
            {
                obj.IsActive = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsActive);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsActive);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsActive);
            }

            value = false;
            if(obj.IsActive != value)
            {
                obj.IsActive = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsActive);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.IsActive);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.IsActive);
            }
        }
        [TestMethod]
        public void SingleColumnRelationshipCrossFilteringBehaviorTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.CrossFilteringBehavior;

            if(obj.CrossFilteringBehavior != CrossFilteringBehavior.OneDirection)
            {
                obj.CrossFilteringBehavior = CrossFilteringBehavior.OneDirection;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.CrossFilteringBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(CrossFilteringBehavior.OneDirection, obj.CrossFilteringBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.CrossFilteringBehavior);
            }

            if(obj.CrossFilteringBehavior != CrossFilteringBehavior.BothDirections)
            {
                obj.CrossFilteringBehavior = CrossFilteringBehavior.BothDirections;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.CrossFilteringBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(CrossFilteringBehavior.BothDirections, obj.CrossFilteringBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.CrossFilteringBehavior);
            }

            if(obj.CrossFilteringBehavior != CrossFilteringBehavior.Automatic)
            {
                obj.CrossFilteringBehavior = CrossFilteringBehavior.Automatic;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.CrossFilteringBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(CrossFilteringBehavior.Automatic, obj.CrossFilteringBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.CrossFilteringBehavior);
            }
        }    
        [TestMethod]
        public void SingleColumnRelationshipJoinOnDateBehaviorTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.JoinOnDateBehavior;

            if(obj.JoinOnDateBehavior != DateTimeRelationshipBehavior.DateAndTime)
            {
                obj.JoinOnDateBehavior = DateTimeRelationshipBehavior.DateAndTime;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.JoinOnDateBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DateTimeRelationshipBehavior.DateAndTime, obj.JoinOnDateBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.JoinOnDateBehavior);
            }

            if(obj.JoinOnDateBehavior != DateTimeRelationshipBehavior.DatePartOnly)
            {
                obj.JoinOnDateBehavior = DateTimeRelationshipBehavior.DatePartOnly;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.JoinOnDateBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(DateTimeRelationshipBehavior.DatePartOnly, obj.JoinOnDateBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.JoinOnDateBehavior);
            }
        }    
        [TestMethod]
        public void SingleColumnRelationshipRelyOnReferentialIntegrityTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.RelyOnReferentialIntegrity;
            var value = orgValue;

            value = true;
            if(obj.RelyOnReferentialIntegrity != value)
            {
                obj.RelyOnReferentialIntegrity = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RelyOnReferentialIntegrity);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.RelyOnReferentialIntegrity);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RelyOnReferentialIntegrity);
            }

            value = false;
            if(obj.RelyOnReferentialIntegrity != value)
            {
                obj.RelyOnReferentialIntegrity = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RelyOnReferentialIntegrity);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.RelyOnReferentialIntegrity);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.RelyOnReferentialIntegrity);
            }
        }
        [TestMethod]
        public void SingleColumnRelationshipSecurityFilteringBehaviorTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = (Model.Relationships[0] as SingleColumnRelationship);
            var orgValue = obj.SecurityFilteringBehavior;

            if(obj.SecurityFilteringBehavior != SecurityFilteringBehavior.OneDirection)
            {
                obj.SecurityFilteringBehavior = SecurityFilteringBehavior.OneDirection;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SecurityFilteringBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SecurityFilteringBehavior.OneDirection, obj.SecurityFilteringBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SecurityFilteringBehavior);
            }

            if(obj.SecurityFilteringBehavior != SecurityFilteringBehavior.BothDirections)
            {
                obj.SecurityFilteringBehavior = SecurityFilteringBehavior.BothDirections;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SecurityFilteringBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SecurityFilteringBehavior.BothDirections, obj.SecurityFilteringBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SecurityFilteringBehavior);
            }

            if(obj.SecurityFilteringBehavior != SecurityFilteringBehavior.None)
            {
                obj.SecurityFilteringBehavior = SecurityFilteringBehavior.None;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SecurityFilteringBehavior);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(SecurityFilteringBehavior.None, obj.SecurityFilteringBehavior);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.SecurityFilteringBehavior);
            }
        }    
    }
	[TestClass]
	public class NamedExpressionGeneratedTests
	{
        [TestMethod]
        public void NamedExpressionDescriptionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.Description;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Description != value)
            {
                obj.Description = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Description);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Description);
            }
        }
        [TestMethod]
        public void NamedExpressionKindTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.Kind;

            if(obj.Kind != ExpressionKind.M)
            {
                obj.Kind = ExpressionKind.M;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Kind);

                handler.UndoManager.Redo();
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(ExpressionKind.M, obj.Kind);

                handler.UndoManager.Undo();
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(orgValue, obj.Kind);
            }
        }    
        [TestMethod]
        public void NamedExpressionExpressionTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.Expression;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Expression != value)
            {
                obj.Expression = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Expression);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Expression);
            }
        }
        [TestMethod]
        public void NamedExpressionMAttributesTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.MAttributes;
            var value = orgValue;

            value = "fr-FR";
            if(obj.MAttributes != value)
            {
                obj.MAttributes = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MAttributes);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.MAttributes);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.MAttributes);
            }
        }
        [TestMethod]
        public void NamedExpressionLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.LineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.LineageTag != value)
            {
                obj.LineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.LineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.LineageTag);
            }
        }
        [TestMethod]
        public void NamedExpressionSourceLineageTagTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.SourceLineageTag;
            var value = orgValue;

            value = "fr-FR";
            if(obj.SourceLineageTag != value)
            {
                obj.SourceLineageTag = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.SourceLineageTag);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.SourceLineageTag);
            }
        }
        [TestMethod]
        public void NamedExpressionNameTest()
        {
            var handler = new TabularModelHandler("TestData\\AllProperties.bim");
            var Model = handler.Model;
            var obj = Model.Expressions["NamedExpr"];
            var orgValue = obj.Name;
            var value = orgValue;

            value = "fr-FR";
            if(obj.Name != value)
            {
                obj.Name = value;
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);

                handler.UndoManager.Redo();
                Assert.AreEqual(1, handler.UndoManager.UndoSteps);
                Assert.AreEqual(0, handler.UndoManager.RedoSteps);
                Assert.AreEqual(value, obj.Name);

                handler.UndoManager.Undo();
                Assert.AreEqual(0, handler.UndoManager.UndoSteps);
                Assert.AreEqual(1, handler.UndoManager.RedoSteps);
                Assert.AreEqual(orgValue, obj.Name);
            }
        }
    }
}