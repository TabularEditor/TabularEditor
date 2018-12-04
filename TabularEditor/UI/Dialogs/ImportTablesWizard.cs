extern alias json;

using json::Newtonsoft.Json;
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
                btnNext.Enabled = CurrentPage < 2;
            }
        }

        private bool CanImport()
        {
            if (CurrentPage != 2) return false;
            return page2.SelectedSchemas.Any();
        }

        public static DialogResult ShowWizard(Table table)
        {
            if(!(table.Partitions[0].DataSource is ProviderDataSource))
            {
                MessageBox.Show("This feature currently only supports tables using Legacy Data Sources.", "Unsupported Data Source", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return DialogResult.Cancel;
            }

            var dialog = new ImportTablesWizard();
            dialog.btnBack.Visible = false;
            dialog.btnNext.Visible = false;
            dialog.btnImport.Text = "OK";
            dialog.page2.SingleSelection = true;
            dialog.page2.Init(TypedDataSource.GetFromTabularDs(table.Partitions[0].DataSource as ProviderDataSource));


            try
            {
                var snAnnotation = table.GetAnnotation("TabularEditor_TableSchema");
                var sn = JsonConvert.DeserializeObject<SchemaNode>(snAnnotation);
                dialog.page2.InitialSelection = sn; // TODO: Make sure the ImportTablesPage displays the provided table pre-selected
            }
            catch
            {
            }
            
            // TODO:

            var res = dialog.ShowDialog();
            if (res == DialogResult.OK) DoUpdate(table, dialog.page2.Source, dialog.page2.SelectedSchemas);
            return res;
        }

        public static DialogResult ShowWizard(Model model)
        {
            var dialog = new ImportTablesWizard();
            dialog.page1.Init(model);
            dialog.CurrentPage = 1;
            var res = dialog.ShowDialog();

            if (res == DialogResult.OK) DoImport(model, dialog.page2.Source, dialog.page2.SelectedSchemas);

            return res;
        }

        public static DialogResult ShowWizard(Model model, ProviderDataSource source)
        {
            var dialog = new ImportTablesWizard();
            dialog.page1.Init(model);
            dialog.page2.Init(TypedDataSource.GetFromTabularDs(source));
            dialog.CurrentPage = 2;
            var res = dialog.ShowDialog();

            if (res == DialogResult.OK) DoImport(model, dialog.page2.Source, dialog.page2.SelectedSchemas);

            return res;
        }

        private static void DoUpdate(Table table, TypedDataSource source, IEnumerable<SchemaNode> schemaNodes)
        {
            throw new NotImplementedException();
        }

        private static void DoImport(Model model, TypedDataSource source, IEnumerable<SchemaNode> schemaNodes)
        {
            foreach(var tableSchema in schemaNodes)
            {
                var newTable = model.AddTable(tableSchema.Name);
                if (newTable.Partitions[0] is MPartition)
                {
                    newTable.Partitions[0].Delete();
                    Partition.CreateNew(newTable);
                }
                newTable.Partitions[0].Name = tableSchema.Name;
                newTable.Partitions[0].Query = tableSchema.GetSql(true, source.UseThreePartName);
                newTable.Partitions[0].DataSource = model.DataSources[source.TabularDsName];

                var schemaTable = source.GetSchemaTable(tableSchema);
                foreach (DataRow row in schemaTable.Rows)
                {
                    var col = newTable.AddDataColumn(
                        row["ColumnName"].ToString(),
                        row["ColumnName"].ToString(),
                        null,
                        TableMetadata.DataTypeMap(row["DataTypeName"].ToString())
                        );
                    col.SourceProviderType = row["DataTypeName"].ToString();
                }
                newTable.SetAnnotation("TabularEditor_TableSchema", JsonConvert.SerializeObject(tableSchema));
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
                if(page1.Mode == Pages.ImportMode.UseExistingDs && page1.CurrentDataSource != null)
                {
                    page2.Init(TypedDataSource.GetFromTabularDs(page1.CurrentDataSource));
                    CurrentPage = 2; return;
                }
            }
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
