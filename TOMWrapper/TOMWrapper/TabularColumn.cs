using System;
using System.Linq;
using Microsoft.AnalysisServices.Tabular;
using System.ComponentModel;

namespace TabularEditor.TOMWrapper
{
    public class TabularColumn : TabularTableObject, IDropDownProperties
    {
        #region Public properties
        public override Table Table { get { return Column.Table; } }
        [Browsable(false)]
        public virtual Column Column { get { return MetadataObject as Column; } }
        public override int Icon { get { return TabularIcons.ICON_COLUMN; } }
        public override TabularObjectType Type { get { return TabularObjectType.Column; } }
        #endregion

        #region Public methods
        public override string GetTooltipText()
        {
            return Column.ErrorMessage.Replace("\n", "");
        }
        public override string GetDisplayFolder(Culture culture)
        {
            return Column.GetDisplayFolder(culture);
        }
        public override void SetDisplayFolder(string folder, Culture culture)
        {
            Column.SetDisplayFolder(folder, culture);
        }
        public override bool InPerspective(Perspective perspective)
        {
            return Column.InPerspective(perspective);
        }
        public override void SetPerspective(string perspectiveName, bool inPerspective)
        {
            Column.SetPerspective(perspectiveName, inPerspective);
        }

        public override void Clone(string newName, bool includeTranslations) { throw new InvalidOperationException("Cannot clone column."); }
        public override void Delete() { throw new InvalidOperationException("Cannot delete column."); }

        public string[] GetDropDownItems(string propertyName)
        {
            switch (propertyName)
            {
                case "SortByColumn":
                    return Table.Columns.Where(c => c != Column && c.Type != ColumnType.RowNumber)
                        .Select(c => c.Name).OrderBy(n => n, StringComparer.InvariantCultureIgnoreCase).ToArray();
            }
            return null;
        }
        #endregion

        #region Editable properties
        public bool ShouldSerializeSortByColumn() { return false; }
        [NoCultureBrowsable, Category("Options"), DisplayName("Sort By Column"), TypeConverter(typeof(ColumnSelectConverter))]
        [Description("Sort the selected column by a different column from the list.")]
        public string SortByColumn { get { return Column.SortByColumn?.Name; } set { Column.SortByColumn = Column.Table.Columns[value]; } }

        public override bool Visible { get { return !Column.IsHidden; } set { Column.IsHidden = !value; } }

        public bool ShouldSerializeDefaultImage() { return false; }
        [NoCultureBrowsable, Category("Reporting Properties"), DisplayName("Default Image"), MultiSelectBrowsable]
        [Description("Display the image in this column as a representation for each row of data in the table in the reporting client tool. Only one column can be selected per table. Example: Product Photo.")]
        public bool DefaultImage
        {
            get { return Column.IsDefaultImage; }
            set
            {
                if (value)
                {
                    Table.Columns.Where(c => c.IsDefaultImage && c.Type != ColumnType.RowNumber).ToList().ForEach(c => c.IsDefaultImage = false);
                }
                Column.IsDefaultImage = value;
            }
        }

        public bool ShouldSerializeDefaultLabel() { return false; }
        [NoCultureBrowsable, Category("Reporting Properties"), DisplayName("Default Label"), MultiSelectBrowsable]
        [Description("Display the value in this column as a label for each row of data in the table in the reporting client tool. Only one column can be selected per table. Example: Product Name.")]
        public bool DefaultLabel
        {
            get { return Column.IsDefaultLabel; }
            set
            {
                if (value)
                {
                    Table.Columns.Where(c => c.IsDefaultLabel && c.Type != ColumnType.RowNumber).ToList().ForEach(c => c.IsDefaultLabel = false);
                }
                Column.IsDefaultLabel = value;
            }
        }

        public bool ShouldSerializeIsKey() { return false; }
        [NoCultureBrowsable, Category("Reporting Properties"), DisplayName("Row Identifier")]
        [Description("Use the values in this column as unique identifiers for each row of data in the table. Only one column can be selected per table. Example: Customer ID.")]
        public bool IsKey
        {
            get { return Column.IsKey; }
            set
            {
                if (value)
                {
                    // Remove any existing key columns on the table:
                    Table.Columns.Where(c => c.IsKey && c.Type != ColumnType.RowNumber).ToList().ForEach(c => c.IsKey = false);
                }
                Column.IsKey = value;
            }
        }

        public bool ShouldSerializeDataCategory() { return false; }
        [NoCultureBrowsable, Category("Reporting Properties"), DisplayName("Data Category"), MultiSelectBrowsable]
        [Description("The data category of the data in the column")]
        public DataCategory DataCategory
        {
            get
            {
                var result = DataCategory.Uncategorized;
                Enum.TryParse(Column.DataCategory, out result);
                return result;
            }
            set
            {
                Column.DataCategory = value == DataCategory.Uncategorized ? null : value.ToString();
            }
        }

        public bool ShouldSerializeSummarizeBy() { return false; }
        [NoCultureBrowsable, Category("Reporting Properties"), DisplayName("Summarize By"), MultiSelectBrowsable]
        [Description("By default, reporting client tools such as PowerPivot apply the aggregate function Sum for column calculations when this column is added to a Field List area. To change the default calculation, select the appropriate function from the drop-down list. Example: Average")]
        public AggregateFunction SummarizeBy { get { return Column.SummarizeBy; } set { Column.SummarizeBy = value; } }

        public bool ShouldSerializeKeepUniqueRows() { return false; }
        [NoCultureBrowsable, Category("Reporting Properties"), DisplayName("Keep Unique Rows"), MultiSelectBrowsable]
        [Description("Group the values of this column based on values in the Table Identifier column. Set this property to prevent row aggregation on duplicate values. Example: Customer Name.")]
        public bool KeepUniqueRows { get { return Column.KeepUniqueRows; } set { Column.KeepUniqueRows = value; } }

        public bool ShouldSerializeDataType() { return false; }
        [NoCultureBrowsable, Category("Options"), DisplayName("Data Type"), MultiSelectBrowsable]
        [Description("The type of column")]
        public DataType DataType { get { return Column.DataType; } set { Column.DataType = value; } }

        // TODO: Provide defaults in a drop-down
        public bool ShouldSerializeFormatString() { return false; }
        [NoCultureBrowsable, Category("Options"), DisplayName("Format String"), MultiSelectBrowsable]
        [Description("The display format of the data in the column.")]
        public string FormatString { get { return Column.FormatString; } set { Column.FormatString = value; } }

        public bool ShouldSerializeIsNullable() { return false; }
        [NoCultureBrowsable, Category("Options"), DisplayName("Nullable"), MultiSelectBrowsable]
        [Description("This column can contain null values, if they exist in the underlying data source.")]
        public bool IsNullable { get { return Column.IsNullable; } set { Column.IsNullable = value; } }

        public bool ShouldSerializeIsUnique() { return false; }
        [NoCultureBrowsable, Category("Options"), DisplayName("Unique"), MultiSelectBrowsable]
        [Description("This column can contain only unique values in the underlying data source.")]
        public bool IsUnique { get { return Column.IsUnique; } set { Column.IsUnique = value; } }

        #endregion
    }

}
