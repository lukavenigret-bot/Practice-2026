using System;
using System.Globalization;
using System.Windows.Controls;
using StoimostDveri.Models;

namespace StoimostDveri.Services
{
    //Фабрика для создания параметров двери из UI-элементов
    //Преобразует данные из ComboBox и TextBox в модель DoorParameters
    public class DoorDataFactory
    {
        // Создание параметров двери из элементов управления
        // </summary>
        // <param name="cmbModel">ComboBox с выбором модели</param>
        // <param name="cmbFinish">ComboBox с выбором отделки</param>
        // <param name="widthText">Текст из поля ввода ширины</param>
        // <param name="heightText">Текст из поля ввода высоты</param>
        // <returns>Объект DoorParameters с заполненными данными</returns>
        public DoorParameters CreateFromForm(ComboBox cmbModel, ComboBox cmbFinish,
                                             string widthText, string heightText)
        {
            // Получение выбранных элементов из ComboBox
            var selectedModel = cmbModel.SelectedItem as ComboBoxItem;
            var selectedFinish = cmbFinish.SelectedItem as ComboBoxItem;

            // Создание и заполнение объекта параметров
            return new DoorParameters
            {
                ModelName = ParseModelName(selectedModel?.Content?.ToString()),
                BasePrice = ParseBasePrice(selectedModel?.Tag?.ToString()),
                WidthMm = ParsePositiveDouble(widthText, 800),
                HeightMm = ParsePositiveDouble(heightText, 2000),
                FinishName = ParseFinishName(selectedFinish?.Content?.ToString()),
                FinishCoeff = ParseFinishCoeff(selectedFinish?.Tag?.ToString())
            };
        }

        // Безопасный парсинг числа с проверкой на положительность
        // <param name="text">Строка для парсинга</param>
        // <param name="defaultValue">Значение по умолчанию</param>
        // <returns>Распарсенное число или значение по умолчанию</returns>
        private double ParsePositiveDouble(string text, double defaultValue)
        {
            // Проверка на пустую строку
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;

            // Замена запятой на точку (для разных региональных настроек)
            text = text.Trim().Replace(',', '.');

            // Попытка парсинга
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result > 0 ? result : defaultValue;
            }

            return defaultValue;
        }

        // Извлечение названия модели из строки ComboBox
        // <param name="text">Строка вида "Стандартная (Classic) - 15 000 ₽"</param>
        // <returns>Название модели</returns>
        private string ParseModelName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Стандартная";

            string[] parts = text.Split('-');
            return parts[0].Trim();
        }

        // Извлечение базовой цены из Tag ComboBox
        private double ParseBasePrice(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return 15000;

            if (double.TryParse(tag, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                return price;

            return 15000;
        }

        // Извлечение названия отделки из строки ComboBox
        // <param name="text">Строка вида "Простая (1.0)"</param>
        // <returns>Название отделки</returns>
        private string ParseFinishName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Простая";

            string[] parts = text.Split('(');
            return parts[0].Trim();
        }

        // Извлечение коэффициента отделки из Tag ComboBox
        private double ParseFinishCoeff(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return 1.0;

            if (double.TryParse(tag, NumberStyles.Any, CultureInfo.InvariantCulture, out double coeff))
                return coeff;

            return 1.0;
        }
    }
}