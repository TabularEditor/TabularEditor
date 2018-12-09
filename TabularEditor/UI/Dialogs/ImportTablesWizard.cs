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
                        break;
                    case 2:
                        page2.Visible = true;
                        page1.Visible = false;
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
            if (CurrentPage != 2) return false;
            return page2.SelectedSchemas.Any();
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
            dialog.page2.InitialSelection = SchemaNode.FromJson(table.Partitions[0].GetAnnotation("TabularEditor_TableSchema"));

            dialog.page2.Init(TypedDataSource.GetFromTabularDs(table.Partitions[0].DataSource as ProviderDataSource));
            dialog.page2.Visible = true;

            // TODO:

            var res = dialog.ShowDialog();
            if (res == DialogResult.OK) DoUpdate(table, dialog.page2.Source, dialog.page2.SelectedSchemas.First());
            return res;
        }

        public static DialogResult ShowWizard(Model model)
        {
            var dialog = new ImportTablesWizard();
            dialog.Model = model;
            dialog.page1.Init(model);
            dialog.CurrentPage = 1;
            var res = dialog.ShowDialog();

            if (res == DialogResult.OK) DoImport(dialog.page1.Mode, model, dialog.page2.Source, dialog.page2.SelectedSchemas);

            return res;
        }

        public static DialogResult ShowWizard(Model model, ProviderDataSource source)
        {
            var dialog = new ImportTablesWizard();
            dialog.page1.Init(model);
            dialog.page2.Init(TypedDataSource.GetFromTabularDs(source));
            dialog.CurrentPage = 2;
            var res = dialog.ShowDialog();

            if (res == DialogResult.OK) DoImport(dialog.page1.Mode, model, dialog.page2.Source, dialog.page2.SelectedSchemas);

            return res;
        }

        private static void DoUpdate(Table table, TypedDataSource source, SchemaNode tableSchema)
        {
            table.Partitions[0].Name = tableSchema.Name;
            table.Partitions[0].Query = tableSchema.GetSql(true, source.UseThreePartName);
            table.Partitions[0].SetAnnotation("TabularEditor_TableSchema", tableSchema.ToJson());

            var schemaTable = source.GetSchemaTable(tableSchema);
            var updatedColumns = new HashSet<TOMWrapper.DataColumn>();
            foreach (DataRow row in schemaTable.Rows)
            {
                var sourceColumn = row["ColumnName"].ToString();
                var dataTypeName = row["DataTypeName"].ToString();
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

        private static void DoImport(Pages.ImportMode importMode, Model model, TypedDataSource source, IEnumerable<SchemaNode> schemaNodes)
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
                newTable.Partitions[0].Query = tableSchema.GetSql(true, source.UseThreePartName);
                if(importMode != Pages.ImportMode.UseTempDs)
                    newTable.Partitions[0].DataSource = model.DataSources[source.TabularDsName];

                var schemaTable = source.GetSchemaTable(tableSchema);
                foreach (DataRow row in schemaTable.Rows)
                {
                    var sourceColumn = row["ColumnName"].ToString();
                    var col = newTable.AddDataColumn(
                        sourceColumn, sourceColumn,
                        null,
                        TableMetadata.DataTypeMap(row["DataTypeName"].ToString())
                        );
                    col.SourceProviderType = row["DataTypeName"].ToString();
                }
                newTable.Partitions[0].SetAnnotation("TabularEditor_TableSchema", tableSchema.ToJson());
                newTable.Select();
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
                            page2.Init(TypedDataSource.GetFromTabularDs(page1.CurrentDataSource));
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
                        throw new NotImplementedException();
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
    }
}
