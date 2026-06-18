using System;
using System.Globalization;
using System.Windows.Controls;

namespace StoimostDveri.Helpers
{
    public static class StringHelper
    {
        public static bool IsDigitsOnly(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public static int ParseInt(string text, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;

            if (int.TryParse(text.Trim(), out int result))
                return result > 0 ? result : defaultValue;

            return defaultValue;
        }

        public static bool IsNullOrWhiteSpace(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
    }
}