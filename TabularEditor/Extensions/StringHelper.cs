using System;

namespace System
{
    public static class StringHelper
    {
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
    }

}
