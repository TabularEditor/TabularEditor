using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMWrapperTest
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

        public static string AasServerName => AasGetServerName();

        private static string AasGetServerName()
        {
            var serverName = Environment.GetEnvironmentVariable("TE_AasTestServer");
            if (string.IsNullOrEmpty(serverName))
                Assert.Inconclusive("Integration test server not specified. Set TE_AasTestServer to run Azure AS-dependent tests.");
            return serverName;
        }
    }
}
