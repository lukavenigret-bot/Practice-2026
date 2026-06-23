using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoimostDveri.Models
{
    // Модель параметров двери
    // Содержит все входные данные для расчёта стоимости
    public class DoorParameters
    {
        // Название модели двери (например, "Стандартная", "Премиум")
        public string ModelName { get; set; }
        // Базовая цена модели в рублях
        public double BasePrice { get; set; }
        // Ширина дверного проёма в миллиметрах
        public double WidthMm { get; set; }
        // Высота дверного проёма в миллиметрах
        public double HeightMm { get; set; }
        // Название типа отделки (например, "Простая", "Сложная")
        public string FinishName { get; set; }
        // Коэффициент отделки (1.0, 1.15, 1.3, 1.5)
        public double FinishCoeff { get; set; }

        // Расчёт площади дверного проёма в квадратных метрах
        // Площадь в кв.м
        // Для двери 800×2000 мм:
        // (800/1000) * (2000/1000) = 0.8 * 2.0 = 1.6 кв.м
        public double GetAreaM2()
        {
            return (WidthMm / 1000.0) * (HeightMm / 1000.0);
        }

        // Получение текстового описания размеров
        // Строка с размерами
        public string GetSizeDescription()
        {
            return $"ширина {WidthMm:F0}мм × высота {HeightMm:F0}мм";
        }
    }
}
