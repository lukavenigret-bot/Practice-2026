using System;
using System.Globalization;
using System.Windows.Controls;

namespace StoimostDveri.Helpers
{
    // Вспомогательные методы для работы со строками
    public static class StringHelper
    {
        // Проверяет, что строка содержит только цифры
        // <param name="text">Проверяемая строка</param>
        // <returns>true - если строка состоит только из цифр</returns>
        public static bool IsDigitsOnly(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        // Безопасный парсинг целого числа
        // <param name="text">Строка для парсинга</param>
        // <param name="defaultValue">Значение по умолчанию</param>
        // <returns>Распарсенное число или значение по умолчанию</returns>
        public static int ParseInt(string text, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;

            if (int.TryParse(text.Trim(), out int result))
                return result > 0 ? result : defaultValue;

            return defaultValue;
        }

        // Проверяет строку на null или пробелы
        public static bool IsNullOrWhiteSpace(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
    }
}