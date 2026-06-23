using System;
using DoorMobileApp.Models;

namespace DoorMobileApp.Services
{
    /// <summary>
    /// Калькулятор стоимости двери
    /// </summary>
    public class DoorCalculator
    {
        private const double StandardArea = 1.6;
        private const double SurchargePerSqm = 3000;

        /// <summary>
        /// Расчёт стоимости двери
        /// </summary>
        public CalculationResult Calculate(DoorParameters parameters)
        {
            var result = new CalculationResult();

            try
            {
                // Расчёт площади
                double area = parameters.GetAreaM2();

                // Расчёт надбавки
                double extraArea = Math.Max(0, area - StandardArea);
                double sizeSurcharge = extraArea * SurchargePerSqm;

                // Итоговая цена
                double intermediatePrice = parameters.BasePrice + sizeSurcharge;
                double finalPrice = intermediatePrice * parameters.FinishCoeff;

                // Сохранение результата
                result.Area = area;
                result.ExtraArea = extraArea;
                result.SizeSurcharge = sizeSurcharge;
                result.FinalPrice = finalPrice;
                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Проверка параметров
        /// </summary>
        public bool ValidateParameters(DoorParameters parameters, out string error)
        {
            error = null;

            if (parameters.WidthMm <= 0)
            {
                error = "Ширина должна быть положительным числом";
                return false;
            }

            if (parameters.HeightMm <= 0)
            {
                error = "Высота должна быть положительным числом";
                return false;
            }

            if (string.IsNullOrEmpty(parameters.ModelName))
            {
                error = "Выберите модель двери";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получение базовой цены по названию модели
        /// </summary>
        public double GetBasePrice(string modelText)
        {
            if (string.IsNullOrEmpty(modelText)) return 15000;
            if (modelText.Contains("Classic") || modelText.Contains("Стандартная")) return 15000;
            if (modelText.Contains("Premium") || modelText.Contains("Премиум")) return 25000;
            if (modelText.Contains("Exclusive") || modelText.Contains("Эксклюзив")) return 40000;
            return 15000;
        }

        /// <summary>
        /// Получение коэффициента отделки
        /// </summary>
        public double GetFinishCoeff(string finishText)
        {
            if (string.IsNullOrEmpty(finishText)) return 1.0;
            if (finishText.Contains("1.0") || finishText.Contains("Простая")) return 1.0;
            if (finishText.Contains("1.15") || finishText.Contains("Средняя")) return 1.15;
            if (finishText.Contains("1.3") || finishText.Contains("Сложная")) return 1.3;
            if (finishText.Contains("1.5") || finishText.Contains("Эксклюзивная")) return 1.5;
            return 1.0;
        }
    }
}