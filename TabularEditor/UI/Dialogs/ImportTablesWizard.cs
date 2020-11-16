extern alias json;

using json::Newtonsoft.Json;
using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Actions;
using TabularEditor.UIServices;

namespace TabularEditor.UI.Dialogs
{
    public partial class ImportTablesWizard : Form
    {
        public ImportTablesWizard()
        {
            InitializeComponent();
        }

        private int _currentPage;
        public int CurrentPage { get { return _currentPage; }
            set {
                _currentPage = value;
                switch(_currentPage)
                {
                    case 1:
                        page1.Visible = true;
                        page2.Visible = false;
                        page3.Visible = false;
                        break;
                    case 2:
                        page1.Visible = false;
                        page2.Visible = true;
                        page3.Visible = false;
                        break;
                    case 3:
                        page1.Visible = false;
                        page2.Visible = false;
                        page3.Visible = true;
                        break;
                }

                btnBack.Enabled = CurrentPage > 1;
                btnImport.Enabled = CanImport();
                if(CurrentPage == 1) {
                    btnNext.Enabled = page1.Mode != Pages.ImportMode.UseExistingDs || page1.CurrentDataSource != null;
                }
                else
                    btnNext.Enabled = CurrentPage < 2;
            }
        }

        private bool CanImport()
        {
            if (CurrentPage == 2) return page2.SelectedSchemas.Any();
            if (CurrentPage == 3) return page3.CurrentSchema?.Any(cs => !string.IsNullOrWhiteSpace(cs.Name)) == true;
            return false;
        }

        private Model Model;

        public static DialogResult ShowWizard(Table table)
        {
            if(!(table.Partitions[0].DataSource is ProviderDataSource))
            {
                MessageBox.Show("This feature currently only supports tables using Legacy Data Sources.", "Unsupported Data Source", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return DialogResult.Cancel;
            }

            var dialog = new ImportTablesWizard();
            dialog.Model = table.Model;
            dialog._currentPage = 2;
            dialog.btnBack.Visible = false;
            dialog.btnNext.Visible = false;
            dialog.btnCancel.Left = 654;
            dialog.page2.lblHeader.Text = "Choose the table/view you want to use as a source for " + table.DaxObjectFullName + ":";
            dialog.btnImport.Text = "OK";
            dialog.page2.SingleSelection = true;
            dialog.page2.InitialSelection = table.GetTableSchema();

            dialog.page2.RowLimitClause = table.GetRowLimitClause();
            dialog.page2.IdentifierQuoting = table.GetIdentifierQuoting();

            if (!dialog.page2.Init(TypedDataSource.GetFromTabularDs(table.Partitions[0].DataSource as ProviderDataSource))) return DialogResult.Cancel;
            dialog.page2.Visible = true;

            // TODO:

            var res = dialog.ShowDialog();
            if (res == DialogResult.OK) DoUpdate(table, dialog.page2.Source, dialog.page2.SelectedSchemas.First(), dialog.page2.RowLimitClause, dialog.page2.IdentifierQuoting);
            return res;
        }

        public static DialogResult ShowWizard(Model model)
        {
            var dialog = new ImportTablesWizard();
            dialog.Model = model;
            dialog.page1.Init(model);
            dialog.CurrentPage = 1;
            var res = dialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                if (dialog.CurrentPage == 2)
                    DoImport(dialog.page1.Mode, model, dialog.page2.Source, dialog.page2.SelectedSchemas, dialog.page2.RowLimitClause, dialog.page2.IdentifierQuoting);
                else if (dialog.CurrentPage == 3)
                    DoImport(model, dialog.page3.CurrentSchema);
            }

            return res;
        }

        public static DialogResult ShowWizard(Model model, ProviderDataSource source)
        {
            var dialog = new ImportTablesWizard();
            //dialog.page1.Init(model);

            dialog.btnBack.Visible = false;
            dialog.btnNext.Visible = false;
            dialog.btnCancel.Left = 654;

            dialog.page2.RowLimitClause = source.GetRowLimitClause();
            dialog.page2.IdentifierQuoting = source.GetIdentifierQuoting();

            var tds = TypedDataSource.GetFromTabularDs(source);
            if (!dialog.page2.Init(tds)) return DialogResult.Cancel;
            dialog.CurrentPage = 2;
            var res = dialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                if (dialog.CurrentPage == 2)
                    DoImport(dialog.page1.Mode, model, dialog.page2.Source, dialog.page2.SelectedSchemas, dialog.page2.RowLimitClause, dialog.page2.IdentifierQuoting);
                else if (dialog.CurrentPage == 3)
                    DoImport(model, dialog.page3.CurrentSchema);
            }

            return res;
        }

        private static void DoUpdate(Table table, TypedDataSource source, SchemaNode tableSchema, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting)
        {
            table.Partitions[0].Name = tableSchema.Name;
            table.Partitions[0].Query = tableSchema.GetSql(identifierQuoting, true, source.UseThreePartName);
            table.SetTableSchema(tableSchema);

            if (!(source is SqlDataSource))
            {
                table.SetRowLimitClause(rowLimitClause);
                table.SetIdentifierQuoting(identifierQuoting);
            }

            var schemaTable = source.GetSchemaTable(tableSchema, identifierQuoting);
            var updatedColumns = new HashSet<TOMWrapper.DataColumn>();
            foreach (DataRow row in schemaTable.Rows)
            {
                var sourceColumn = row["ColumnName"].ToString();
                var dataTypeName =
                    schemaTable.Columns.Contains("DataTypeName") ?
                        row["DataTypeName"].ToString() :
                        (row["DataType"] as Type).Name;
                var column = table.DataColumns.FirstOrDefault(c => c.SourceColumn.EqualsI(sourceColumn));
                if (column == null) column = table.AddDataColumn(sourceColumn, sourceColumn);
                column.DataType = TableMetadata.DataTypeMap(dataTypeName);
                column.SourceProviderType = dataTypeName;

                updatedColumns.Add(column);
            }
            foreach (var col in table.DataColumns.Except(updatedColumns).ToList())
            {
                col.Delete();
            }
        }

        private static void DoImport(Model model, List<Pages.SchemaColumn> schema)
        {
            var table = model.AddTable();
            foreach(var col in schema)
            {
                table.AddDataColumn(col.Name, col.Source, null, col.DataType);
            }
            table.Edit();
            if (UIController.Current.TreeModel.Perspective != null)
                table.InPerspective[UIController.Current.TreeModel.Perspective] = true;
        }

        private static void DoImport(Pages.ImportMode importMode, Model model, TypedDataSource source, IEnumerable<SchemaNode> schemaNodes, RowLimitClause rowLimitClause, IdentifierQuoting identifierQuoting)
        {
            
            foreach (var tableSchema in schemaNodes)
            {
                var newTable = model.AddTable(tableSchema.Name);
                if (newTable.Partitions[0] is MPartition)
                {
                    Partition.CreateNew(newTable);
                    newTable.Partitions[0].Delete();
                }
                newTable.Partitions[0].Name = tableSchema.Name;
                newTable.Partitions[0].Query = tableSchema.GetSql(identifierQuoting, true, source.UseThreePartName);
                if (source?.TabularDsName != null && model.DataSources.Contains(source.TabularDsName))
                {
                    newTable.Partitions[0].DataSource = model.DataSources[source.TabularDsName];
                }

                if (importMode != Pages.ImportMode.UseTempDs && !(source is SqlDataSource))
                {
                    newTable.SetRowLimitClause(rowLimitClause);
                    newTable.SetIdentifierQuoting(identifierQuoting);
                }

                var schemaTable = source.GetSchemaTable(tableSchema, identifierQuoting);
                foreach (DataRow row in schemaTable.Rows)
                {
                    var sourceColumn = row["ColumnName"].ToString();
                    var dataType =
                        schemaTable.Columns.Contains("DataTypeName") ?
                            row["DataTypeName"].ToString() :
                            (row["DataType"] as Type).Name;
                    var col = newTable.AddDataColumn(
                        sourceColumn, sourceColumn,
                        null,
                        TableMetadata.DataTypeMap(dataType)
                        );
                    col.SourceProviderType = dataType;
                }
                newTable.SetTableSchema(tableSchema);
                newTable.Select();
                if (UIController.Current.TreeModel.Perspective != null)
                    newTable.InPerspective[UIController.Current.TreeModel.Perspective] = true;
            }
        }

        private void ImportTablesWizard_Shown(object sender, EventArgs e)
        {
            page2.ExpandFirstNode();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(CurrentPage == 1)
            {
                switch (page1.Mode)
                {
                    case Pages.ImportMode.UseExistingDs:
                        if (page1.CurrentDataSource != null)
                        {
                            if (!page2.Init(TypedDataSource.GetFromTabularDs(page1.CurrentDataSource))) return;
                            CurrentPage = 2; return;
                        }
                        break;

                    case Pages.ImportMode.UseNewDs:
                        var connectionDialog = ShowConnectionDialog();
                        if (connectionDialog == null) return;
                        var source = TypedDataSource.GetFromConnectionUi(connectionDialog);
                        var tabularDs = Model.AddDataSource(source.SuggestSourceName());
                        ConnectionUIHelper.ApplyToTabularDs(connectionDialog, tabularDs);
                        source = TypedDataSource.GetFromTabularDs(tabularDs);
                        page2.Init(source);
                        CurrentPage = 2;
                        return;


                    case Pages.ImportMode.UseTempDs:
                        connectionDialog = ShowConnectionDialog();
                        if (connectionDialog == null) return;
                        source = TypedDataSource.GetFromConnectionUi(connectionDialog);
                        source.TabularDsName = "(Temporary connection)";
                        page2.Init(source);
                        CurrentPage = 2;
                        return;

                    case Pages.ImportMode.UseClipboard:
                        page3.Visible = true;
                        page3.BringToFront();
                        CurrentPage = 3;
                        break;
                }
            }
        }

        private DataConnectionDialog ShowConnectionDialog()
        {
            var dcd = new DataConnectionDialog();
            Microsoft.Data.ConnectionUI.DataSource.AddStandardDataSources(dcd);
            dcd.SelectedDataSource = Microsoft.Data.ConnectionUI.DataSource.SqlDataSource;
            dcd.SelectedDataProvider = DataProvider.SqlDataProvider;
            var res = DataConnectionDialog.Show(dcd);

            if (res == DialogResult.OK)
            {
                return dcd;
            }
            return null;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if(CurrentPage == 2)
            {
                CurrentPage = 1; return;
            }
            if(CurrentPage == 3)
            {
                page3.Visible = false;
                CurrentPage = 1; return;
            }
        }

        private void page1_Validated(object sender, EventArgs e)
        {
            if (CurrentPage == 1)
            {
                btnNext.Enabled = page1.Mode != Pages.ImportMode.UseExistingDs || page1.CurrentDataSource != null;
            }
        }

        private void page2_Validated(object sender, EventArgs e)
        {
            if (CurrentPage == 2)
            {
                btnImport.Enabled = CanImport();
            }
        }

        private void page3_Validated(object sender, EventArgs e)
        {
            if (CurrentPage == 3)
            {
                btnImport.Enabled = CanImport();
            }
        }
    }

    internal static class ImportAnnotationsHelper
    {
        const string QUOTING = "TabularEditor_IdentifierQuoting";
        const string LIMITCLAUSE = "TabularEditor_RowLimitClause";
        const string SCHEMA = "TabularEditor_TableSchema";
        const string PREVIEWCONNSTRING = "TabularEditor_PreviewConnectionString";

        public static IdentifierQuoting GetIdentifierQuoting(this TOMWrapper.DataSource source)
        {
            var value = source.GetAnnotation(QUOTING);
            if (Enum.TryParse(value, out IdentifierQuoting parsedValue))
            {
                return parsedValue;
            }

            return IdentifierQuoting.SquareBracket; // Default
        }

        public static void SetIdentifierQuoting(this TOMWrapper.DataSource source, IdentifierQuoting identifierQuoting)
        {
            source.SetAnnotation(QUOTING, $"{(int)identifierQuoting}");
        }
        public static void SetIdentifierQuoting(this Table table, IdentifierQuoting identifierQuoting)
        {
            table.Partitions[0].DataSource.SetIdentifierQuoting(identifierQuoting);
        }
        public static void SetRowLimitClause(this Table table, RowLimitClause rowLimitClause)
        {
            table.Partitions[0].DataSource.SetRowLimitClause(rowLimitClause);
        }

        public static RowLimitClause GetRowLimitClause(this Table table)
        {
            return table.Partitions[0].DataSource.GetRowLimitClause();
        }
        public static IdentifierQuoting GetIdentifierQuoting(this Table table)
        {
            return table.Partitions[0].DataSource.GetIdentifierQuoting();
        }

        public static RowLimitClause GetRowLimitClause(this TOMWrapper.DataSource source)
        {
            var value = source.GetAnnotation(LIMITCLAUSE);
            if (Enum.TryParse(value, out RowLimitClause parsedValue))
            {
                return parsedValue;
            }

            return RowLimitClause.Top; // Default
        }
        public static string GetPreviewConnectionString(this TOMWrapper.DataSource source)
        {
            if (source.HasAnnotation(PREVIEWCONNSTRING))
                return source.GetAnnotation(PREVIEWCONNSTRING);
            else
                return null;
        }
        public static void SetRowLimitClause(this TOMWrapper.DataSource source, RowLimitClause rowLimitClause)
        {
            source.SetAnnotation(LIMITCLAUSE, $"{(int)rowLimitClause}");
        }

        public static SchemaNode GetTableSchema(this Table table)
        {
            return SchemaNode.FromJson(table.Partitions[0].GetAnnotation(SCHEMA));
        }

        public static void SetTableSchema(this Table table, SchemaNode schema)
        {
            table.Partitions[0].SetAnnotation(SCHEMA, schema.ToJson());
        }
    }
}
