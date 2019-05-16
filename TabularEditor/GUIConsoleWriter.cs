using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TabularEditor
{
    // This always writes to the parent console window and also to a redirected stdout if there is one.
    // It would be better to do the relevant thing (eg write to the redirected file if there is one, otherwise
    // write to the console) but it doesn't seem possible.
    public class GUIConsoleWriter
    {
        const uint WM_CHAR = 0x0102;
        const int VK_ENTER = 0x0D;

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        StreamWriter _stdOutWriter;

        IntPtr cw = (IntPtr)0;

        // this must be called early in the program
        public GUIConsoleWriter()
        {
            // this needs to happen before attachconsole.
            // If the output is not redirected we still get a valid stream but it doesn't appear to write anywhere
            // I guess it probably does write somewhere, but nowhere I can find out about
            var stdout = Console.OpenStandardOutput();
            _stdOutWriter = new StreamWriter(stdout);
            _stdOutWriter.AutoFlush = true;

            AttachConsole(ATTACH_PARENT_PROCESS);

            cw = GetConsoleWindow();
        }

        public void WriteLine(string line)
        {
            //_stdOutWriter.WriteLine(line);
            Console.WriteLine(line);
        }

        public void WriteLine(string line, params object[] args)
        {
            if (args == null || args.Length == 0)
                WriteLine(line);
            else
                WriteLine(string.Format(line, args));
        }

        public void DettachConsole()
        {
            if ((int)cw != 0)
            {
                SendMessage(cw, WM_CHAR, (IntPtr)VK_ENTER, IntPtr.Zero);
                FreeConsole();
            }
        }
    }
}
