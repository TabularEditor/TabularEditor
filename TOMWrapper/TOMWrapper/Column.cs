using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace TabularEditor.TOMWrapper
{
    public abstract partial class Column: ITabularPerspectiveObject, IDaxObject
    {
        [Browsable(true),DisplayName("Perspectives"), Category("Translations and Perspectives")]
        public PerspectiveIndexer InPerspective { get; private set; }

        protected override void Init()
        {
            InPerspective = new PerspectiveColumnIndexer(this);
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if(propertyName == "IsKey" && (bool)newValue == true)
            {
                // When the IsKey column is set to "true", all other columns must have their IsKey set to false.
                // This has to happen within one undo-batch, so the change can be perfectly restored.
                Handler.UndoManager.BeginBatch("key column");
                foreach(var col in Table.Columns.Where(c => c.IsKey))
                {
                    col.IsKey = false;
                }
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == "IsKey" && IsKey == true)
            {
                Handler.UndoManager.EndBatch();
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        [Browsable(true), Category("Metadata"), DisplayName("DAX identifier")]
        public string DaxObjectFullName
        {
            get
            {
                return string.Format("{0}[{1}]", DaxTableName, DaxObjectName);
            }
        }

        [Browsable(false)]
        public string DaxObjectName
        {
            get { return Name.Replace("]", "]]"); }
        }

        [Browsable(false)]
        public string DaxTableName
        {
            get { return Table.DaxTableName; }
        }

    }

    public partial class ColumnCollection
    {
        public override IEnumerator<Column> GetEnumerator()
        {
            return MetadataObjectCollection.Where(c => c.Type != Microsoft.AnalysisServices.Tabular.ColumnType.RowNumber).Select(c => Handler.WrapperLookup[c] as Column).GetEnumerator();
        }
    }

    public class ColumnConverter: TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context.Instance is ITabularTableObject || context.Instance is ITabularNamedObject[];
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if(context.Instance is ITabularNamedObject[])
            {
                var cols = (context.Instance as ITabularNamedObject[]).Cast<Column>();
                return new StandardValuesCollection(Table(context).Columns.Where(c => !cols.Contains(c)).OrderBy(c => c.Name).ToList());
            } else
            {
                var col = (context.Instance as Column);
                return new StandardValuesCollection(Table(context).Columns.Where(c => c != col).OrderBy(c => c.Name).ToList());
            }
        }

        private Table Table(ITypeDescriptorContext context)
        {
            if (context.Instance is ITabularTableObject) return (context.Instance as ITabularTableObject).Table;
            if (context.Instance is ITabularNamedObject[]) return ((context.Instance as ITabularNamedObject[]).First() as Column)?.Table;
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                var name = (string)value;
                if (string.IsNullOrEmpty(name)) return null;

                var table = Table(context);
                if (!table.Columns.Contains(name)) throw new ArgumentException(string.Format("The table does not contain a column named \"{0}\"", name), context.PropertyDescriptor.Name);

                return table.Columns[name];
            }
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (value as Column)?.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
