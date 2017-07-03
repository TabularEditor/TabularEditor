using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.Scripting
{
    public static class ScriptHelper
    {
        public static void Output(this object value, int lineNumber = -1)
        {
            var caption = string.Format("Script output{0}", lineNumber > 0 ? " at line " + lineNumber : "");

            var collection = value as IEnumerable<ITabularNamedObject>;
            if(collection != null)
            {
                MessageBox.Show(string.Join("\n", collection.Select(obj => obj.Name + " (" + obj.GetTypeName() + ")")), caption);
            }
            else if (value is ITabularNamedObject)
            {
                var obj = value as ITabularNamedObject;
                MessageBox.Show(obj.Name + " (" + obj.GetTypeName() + ")", caption);
            }
            else MessageBox.Show(
                value.ToString(),
                caption
                );
        }
    }
}
