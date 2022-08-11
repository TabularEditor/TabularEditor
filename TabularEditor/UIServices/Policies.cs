using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UIServices
{
    internal class Policies
    {
        private const string Key = "Software\\Policies\\Kapacity\\Tabular Editor";
        
        private static Policies instance;
        public static Policies Instance
        {
            get
            {
                if (instance == null) instance = new Policies();
                return instance;
            }
        }

        public Policies()
        {
            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey(Key))
                {
                    if (registryKey == null) return;
                    DisableUpdates = Read(registryKey, "DisableUpdates");
                    DisableCSharpScripts = Read(registryKey, "DisableCSharpScripts");
                    DisableMacros = Read(registryKey, "DisableMacros");
                    DisableBpaDownload = Read(registryKey, "DisableBpaDownload");
                    DisableWebDaxFormatter = Read(registryKey, "DisableWebDaxFormatter");
                }
            }
            catch { }
        }

        private bool Read(RegistryKey key, string name)
        {
            var value = key.GetValue(name);
            if (value == null) return false;
            try
            {
                var intValue = Convert.ToInt32(value);
                return intValue != 0;
            }
            catch
            {
                return false;
            }
        }

        public bool DisableUpdates { get; private set; } = false;
        public bool DisableCSharpScripts { get; private set; } = false;
        public bool DisableMacros { get; private set; } = false;
        public bool DisableBpaDownload { get; private set; } = false;
        public bool DisableWebDaxFormatter { get; private set; } = false;
    }
}
