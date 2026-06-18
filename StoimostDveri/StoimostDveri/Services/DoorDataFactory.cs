using System;
using System.Globalization;
using System.Windows.Controls;
using StoimostDveri.Models;

namespace StoimostDveri.Services
{
    public class DoorDataFactory
    {
        public DoorParameters CreateFromForm(ComboBox cmbModel, ComboBox cmbFinish,
                                             string widthText, string heightText)
        {
            var selectedModel = cmbModel.SelectedItem as ComboBoxItem;
            var selectedFinish = cmbFinish.SelectedItem as ComboBoxItem;

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

        private double ParsePositiveDouble(string text, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;

            text = text.Trim().Replace(',', '.');

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result > 0 ? result : defaultValue;
            }

            return defaultValue;
        }

        private string ParseModelName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Стандартная";

            string[] parts = text.Split('-');
            return parts[0].Trim();
        }

        private double ParseBasePrice(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return 15000;

            if (double.TryParse(tag, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                return price;

            return 15000;
        }

        private string ParseFinishName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Простая";

            string[] parts = text.Split('(');
            return parts[0].Trim();
        }

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