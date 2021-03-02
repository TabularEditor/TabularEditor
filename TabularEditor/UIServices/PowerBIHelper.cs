/*
https://github.com/DaxStudio/DaxStudio/blob/master/src/DaxStudio.UI/Utils/PowerBIHelper.cs

DAX Studio - Microsoft Reciprocal License (Ms-RL)
=================================================

This license governs use of the accompanying software.  If you use the software, you accept
this license. If you do not accept the license, do not use the software.


1. Definitions
--------------

The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
meaning here as under U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.

A "contributor" is any person that distributes its contribution under this license.

"Licensed patents" are a contributor's patent claims that read directly on its contribution.


2. Grant of Rights
------------------

(A) Copyright Grant- Subject to the  terms of this license, including the license conditions
    and limitations in section 3,  each contributor  grants you a non-exclusive,  worldwide,
    royalty-free copyright license to reproduce  its contribution,  prepare derivative works
	of its contribution,  and distribute its  contribution or any  derivative works that you
	create.

(B) Patent Grant- Subject to the terms of this license, including the license conditions and
    limitations in section 3, each contributor grants you a non-exclusive,worldwide,royalty-
    free license under its licensed patents to make, have made, use,  sell,  offer for sale,
    import, and/or otherwise dispose of its contribution in the software or derivative works
	of the contribution in the software.


3. Conditions and Limitations
-----------------------------

(A) Reciprocal Grants- For any file you distribute  that contains code from the software (in
    source code or binary format),  you must provide recipients the source code to that file
    along with a copy of this license,  which license will govern that file. You may license
    other files that are  entirely your own work  and do not contain code  from the software
	under any terms you choose.

(B) No Trademark License-  This license  does not grant you  rights to use any contributors'
     name, logo, or trademarks.

(C) If you bring a  patent claim against  any contributor  over patents  that you  claim are
    infringed by the software,  your patent  license from such  contributor to  the software
	ends automatically.

(D) If you distribute any  portion of the software,  you must retain all copyright,  patent,
    trademark, and attribution notices that are present in the software.

(E) If you distribute any  portion of the software  in source code form,  you may do so only
    under this license by including a complete copy  of this license with your distribution.
	If you distribute any  portion of the software in compiled  or object code form, you may
	only do so under a license that complies with this license.

(F) The software is licensed "as-is."  You bear the risk of using it.  The contributors give
    no express warranties, guarantees or conditions. You may have additional consumer rights
	under your local laws which  this license cannot change.  To the extent  permitted under
	your local laws,  the contributors  exclude  the implied warranties  of merchantability,
	fitness for a particular purpose and non-infringement.
 
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;

namespace TabularEditor.UIServices
{
    public enum LocalInstanceType
    {
        None = 0,
        Devenv = 1,
        PowerBI = 2
    }
    public class LocalInstance
    {
        public LocalInstance(string name, int port, LocalInstanceType icon)
        {
            Port = port;
            Type = icon;
            try
            {
                var dashPos = name.LastIndexOf(" - ");
                if (dashPos >= 0)
                { Name = name.Substring(0, dashPos); }  // Strip "Power BI Designer" or "Power BI Desktop" off the end of the string
                else
                {
                    //Log.Warning("{class} {method} {message} {dashPos}", "PowerBIInstance", "ctor", "Unable to find ' - ' in Power BI title", dashPos);
                    Name = name;
                }
            }
            catch (Exception ex)
            {
                //Log.Error("{class} {method} {message} {stacktrace}", "PowerBIInstance", "ctor", ex.Message, ex.StackTrace);
                Name = name;
            }
        }
        public int Port { get; private set; }
        public string Name { get; private set; }

        public LocalInstanceType Type { get; private set; }
    }

    public class PowerBIHelper
    {

        private static List<LocalInstance> _instances = new List<LocalInstance>();
        private static bool _portSet = false;

        public static void Refresh()
        {
            _instances.Clear();

            var tcpTable = ManagedIpHelper.GetExtendedTcpTable();

            foreach (var proc in Process.GetProcessesByName("msmdsrv"))
            {
                int _port = 0;
                LocalInstanceType _icon = LocalInstanceType.PowerBI;
                var parent = proc.GetParent();

                // exit here if the parent == "services" then this is a SSAS instance
                if (parent.ProcessName.Equals("services", StringComparison.OrdinalIgnoreCase)) continue;

                // if the process was launched from Visual Studio change the icon
                if (parent.ProcessName == "devenv") _icon = LocalInstanceType.Devenv;

                // get the window title so that we can parse out the file name
                var parentTitle = parent.MainWindowTitle;
                if (parentTitle.Length == 0)
                {
                    // for minimized windows we need to use some Win32 api calls to get the title
                    parentTitle = GetWindowTitle(parent.Id);
                }

                // try and get the tcp port from the Win32 TcpTable API
                try
                {
                    var tcpRow = tcpTable.SingleOrDefault((r) => r.ProcessId == proc.Id && r.State == TcpState.Listen && IPAddress.IsLoopback(r.LocalEndPoint.Address));
                    if (tcpRow != null)
                    {
                        _port = tcpRow.LocalEndPoint.Port;
                        _portSet = true;
                        _instances.Add(new LocalInstance(parentTitle, _port, _icon));
                        //Log.Debug("{class} {method} PowerBI found on port: {port}", "PowerBIHelper", "Refresh", _port);
                    }
                    else
                    {
                        //Log.Debug("{class} {method} PowerBI port not found for process: {processName} PID: {pid}", "PowerBIHelper", "Refresh", proc.ProcessName, proc.Id);
                    }
                }
                catch (Exception ex)
                {
                    //Log.Error("{class} {Method} {Error} {StackTrace}", "PowerBIHelper", "Refresh", ex.Message, ex.StackTrace);
                }
            }

        }



        public static List<LocalInstance> Instances
        {
            get
            {
                if (!_portSet) { Refresh(); }
                return _instances;
            }
        }

        #region PInvoke calls to get the window title of a minimize window

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        private static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        private const uint WM_GETTEXT = 0x000D;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam,
            StringBuilder lParam);

        private static string GetWindowTitle(int procId)
        {
            foreach (var handle in EnumerateProcessWindowHandles(procId))
            {
                StringBuilder message = new StringBuilder(1000);
                if (IsWindowVisible(handle))
                {
                    SendMessage(handle, WM_GETTEXT, message.Capacity, message);
                    if (message.Length > 0) return message.ToString();
                }

            }
            return "";
        }



        #endregion


    }

    public static class ProcessExtensions
    {
        public static Process GetParent(this Process process)
        {
            try
            {
                using (var query = new ManagementObjectSearcher(
                  "SELECT ParentProcessId " +
                  "FROM Win32_Process " +
                  "WHERE ProcessId=" + process.Id))
                {
                    return query
                      .Get()
                      .OfType<ManagementObject>()
                      .Select(p => Process.GetProcessById((int)(uint)p["ParentProcessId"]))
                      .FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}