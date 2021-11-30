using System.Data.Common;

namespace TabularEditor.TOMWrapper.Utils
{
    public static class DbConnectionStringBuilderExtension
    {
        public static bool TryGet(this DbConnectionStringBuilder builder, string[] candidateKeys, out string key, out string value)
        {
            foreach (var candidateKey in candidateKeys)
            {
                value = builder.Get(candidateKey);
                if (!string.IsNullOrEmpty(value))
                {
                    key = candidateKey;
                    return true;
                }
            }
            key = null;
            value = null;
            return false;
        }

        public static string Get(this DbConnectionStringBuilder builder, string key)
        {
            if (!builder.ContainsKey(key)) return null;
            return builder[key].ToString();
        }
        public static T Get<T>(this DbConnectionStringBuilder builder, string key)
        {
            if (!builder.ContainsKey(key)) return default(T);
            return (T)builder[key];
        }
    }
}
