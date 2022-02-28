using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TabularEditor.Utils
{
    public static class Vb6
    {
        public static string Vb6Format(object expr, string format)
        {
            string result;
            int hr = VarFormat(ref expr, format, 0, 0, 0, out result);
            if (hr != 0) throw new COMException("Format error", hr);
            return result;
        }
        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
        private static extern int VarFormat(ref object expr, string format, int firstDay, int firstWeek, int flags,
            [MarshalAs(UnmanagedType.BStr)] out string result);
    }
}
