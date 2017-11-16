using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.Scripting
{
    /// <summary>
    /// Public static methods that are decorated with this attribute, will be exposed to
    /// the scripting engine as top-level methods, so that the end-user does not need to
    /// specify the class name to invoke the method.
    /// </summary>
    public class ScriptMethodAttribute: Attribute
    {
    }
}
