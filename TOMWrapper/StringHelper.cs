using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor
{
    public static class StringHelper
    {
        public static string ToLiteral(object input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        public static bool IsNumeric(this string value, bool allowDecimals = false)
        {
            if (allowDecimals)
            {
                decimal d;
                return decimal.TryParse(value, out d);
            }
            else
            {
                long n;
                return long.TryParse(value, out n);
            }
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
        /// </summary>
        /// <param name="str">Current instance</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace all occurrences of oldValue</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules for the search</param>
        /// <returns></returns>
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}
