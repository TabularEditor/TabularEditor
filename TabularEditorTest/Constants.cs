using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor
{
    public static class Constants
    {
        public static string ServerName => GetServerName();

        private static string GetServerName()
        {
            var serverName = Environment.GetEnvironmentVariable("TE_TestServer");
            if (string.IsNullOrEmpty(serverName))
                Assert.Inconclusive("Integration test server not specified. Set TE_TestServer to run server-dependent tests.");
            return serverName;
        }
    }
}
