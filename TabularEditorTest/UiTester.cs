using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor
{
    public static class UiTester
    {
        private static bool exceptionModeSet = false;
        private static readonly Object lockObj = new Object();
        public static void EnsureCorrectExceptionMode()
        {
            lock (lockObj)
            {
                if (exceptionModeSet) return;
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                exceptionModeSet = true;
            }
        }
    }
}
