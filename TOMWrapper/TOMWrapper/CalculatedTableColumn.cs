using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class CalculatedTableColumn
    {
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == Properties.ISNAMEINFERRED && ColumnOrigin != null && IsNameInferred == true)
            {
                Name = ColumnOrigin.Name;
            }
            if(propertyName == Properties.ISDATATYPEINFERRED && ColumnOrigin != null && IsDataTypeInferred == true)
            {
                DataType = ColumnOrigin.DataType;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }
    }
}
