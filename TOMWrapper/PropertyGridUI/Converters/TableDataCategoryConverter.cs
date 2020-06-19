using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.PropertyGridUI.Converters
{
    internal class TableDataCategoryConverter: StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetNames(typeof(TableDataCategory)));
        }
    }


    public enum TableDataCategory
    {
        Unknown = 0,
        Regular = 1,
        Time = 2,
        Geography = 3,
        Organization = 4,
        BillOfMaterials = 5,
        Accounts = 6,
        Customers = 7,
        Products = 8,
        Scenario = 9,
        Quantitative = 10,
        Utility = 11,
        Currency = 12,
        Rates = 13,
        Channel = 14,
        Promotion = 15
    }
}
