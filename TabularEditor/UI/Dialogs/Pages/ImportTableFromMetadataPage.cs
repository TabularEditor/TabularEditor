using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI.Dialogs.Pages
{
    public partial class ImportTableFromMetadataPage : UserControl
    {
        public ImportTableFromMetadataPage()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;

            var nameCol = new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name" };
            var sourceCol = new DataGridViewTextBoxColumn { HeaderText = "Source Column", DataPropertyName = "Source" };
            var dataTypeCol = new DataGridViewComboBoxColumn {
                HeaderText = "Data Type",
                DataPropertyName = "DataType",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            dataTypeCol.DataSource = Enum.GetValues(typeof(DataType));

            dataGridView1.Columns.AddRange(nameCol, sourceCol, dataTypeCol);

            var schema = new List<SchemaColumn>();
            var bindingSource = new BindingSource();
            bindingSource.DataSource = schema;
            dataGridView1.DataSource = bindingSource;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.tabulareditor.com/Importing-Tables.html?highlight=import#power-query-data-sources");
        }
        private bool IsNameProperty(string text)
        {
            return text.EqualsI("name") || text.EqualsI("column") || text.EqualsI("field");
        }
        private bool IsTypeProperty(string text)
        {
            return text.IndexOf("type", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        enum LexerMode
        {
            SingleQuote,
            DoubleQuote,
            SquareQuote,
            TickQuote,
            AlphaNumeric,
            None
        }

        private string[] Split(string row, string columnSeparator)
        {
            if (columnSeparator == null) return new string[1] { row };
            if (columnSeparator == "\t") return row.Split('\t');

            char detectedSeparator = (char)0;

            var mode = LexerMode.None;

            var columns = new List<string>();

            // For all other column separators, we must handle quotes:
            int i = 0;
            int startAt = -1;
            while(i < row.Length)
            {
                switch (mode)
                {
                    case LexerMode.None:
                        switch(row[i])
                        {
                            case '\'': mode = LexerMode.SingleQuote; startAt = i + 1; break;
                            case '"': mode = LexerMode.DoubleQuote; startAt = i + 1; break;
                            case '`': mode = LexerMode.TickQuote; startAt = i + 1; break;
                            case '[': mode = LexerMode.SquareQuote; startAt = i + 1; break;
                            default:
                                if (char.IsLetterOrDigit(row[i]))
                                {
                                    mode = LexerMode.AlphaNumeric;
                                    startAt = i;
                                }
                                break;
                        }
                        break;
                    case LexerMode.SingleQuote:
                        if (row[i] == '\'')
                        {
                            columns.Add(row.Substring(startAt, i - startAt));
                            mode = LexerMode.None;
                        }
                        break;
                    case LexerMode.DoubleQuote:
                        if (row[i] == '"')
                        {
                            columns.Add(row.Substring(startAt, i - startAt));
                            mode = LexerMode.None;
                        }
                        break;
                    case LexerMode.TickQuote:
                        if (row[i] == '`')
                        {
                            columns.Add(row.Substring(startAt, i - startAt));
                            mode = LexerMode.None;
                        }
                        break;
                    case LexerMode.SquareQuote:
                        if (row[i] == ']')
                        {
                            columns.Add(row.Substring(startAt, i - startAt));
                            mode = LexerMode.None;
                        }
                        break;
                    case LexerMode.AlphaNumeric:
                        if(char.IsWhiteSpace(row[i]) || detectedSeparator == row[i] || (detectedSeparator == (char)0 &&
                            (row[i] == ',' || row[i] == ';' || row[i] == '\t')))
                        {
                            if (detectedSeparator == (char)0) detectedSeparator = row[i];
                            columns.Add(row.Substring(startAt, i - startAt));
                            mode = LexerMode.None;
                        }
                        break;
                }

                i++;
            }

            if (mode == LexerMode.AlphaNumeric) columns.Add(row.Substring(startAt, i - startAt));

            return columns.ToArray();
        }

        public List<SchemaColumn> parsedSchema;

        private bool TryParseSchema(string schema)
        {
            var dataRows = schema.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Trim('\r', ' ', '\t')).ToArray();
            if (dataRows.Length == 0) return false;

            parsedSchema = new List<SchemaColumn>();

            string columnSeparator;
            bool firstRowIsHeader = false;
            if (dataRows[0].Contains('\t')) columnSeparator = "\t";
            else if (dataRows[0].Contains("|")) columnSeparator = "|";
            else if (dataRows[0].Contains(',')) columnSeparator = ",";
            else if (dataRows[0].Contains(";")) columnSeparator = ";";
            else if (dataRows[0].Contains(" ")) columnSeparator = " ";
            else columnSeparator = null;

            var firstRow = Split(dataRows[0], columnSeparator);
            if (firstRow == null) return false;
            var expectedColumns = firstRow.Length;

            int nameIndex = -1; // Index of column containing names
            int typeIndex = -1; // Index of column containing data types

            if (expectedColumns > 1)
            {
                if(firstRow.Any(s => IsTypeProperty(s) || IsNameProperty(s)))
                {
                    firstRowIsHeader = true;
                    nameIndex = Array.FindIndex(firstRow, s => IsNameProperty(s));
                    typeIndex = Array.FindIndex(firstRow, s => IsTypeProperty(s));
                }
            }
            else
            {
                // Assume each line contains a column name
                if (IsNameProperty(dataRows[0])) firstRowIsHeader = true;
                nameIndex = 0;
            }

            // If no header and number of columns > 1 we assume that column names appear in the first column, and data types in the second:
            if(!firstRowIsHeader && expectedColumns > 1)
            {
                nameIndex = 0;
                typeIndex = 1;
            }

            for (int i = firstRowIsHeader ? 1 : 0; i < dataRows.Length; i++)
            {
                var dataCols = Split(dataRows[i], columnSeparator);

                if (nameIndex >= dataCols.Length) continue;
                if (typeIndex >= dataCols.Length) continue;

                var name = dataCols[nameIndex].Trim();
                var type = typeIndex == -1 ? DataType.String : ParseType(dataCols[typeIndex]);

                if (parsedSchema.Any(sc => sc.Name.EqualsI(name))) continue;

                parsedSchema.Add(new SchemaColumn { Name = name, Source = name, DataType = type });
            }

            return parsedSchema.Count > 0;
        }

        private DataType ParseType(string typeText)
        {
            if (typeText.ContainsAnyI("binary", "variant")) return DataType.Binary;
            if (typeText.ContainsAnyI("char", "text", "string", "any")) return DataType.String;
            if (typeText.ContainsAnyI("number", "double", "single", "percentage", "duration", "real", "float", "single", "double")) return DataType.Double;
            if (typeText.ContainsAnyI("currency", "decimal", "numeric", "money", "fixed")) return DataType.Decimal;
            if (typeText.ContainsAnyI("int", "byte", "whole", "long", "small")) return DataType.Int64;
            if (typeText.ContainsAnyI("date", "time")) return DataType.DateTime;
            if (typeText.ContainsAnyI("logical", "bool", "bit")) return DataType.Boolean;
            return DataType.String;
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }
        
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (TryParseSchema(richTextBox1.Text))
            {
                var source = new BindingSource();
                source.DataSource = parsedSchema;
                dataGridView1.DataSource = source;
            }
            OnValidated(new EventArgs());
        }

        private void ImportTableFromMetadataPage_Validated(object sender, EventArgs e)
        {

        }

        private void ImportTableFromMetadataPage_Validating(object sender, CancelEventArgs e)
        {
            
        }

        public List<SchemaColumn> CurrentSchema => (dataGridView1.DataSource as BindingSource)?.DataSource as List<SchemaColumn>;

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            OnValidated(new EventArgs());
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            OnValidated(new EventArgs());
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            OnValidated(new EventArgs());
        }
    }

    public static class StringHelper
    {
        public static bool ContainsAnyI(this string originalString, params string[] search)
        {
            foreach(var s in search)
            {
                if (originalString.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            }
            return false;
        }
    }

    public class SchemaColumn
    {
        private string _name;
        private string _source;

        public string Name
        {
            get => _name;
            set {
                _name = value;
                if (string.IsNullOrEmpty(_source)) _source = value;
            }
        }

        public string Source { get => _source; set => _source = value; }
        public DataType DataType { get; set; } = DataType.String;
    }
}
