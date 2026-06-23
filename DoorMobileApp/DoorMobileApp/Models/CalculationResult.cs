using System;

namespace DoorMobileApp.Models
{
    /// <summary>
    /// Результат расчёта
    /// </summary>
    public class CalculationResult
    {
        /// <summary>Площадь (кв.м)</summary>
        public double Area { get; set; }

        /// <summary>Дополнительная площадь (кв.м)</summary>
        public double ExtraArea { get; set; }

        /// <summary>Надбавка (руб.)</summary>
        public double SizeSurcharge { get; set; }

        /// <summary>Итоговая цена (руб.)</summary>
        public double FinalPrice { get; set; }

        /// <summary>Флаг успешности расчёта</summary>
        public bool IsValid { get; set; } = true;

        /// <summary>Сообщение об ошибке</summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Форматированная цена
        /// </summary>
        public string GetPriceDisplay()
        {
            return $"ИТОГО: {FinalPrice:N0} ₽";
        }

        /// <summary>
        /// Форматированная площадь
        /// </summary>
        public string GetAreaDisplay()
        {
            return $"{Area:F2} кв.м";
        }
    }
}