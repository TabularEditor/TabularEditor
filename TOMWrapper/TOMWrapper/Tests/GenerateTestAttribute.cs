using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Tests
{
    public class GenerateTestAttribute: Attribute
    {
        public readonly object[] TestArguments;

        public GenerateTestAttribute(params object[] testArguments)
        {
            TestArguments = testArguments;
        }
    }
}
