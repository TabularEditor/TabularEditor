using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TabularEditor.RegressionTests
{
    [TestClass]
    public class Regression_2_8_1_tests
    {

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        const UInt32 WM_KEYDOWN = 0x0100;
        const UInt32 WM_KEYUP = 0x0101;
        const int VK_DELETE = 0x2E;
        

        [TestMethod]
        public void DeleteColumnTest()
        {
            UiTester.EnsureCorrectExceptionMode();

            var fm = new FormMain();
            var ui = UI.UIController.Current;
            //fm.Show();

            ui.Database_Open(Constants.ServerName, "TomWrapperTest");
            var model = ui.Handler.Model;

            ui.Goto(model.Tables["Currency"].Columns["CurrencyName"]);
            Application.DoEvents();

            PostMessage(ui.Elements.TreeView.Handle, WM_KEYDOWN, VK_DELETE, 0);
            Thread.Sleep(100);
            PostMessage(ui.Elements.TreeView.Handle, WM_KEYUP, VK_DELETE, 0);
            Application.DoEvents();
        }
    }
}
