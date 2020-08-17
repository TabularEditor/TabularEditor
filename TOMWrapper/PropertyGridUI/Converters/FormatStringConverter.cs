using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    class NumberFormatConverter: EnumConverter
    {
        public NumberFormatConverter(Type type) : base(type) { }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(
                Enum.GetValues(typeof(Format.NumberFormats))
                .OfType<Format.NumberFormats>().Where(nf => nf != Format.NumberFormats.Mixed).ToArray()
                );
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && (Format.NumberFormats)value == Format.NumberFormats.Mixed)
                return "";
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    class DateFormatConverter: StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(
                DateTimeFormatInfo.CurrentInfo.GetAllDateTimePatterns().Select(p => p.Replace("tt", "AM/PM")).ToArray()
                );
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    class Format: IDynamicPropertyObject
    {
        IFormattableObject baseObject;

        public Format(IFormattableObject baseObject)
        {
            this.baseObject = baseObject;

            // Set the format to Mixed, in case multiple objects with different formats have been selected:
            if((baseObject as FormattableObjectCollection)?.Mixed ?? false)
            {
                numberFormat = NumberFormats.Mixed;
            }
            else
                ApplyFormatString(baseObject.FormatString);
        }

        private static string GetNumberFormatString(NumberFormats format, bool useTS, int dec, int exp)
        {
            switch (format)
            {
                case NumberFormats.WholeNumber: return useTS ? "#,##0" : "0";
                case NumberFormats.DecimalNumber: return (useTS ? "#,##0" : "0") + (dec > 0 ? ("." + new string('0', dec)) : "");
                case NumberFormats.Currency: return "$ " + GetNumberFormatString(NumberFormats.DecimalNumber, useTS, dec, exp);
                case NumberFormats.Percentage: return GetNumberFormatString(NumberFormats.DecimalNumber, useTS, dec, exp) + " %";
                case NumberFormats.Scientific: return GetNumberFormatString(NumberFormats.DecimalNumber, useTS, dec, exp) + "E+" + new string('0', exp);
                default:
                    return "";
            }
        }

        public string GetFormatString()
        {
            var str = GetNumberFormatString(numberFormat, thousandSeparators, decimals, exponentDigits);
            if (parenthesisForNegative) str = string.Format("{0};({0})", str);
            return str;
        }

        public void ApplyFormatString(string value)
        {
            var ixSemicolon = value.IndexOf(';');
            var val = ixSemicolon > 0 ? value.Substring(0, ixSemicolon) : value;
            parenthesisForNegative = value == string.Format("{0};({0})", val);

            numberFormat = NumberFormats.Custom;

            if (string.IsNullOrEmpty(val))
            {
                numberFormat = NumberFormats.General;
                return;
            }

            // Check for currency:
            if (val.StartsWith("$ "))
            {
                val = val.Substring(2);
                numberFormat = NumberFormats.Currency;
            }

            // Check for scientific:
            var ixExp = val.IndexOf("E+0");
            if (numberFormat == NumberFormats.Custom && ixExp > 0 && val.Substring(ixExp + 2) == new string('0', val.Length - ixExp - 2))
            {
                exponentDigits = val.Length - ixExp - 2;
                numberFormat = NumberFormats.Scientific;
                val = val.Substring(0, ixExp);
            }

            // Check for percentage:
            if (numberFormat == NumberFormats.Custom && val.EndsWith(" %"))
            {
                numberFormat = NumberFormats.Percentage;
                val = val.Substring(0, val.Length - 2);
            }

            // Determine thousand separators:
            if (val.StartsWith("#,##0")) { thousandSeparators = true; val = val.Substring(5); }
            else if (val.StartsWith("0")) { thousandSeparators = false; val = val.Substring(1); }

            // Determine decimals:
            if (val == "")
            {
                decimals = 0;
                if (numberFormat == NumberFormats.Custom) numberFormat = NumberFormats.WholeNumber;
            }
            else if (val == "." + new string('0', val.Length - 1))
            {
                decimals = val.Length - 1;
                if (numberFormat == NumberFormats.Custom) numberFormat = NumberFormats.DecimalNumber;
            }
            else numberFormat = NumberFormats.Custom;
        }

        bool BrowsableDateTimeProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "DateFormat": return true;
                case "ExampleDate": return true;
                default:
                    return false;
            }
        }
        bool BrowsableNumberProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "NumberFormat": return true;
                case "Decimals": return numberFormat == NumberFormats.Currency || numberFormat == NumberFormats.DecimalNumber || numberFormat == NumberFormats.Percentage || numberFormat == NumberFormats.Scientific;
                case "ExponentDigits": return numberFormat == NumberFormats.Scientific;
                case "ThousandSeparators":
                case "ParenthesisForNegative":
                    return numberFormat != NumberFormats.General && numberFormat != NumberFormats.Custom && numberFormat != NumberFormats.Mixed;
                case "Example":
                    return numberFormat != NumberFormats.Mixed;
                default:
                    return false;
            }
        }

        public bool Browsable(string propertyName)
        {
            switch (baseObject.DataType)
            {
                case DataType.DateTime:
                    return BrowsableDateTimeProperty(propertyName);
                case DataType.Decimal:
                case DataType.Double:
                case DataType.Int64:
                    return BrowsableNumberProperty(propertyName);
                case DataType.Variant:
                case DataType.Unknown:
                    // If we don't know the datatype of the object, assume it is a number:
                    return BrowsableNumberProperty(propertyName);
                default:
                    return false;
            }
        }

        public bool Editable(string propertyName)
        {
            switch(propertyName)
            {
                case "Example":
                case "ExampleDate":
                    return false;
                default: return true;
            }
        }

        [DisplayName("Example")]
        public string Example
        {
            get
            {
                try
                {
                    return string.Format("{0:" + baseObject.FormatString.Replace("AMPM", "tt") + "}", -1234.567);
                }
                catch
                {
                    return string.Format("{0}", -1234.567);
                }
            }
        }

        [DisplayName("Example")]
        public string ExampleDate
        {
            get
            {
                try
                {
                    return string.Format("{0:" + baseObject.FormatString + "}", DateTime.Now);
                }
                catch
                {
                    return string.Format("{0}", DateTime.Now);
                }
            }
        }

        public enum NumberFormats
        {
            Mixed = 0,
            Custom = 1,
            General = 2,
            WholeNumber = 3,
            DecimalNumber = 4,
            Currency = 5,
            Percentage = 6,
            Scientific = 7
        }
        private NumberFormats numberFormat = NumberFormats.General;
        private int decimals = 2;
        private bool thousandSeparators = true;
        private bool parenthesisForNegative = false;
        private int exponentDigits = 1;

        [DisplayName("Format"),TypeConverter(typeof(NumberFormatConverter))]
        public NumberFormats NumberFormat
        {
            get
            {
                return numberFormat;
            }

            set
            {
                numberFormat = value;
                if (numberFormat == NumberFormats.DecimalNumber && decimals == 0) decimals = 1;

                baseObject.FormatString = GetFormatString();
            }
        }

        [DisplayName("Date Format"),TypeConverter(typeof(DateFormatConverter))]
        public string DateFormat
        {
            get
            {
                return baseObject.FormatString;
            }
            set
            {
                baseObject.FormatString = value;
            }
        }

        [DisplayName("Number of Decimals")]
        public int Decimals
        {
            get
            {
                return decimals;
            }

            set
            {
                if (value < 0 || value > 20) throw new ArgumentOutOfRangeException("Decimals", value, "Must be >= 0 and <= 20.");
                decimals = value;
                if (value == 0 && numberFormat == NumberFormats.DecimalNumber) numberFormat = NumberFormats.WholeNumber;
                baseObject.FormatString = GetFormatString();
            }
        }

        [DisplayName("Use Thousand Separators")]
        public bool ThousandSeparators
        {
            get
            {
                return thousandSeparators;
            }

            set
            {
                thousandSeparators = value;
                baseObject.FormatString = GetFormatString();
            }
        }

        [DisplayName("Use Parenthesis for Negative values")]
        public bool ParenthesisForNegative
        {
            get
            {
                return parenthesisForNegative;
            }

            set
            {
                parenthesisForNegative = value;
                baseObject.FormatString = GetFormatString();
            }
        }

        [DisplayName("Number of Exponent Digits")]
        public int ExponentDigits
        {
            get
            {
                return exponentDigits;
            }

            set
            {
                if (value < 1 || value > 5) throw new ArgumentOutOfRangeException("ExponentDigits", value, "Must be >= 1 and <= 5.");
                exponentDigits = value;
                baseObject.FormatString = GetFormatString();
            }
        }
    }

    class FormattableObjectCollection: IFormattableObject
    {
        List<IFormattableObject> selection;

        public FormattableObjectCollection(IEnumerable<IFormattableObject> selection)
        {
            this.selection = selection.ToList();
            Mixed = selection.Any() && !selection.All(obj => obj.FormatString == selection.First().FormatString);
        }

        public DataType DataType
        {
            get
            {
                var types = selection.Select(obj => obj.DataType).Distinct().ToList();
                if (types.Count == 1) return types[0];
                else return DataType.Unknown;
            }
        }

        public bool Mixed { get; private set; } = false;

        public string FormatString
        {
            get
            {
                var strings = selection.Select(obj => obj.FormatString).Distinct().ToList();
                if (strings.Count == 1) return strings[0];
                else return "";
            }
            set
            {
                TabularModelHandler.Singleton.BeginUpdate("FormatString change");
                selection.ForEach(obj => obj.FormatString = value);
                TabularModelHandler.Singleton.EndUpdate();
            }
        }
    }

    class KPIFormatString : IFormattableObject
    {
        public KPIFormatString(KPI kpi)
        {
            KPI = kpi;
        }
        public KPI KPI { get; private set; }

        public DataType DataType { get { return KPI.Measure.DataType; } }

        public string FormatString
        {
            get
            {
                return KPI.TargetFormatString;
            }

            set
            {
                KPI.TargetFormatString = value;
            }
        }
    }

    class FormatStringConverter : VirtualObjectConverter
    {
        public override object GetObject(ITypeDescriptorContext context, object value)
        {
            IFormattableObject contextObject = null;

            var multi = context.Instance as object[];
            if (multi != null)
            {
                var objects = multi.OfType<IFormattableObject>()
                    .Concat(multi.OfType<KPI>().Select(kpi => new KPIFormatString(kpi)));
                contextObject = new FormattableObjectCollection(objects);
            }
            else if (context.Instance is KPI)
            {
                contextObject = new KPIFormatString(context.Instance as KPI);
            }
            else
                contextObject = context.Instance as IFormattableObject;

            if(contextObject == null) return null;

            return new Format(contextObject);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }        
    }
}
