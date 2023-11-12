using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.Utils
{
    internal static class JObjectExtension
    {
        public static bool Has(this JObject obj, string propertyName, string propertyValue)
        {
            return obj[propertyName] is JValue jValue
                && jValue.Type == JTokenType.String
                && (string)jValue == propertyValue;
        }
    }
}
