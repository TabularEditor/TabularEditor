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
        public static readonly string ServerName = GetServerName();

        private static string GetServerName()
        {
            var serverName = Environment.GetEnvironmentVariable("TE_TestServer");
            if (string.IsNullOrEmpty(serverName))
                Assert.Fail("Test Server not specified. Please set environment variable TE_TestServer with the connection string of the AS engine server to use for testing");
            return serverName;
        }
    }
}
