using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UIServices
{
    public static class Crypto
    {
        public static string SHA256(string input)
        {
            var hasher = new SHA256Managed();
            var sb = new StringBuilder();

            byte[] hashedBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(input), 0, Encoding.UTF8.GetByteCount(input));

            foreach (var b in hashedBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
