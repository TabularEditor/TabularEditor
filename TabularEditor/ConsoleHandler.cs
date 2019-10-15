using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace TabularEditor
{
    internal class ConsoleHandler
    {
        internal static class Win32Native
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool AttachConsole(uint dwProcessId);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern uint GetFileType(SafeFileHandle handle);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);
        }

        private const int STDOUT_HANDLE_NAME = -11;
        private const int STDERR_HANDLE_NAME = -12;
        private const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;

        /// <summary>
        /// <see cref="System.Console.IsHandleRedirected(IntPtr ioHandle)" />
        /// </summary>
        [SecuritySafeCritical]
        private static bool IsHandleRedirected(IntPtr ioHandle)
        {
            const int FileTypeDisk = 0x0001;
            //const int FileTypeChar = 0x0002;
            const int FileTypePipe = 0x0003;
            //const int FileTypeRemote = 0x8000;
            //const int FileTypeUnknown = 0x0000;

            var handle = new SafeFileHandle(ioHandle, ownsHandle: false);

            var type = Win32Native.GetFileType(handle);
            if (type == FileTypeDisk || type == FileTypePipe)
                return true;

            //return !GetConsoleMode(ioHandle, out var num);
            return false;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void RedirectToParent(bool throwOnFailure = false)
        {
            var stdoutRedirected = IsHandleRedirected(Win32Native.GetStdHandle(STDOUT_HANDLE_NAME));
            if (stdoutRedirected)
            {
                var stdoutStream = Console.Out;
            }

            var stderrRedirected = IsHandleRedirected(Win32Native.GetStdHandle(STDERR_HANDLE_NAME));
            if (stderrRedirected)
            {
                var stderrStream = Console.Error;
            }

            if (!Win32Native.AttachConsole(ATTACH_PARENT_PROCESS) && throwOnFailure)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!stderrRedirected)
            {
                Win32Native.SetStdHandle(STDERR_HANDLE_NAME, Win32Native.GetStdHandle(STDOUT_HANDLE_NAME));
            }
        }
    }
}
