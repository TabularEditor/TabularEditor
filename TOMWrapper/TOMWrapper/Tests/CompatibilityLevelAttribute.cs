using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.Tests
{
    public class CompatibilityLevelAttribute: Attribute
    {
        public readonly int MinimumCompatibilityLevel;

        /// <summary>
        /// Set this attribute to specify the minimum compatibilitylevel needed to use the
        /// decorated member.
        /// </summary>
        /// <param name="compatibilityLevel"></param>
        public CompatibilityLevelAttribute(int minimumCompatibilityLevel = 1200)
        {
            MinimumCompatibilityLevel = minimumCompatibilityLevel;
        }
    }
}
