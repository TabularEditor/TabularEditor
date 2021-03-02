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
        public static string Left(this string value, int chars, bool addEllipsis = false)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= chars ? value : (value.Substring(0, chars) + (addEllipsis ? "..." : ""));
        }
        public static string Right(this string value, int chars)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= chars ? value : value.Substring(value.Length - chars, chars);
        }

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

        /// <summary>
        /// Invariant Culture Ignore Case comparison between two strings.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool EqualsI(this string str, string other)
        {
            if (str == null || other == null) return str == other;
            return str.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Replace(this string value, char[] find, char replace)
        {
            var sb = new StringBuilder();
            foreach (var c in value)
            {
                if (find.Contains(c))
                {
                    sb.Append(replace);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string Replace(this string value, char[] find, char[] replace)
        {
            if (find.Length != replace.Length) throw new ArgumentException("The 'find' and 'replace' arrays have different lengths.");

            var sb = new StringBuilder();
            foreach (var c in value)
            {
                var ix = Array.IndexOf(find, c);
                if (ix >= 0)
                {
                    sb.Append(replace[ix]);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
