using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    public class AlternateOfEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            var column = context.Instance as Column;
            if (column == null) return UITypeEditorEditStyle.None;
            return column.AlternateOf == null ? UITypeEditorEditStyle.Modal : UITypeEditorEditStyle.None;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var column = context.Instance as Column;
            return column.AddAlternateOf();
        }
    }
    public class KpiEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            var measure = context.Instance as Measure;
            if (measure == null) return UITypeEditorEditStyle.None;
            return measure.KPI == null ? UITypeEditorEditStyle.Modal : UITypeEditorEditStyle.None;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var measure = context.Instance as Measure;
            return measure.AddKPI();
        }
    }
}
