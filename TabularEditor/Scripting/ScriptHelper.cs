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
            if (ScriptOutputForm.DontShow) return;

            var caption = string.Format("Script output{0}", lineNumber > 0 ? " at line " + lineNumber : "");
            ScriptOutputForm.ShowObject(value, caption);
        }
    }
}
