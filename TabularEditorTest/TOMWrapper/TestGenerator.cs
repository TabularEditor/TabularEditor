using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TabularEditor.TOMWrapper.Tests
{
	[TestClass]
	public class GeneratedTests
	{


		[TestMethod]
		public void Model_AddPerspective_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			tmh.Model.AddPerspective();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			tmh.Model.AddPerspective();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			tmh.Model.AddPerspective();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddPerspective_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddPerspective.bim", 1400);
			var obj = tmh.Model.AddPerspective();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddPerspective.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			tmh.Model.AddExpression();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			tmh.Model.AddExpression();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			tmh.Model.AddExpression();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddExpression_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddExpression.bim", 1400);
			var obj = tmh.Model.AddExpression();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddExpression.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			tmh.Model.AddCalculatedTable();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			tmh.Model.AddCalculatedTable();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			tmh.Model.AddCalculatedTable();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddCalculatedTable_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedTable.bim", 1400);
			var obj = tmh.Model.AddCalculatedTable();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			tmh.Model.AddTable();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			tmh.Model.AddTable();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			tmh.Model.AddTable();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTable_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTable.bim", 1400);
			var obj = tmh.Model.AddTable();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTable.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			tmh.Model.AddRelationship();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			tmh.Model.AddRelationship();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			tmh.Model.AddRelationship();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRelationship_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRelationship.bim", 1400);
			var obj = tmh.Model.AddRelationship();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRelationship.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			tmh.Model.AddTranslation("da-DK");
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			tmh.Model.AddTranslation("da-DK");
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			tmh.Model.AddTranslation("da-DK");
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddTranslation_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddTranslation.bim", 1400);
			var obj = tmh.Model.AddTranslation("da-DK");
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddTranslation.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			tmh.Model.AddRole();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			tmh.Model.AddRole();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			tmh.Model.AddRole();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddRole_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddRole.bim", 1400);
			var obj = tmh.Model.AddRole();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddRole.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			tmh.Model.AddDataSource();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			tmh.Model.AddDataSource();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			tmh.Model.AddDataSource();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddDataSource_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataSource.bim", 1400);
			var obj = tmh.Model.AddDataSource();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			tmh.Model.AddStructuredDataSource();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			tmh.Model.AddStructuredDataSource();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			tmh.Model.AddStructuredDataSource();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Model_AddStructuredDataSource_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddStructuredDataSource.bim", 1400);
			var obj = tmh.Model.AddStructuredDataSource();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddStructuredDataSource.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			tmh.Model.Tables[0].AddMeasure();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			tmh.Model.Tables[0].AddMeasure();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			tmh.Model.Tables[0].AddMeasure();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddMeasure_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddMeasure.bim", 1400);
			var obj = tmh.Model.Tables[0].AddMeasure();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddMeasure.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			tmh.Model.Tables[0].AddCalculatedColumn();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddCalculatedColumn_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddCalculatedColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddCalculatedColumn();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddCalculatedColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			tmh.Model.Tables[0].AddDataColumn();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddDataColumn_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddDataColumn.bim", 1400);
			var obj = tmh.Model.Tables[0].AddDataColumn();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddDataColumn.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_BaseTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_UndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_UndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			tmh.Model.Tables[0].AddHierarchy();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoRedoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Redo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_DeleteUndoUndoTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Delete();
			tmh.UndoManager.Undo();
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}

		[TestMethod]
		public void Table_AddHierarchy_RenameTest() {
			var tmh = ObjectHandlingTests.CreateTestModel("Test_AddHierarchy.bim", 1400);
			var obj = tmh.Model.Tables[0].AddHierarchy();
			obj.Name = "ChangeName";
			tmh.UndoManager.Undo();
			tmh.Save("Test_AddHierarchy.bim", SaveFormat.ModelSchemaOnly, SerializeOptions.Default);
		}
	}
}