using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public static class TabularConnection
    {
        public static string GetConnectionString(string serverName)
        {
            return string.Format("Provider=MSOLAP;DataSource={0}", serverName);
        }

        public static string GetConnectionString(string serverName, string userName, string password)
        {
            return string.Format("Provider=MSOLAP;DataSource={0};User ID={1};Password=\"{2}\";Persist Security Info=True;Impersonation Level=Impersonate;",
                serverName,
                userName,
                password.Replace("\"", "\"\""));
        }
    }
}
