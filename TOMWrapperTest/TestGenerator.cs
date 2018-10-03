using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.TOMWrapper.Serialization;

namespace TabularEditor.TOMWrapper.Tests
{
	[TestClass]
	public class GeneratedTests
	{


		[TestMethod]
		public void Model_AddPerspective_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			tmh.Model.AddPerspective();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			tmh.Model.AddPerspective();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			tmh.Model.AddPerspective();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1200);
			var obj = tmh.Model.AddPerspective();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			tmh.Model.AddCalculatedTable();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			tmh.Model.AddCalculatedTable();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			tmh.Model.AddCalculatedTable();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1200);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			tmh.Model.AddTable();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			tmh.Model.AddTable();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			tmh.Model.AddTable();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1200);
			var obj = tmh.Model.AddTable();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			tmh.Model.AddRelationship();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			tmh.Model.AddRelationship();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			tmh.Model.AddRelationship();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1200);
			var obj = tmh.Model.AddRelationship();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			tmh.Model.AddTranslation("da-DK");
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			tmh.Model.AddTranslation("da-DK");
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			tmh.Model.AddTranslation("da-DK");
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1200);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			tmh.Model.AddRole();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			tmh.Model.AddRole();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			tmh.Model.AddRole();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1200);
			var obj = tmh.Model.AddRole();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			tmh.Model.AddDataSource();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			tmh.Model.AddDataSource();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			tmh.Model.AddDataSource();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1200);
			var obj = tmh.Model.AddDataSource();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			tmh.Model.Tables[0].AddMeasure();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			tmh.Model.Tables[0].AddMeasure();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			tmh.Model.Tables[0].AddMeasure();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1200);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1200);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_BaseTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_UndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_UndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoRedoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoUndoTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_RenameTest_1200() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1200);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			tmh.Model.AddPerspective();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			tmh.Model.AddPerspective();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			tmh.Model.AddPerspective();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			tmh.Model.AddExpression();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			tmh.Model.AddExpression();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			tmh.Model.AddExpression();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			tmh.Model.AddCalculatedTable();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			tmh.Model.AddCalculatedTable();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			tmh.Model.AddCalculatedTable();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			tmh.Model.AddTable();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			tmh.Model.AddTable();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			tmh.Model.AddTable();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			tmh.Model.AddRelationship();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			tmh.Model.AddRelationship();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			tmh.Model.AddRelationship();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			tmh.Model.AddTranslation("da-DK");
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			tmh.Model.AddTranslation("da-DK");
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			tmh.Model.AddTranslation("da-DK");
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			tmh.Model.AddRole();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			tmh.Model.AddRole();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			tmh.Model.AddRole();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			tmh.Model.AddDataSource();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			tmh.Model.AddDataSource();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			tmh.Model.AddDataSource();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			tmh.Model.AddStructuredDataSource();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			tmh.Model.AddStructuredDataSource();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			tmh.Model.AddStructuredDataSource();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			tmh.Model.Tables[0].AddMeasure();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			tmh.Model.Tables[0].AddMeasure();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			tmh.Model.Tables[0].AddMeasure();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_BaseTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_UndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_UndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoRedoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoUndoTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_RenameTest_1400() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}
	}
}