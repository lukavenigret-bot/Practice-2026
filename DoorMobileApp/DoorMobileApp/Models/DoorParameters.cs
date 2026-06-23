using System;

namespace DoorMobileApp.Models
{
    /// <summary>
    /// Модель параметров двери
    /// </summary>
    public class DoorParameters
    {
        /// <summary>
        /// Название модели
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Базовая цена (руб.)
        /// </summary>
        public double BasePrice { get; set; }

        /// <summary>
        /// Ширина (мм)
        /// </summary>
        public double WidthMm { get; set; }

        /// <summary>
        /// Высота (мм)
        /// </summary>
        public double HeightMm { get; set; }

        /// <summary>
        /// Название отделки
        /// </summary>
        public string FinishName { get; set; }

        /// <summary>
        /// Коэффициент отделки
        /// </summary>
        public double FinishCoeff { get; set; }

        /// <summary>
        /// Расчёт площади (кв.м)
        /// </summary>
        public double GetAreaM2()
        {
            return (WidthMm / 1000.0) * (HeightMm / 1000.0);
        }

        /// <summary>
        /// Описание размеров
        /// </summary>
        public string GetSizeDescription()
        {
            return $"{WidthMm:F0}×{HeightMm:F0} мм";
        }
    }
}