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

        private static byte[] entropy = { 18, 155, 3, 200, 17, 78, 163, 49 };

        private static UTF8Encoding encoder = new UTF8Encoding();

        public static string Encrypt(this string unencrypted)
        {
            if (string.IsNullOrWhiteSpace(unencrypted)) return string.Empty;
            return Convert.ToBase64String(ProtectedData.Protect(encoder.GetBytes(unencrypted), entropy, DataProtectionScope.CurrentUser));
        }

        public static string Decrypt(this string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted)) return string.Empty;
            return encoder.GetString(ProtectedData.Unprotect(Convert.FromBase64String(encrypted), entropy, DataProtectionScope.CurrentUser));
        }
    }
}
