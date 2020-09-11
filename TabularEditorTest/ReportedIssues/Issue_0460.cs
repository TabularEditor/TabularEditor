using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.ReportedIssues
{
    [TestClass]
    public class Issue_0460
    {
        [TestMethod]
        public void Test()
        {
            string msg = @"Deployment failed! The JSON DDL request failed with the following error: The 'Name' property cannot contain any of the following characters: . , ; ' ` : / \ * | ? "" & % $ !+ = ( )[ ] { } < >.

Technical Details:
RootActivityId: 08de469b - 3d73 - 4313 - 89ac - f014f1bd84be
Date(UTC): 5 / 2 / 2020 8:21:06 PM";

            var clHandler = new CommandLineHandler();
            clHandler.Error(msg);
            clHandler.Error(msg, new object[] { });
        }
    }
}
