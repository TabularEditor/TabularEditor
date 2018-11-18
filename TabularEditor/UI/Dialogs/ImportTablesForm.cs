using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aga.Controls.Tree;
using TabularEditor.TOMWrapper;
using System.Data.OleDb;
using TabularEditor.UI.Actions;

namespace TabularEditor.UI.Dialogs
{

    public partial class ImportTablesForm : Form
    {
        private static ImportTablesForm _singleton = null;

        public static void ImportTable(Model model)
        {
            if (_singleton == null) _singleton = new ImportTablesForm();

            _singleton.treeViewAdv1.Model = null;
            _singleton.comboBox1.Items.Clear();
            _singleton.comboBox1.Items.AddRange(model.DataSources
                .OfType<ProviderDataSource>()
                .Where(ds => ds.Provider.IndexOf("SQLNCLI", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                             ds.Provider.IndexOf("OLEDB", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                             ds.ConnectionString.IndexOf("SQLNCLI", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                             ds.ConnectionString.IndexOf("OLEDB", StringComparison.InvariantCultureIgnoreCase) >= 0).ToArray());
            if (_singleton.comboBox1.Items.Count > 0) _singleton.comboBox1.SelectedIndex = 0;
            _singleton.btnImportFromQuery.Enabled = _singleton.comboBox1.Items.Count > 0;
            _singleton.UpdateUI();

            if (_singleton.ShowDialog() == DialogResult.OK)
            {
                var t = model.AddTable();
                foreach(var col in _singleton.currentTreeModel.Columns.Where(c => c.Import))
                {
                    t.AddDataColumn(col.Name, col.SourceColumn, null, col.GetDataType());
                }
                var currentDs = _singleton.comboBox1.SelectedItem as ProviderDataSource;
                if (_singleton.FromQuery && currentDs != null)
                {
                    var p = Partition.CreateNew(t);
                    t.Partitions[0].Delete();
                    p.DataSource = currentDs;
                    p.Query = _singleton.textBox1.Text;
                }
                t.Vis().Edit();
            }
        }

        public ImportTablesForm()
        {
            InitializeComponent();
            comboBox1.DisplayMember = "Name";
        }

        ImportColumnsTreeModel currentTreeModel;

        private void button1_Click(object sender, EventArgs e)
        {
            currentTreeModel = ImportColumnsTreeModel.CreateFromClipboard();
            if (currentTreeModel != null)
            {
                treeViewAdv1.Model = currentTreeModel;
                FromQuery = false;
            }

            UpdateUI();
        }

        private void nodeCheckBox1_CheckStateChanged(object sender, TreePathEventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            btnImport.Enabled = currentTreeModel != null && currentTreeModel.Columns.Any(c => c.Import);
        }

        private bool FromQuery = false;

        private void btnImportFromQuery_Click(object sender, EventArgs e)
        {
            currentTreeModel = ImportColumnsTreeModel.CreateFromOLEDBQuery((comboBox1.SelectedItem as ProviderDataSource).ConnectionString, textBox1.Text);
            if (currentTreeModel != null)
            {
                treeViewAdv1.Model = currentTreeModel;
                FromQuery = true;
            }

            UpdateUI();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/otykier/TabularEditor/wiki/Importing-Tables#power-query-data-sources");
        }
    }

    public class ImportColumnsTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        internal List<ImportColumn> Columns = new List<ImportColumn>();

        public static ImportColumnsTreeModel CreateFromOLEDBQuery(string connectionString, string queryText)
        {
            var result = new ImportColumnsTreeModel();

            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new OleDbCommand(queryText, conn);
                    var rdr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                    var schema = rdr.GetSchemaTable();

                    foreach (DataRow row in schema.Rows)
                    {
                        var name = (string)row["ColumnName"];
                        var type = (Type)row["DataType"];
                        var providerType = (OleDbType)(int)row["ProviderType"];

                        if (!result.Columns.Any(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var col = new ImportColumn(result, name, Table.DataTypeMapping.ContainsKey(type) ? Table.DataTypeMapping[type] : DataType.Automatic, providerType);
                            result.Columns.Add(col);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not establish connection to data source", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return result;
        }

        public static ImportColumnsTreeModel CreateFromClipboard()
        {
            var result = new ImportColumnsTreeModel();

            var text = Clipboard.GetText();
            try
            {
                var dataRows = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var header = dataRows[0].Split('\t');
                var nameIndex = Array.FindIndex(header, s => s.Equals("Name", StringComparison.InvariantCultureIgnoreCase));
                var typeIndex = Array.FindIndex(header, s => s.Equals("TypeName", StringComparison.InvariantCultureIgnoreCase));
                if(typeIndex == -1) typeIndex = Array.FindIndex(header, s => s.IndexOf("Type", StringComparison.InvariantCultureIgnoreCase) >= 0);
                for (int i = 1; i < dataRows.Length; i++)
                {
                    var dataCols = dataRows[i].Split('\t');
                    var importColumn = new ImportColumn(result, dataCols[nameIndex], dataCols[typeIndex]);
                    result.Columns.Add(importColumn);
                }

                if (result.Columns.Count == 0) throw new Exception();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The clipboard does not currently contain any Power Query schema data.", "No schema data in clipboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return result;
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            return Columns;
        }

        public bool IsLeaf(TreePath treePath)
        {
            return true;
        }
    }

    public class ImportColumn
    {
        ImportColumnsTreeModel TreeModel;

        public ImportColumn(ImportColumnsTreeModel treeModel, string name, DataType dataType, OleDbType providerType)
        {
            TreeModel = treeModel;
            SourceColumn = name;
            Name = name;
            ProviderType = providerType;
            Import = true;
            switch(dataType)
            {
                case TOMWrapper.DataType.Boolean: DataType = "Boolean"; break;
                case TOMWrapper.DataType.DateTime: DataType = "Date/Time"; break;
                case TOMWrapper.DataType.Decimal: DataType = "Currency"; break;
                case TOMWrapper.DataType.Double: DataType = "Real"; break;
                case TOMWrapper.DataType.Int64: DataType = "Integer"; break;
                case TOMWrapper.DataType.String: DataType = "Text"; break;
                case TOMWrapper.DataType.Binary: DataType = "Binary"; break;
                default: DataType = "Text"; break;
            }
        }

        public ImportColumn(ImportColumnsTreeModel treeModel, string name, string typeName)
        {
            TreeModel = treeModel;
            SourceColumn = name;
            Name = name;
            Import = true;
            switch(typeName.ToLower())
            {
                case "binary.type":
                case "binary":
                case "varbinary":
                case "variant":
                case "sqlvariant":
                    DataType = "Binary"; break;

                case "any.type":
                case "text.type":
                case "text":
                case "string":
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    DataType = "Text"; break;

                case "number.type":
                case "double.type":
                case "single.type":
                case "percentage.type":
                case "duration.type":
                case "number":
                case "real":
                case "float":
                case "single":
                case "double":
                    DataType = "Real"; break;

                case "currency.type":
                case "decimal.type":
                case "currency":
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    DataType = "Currency"; break;

                case "int64.type":
                case "int32.type":
                case "int16.type":
                case "byte.type":
                case "int":
                case "integer":
                case "whole":
                case "byte":
                case "bigint":
                case "smallint":
                case "tinyint":
                case "int64":
                case "int32":
                case "long":
                    DataType = "Integer"; break;

                case "datetimezone.type":
                case "datetime.type":
                case "date.type":
                case "time.type":
                case "datetime":
                case "date":
                case "time":
                    DataType = "Date/Time"; break;

                case "logical.type":
                case "boolean":
                case "bool":
                case "bit":
                    DataType = "Boolean"; break;

                default:
                    DataType = "Text"; break;
            }
        }

        public OleDbType ProviderType { get; set; }
        public string SourceColumn { get; set; }
        public bool Import { get; set; }
        public string DataType { get;set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (TreeModel.Columns.Any(c => c != this && c.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase)))
                    throw new InvalidOperationException("Another column with this name already exists.");
                _name = value;
            }
        }

        public DataType GetDataType()
        {
            switch(DataType)
            {
                case "Integer": return TOMWrapper.DataType.Int64;
                case "Real": return TOMWrapper.DataType.Double;
                case "Boolean": return TOMWrapper.DataType.Boolean;
                case "Text": return TOMWrapper.DataType.String;
                case "Date/Time": return TOMWrapper.DataType.DateTime;
                case "Currency": return TOMWrapper.DataType.Decimal;
                case "Binary": return TOMWrapper.DataType.Binary;
                default: return TOMWrapper.DataType.String;
            }
        }

        //Integer
        //Real
        //Boolean
        //Text
        //Date/Time
        //Currency


    }
}
