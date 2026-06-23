using StoimostDveri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoimostDveri.Services
{
    // Калькулятор стоимости двери
    // Содержит бизнес-логику для расчёта цены
    class DoorCalculator
    {
        // Константы: стандартная площадь и стоимость надбавки за кв.м
        private const double StandardArea = 1.6;        // Стандартная площадь (кв.м)
        private const double SurchargePerSqm = 3000;    // Надбавка за кв.м (руб.)

        // Выполняет полный расчёт стоимости двери
        // Параметры двери
        // Результат расчёта
        public CalculationResult Calculate(DoorParameters parameters)
        {
            // ШАГ 1: Расчёт площади
            double area = parameters.GetAreaM2();
            // ШАГ 2: Расчёт дополнительной площади сверх стандарта
            double extraArea = CalculateExtraArea(area);
            // ШАГ 3: Расчёт надбавки за нестандартный размер
            double sizeSurcharge = extraArea * SurchargePerSqm;
            // ШАГ 4: Расчёт промежуточной цены (базовая + надбавка)
            double intermediatePrice = parameters.BasePrice + sizeSurcharge;
            // ШАГ 5: Расчёт финальной цены (с учётом коэффициента отделки)
            double finalPrice = intermediatePrice * parameters.FinishCoeff;

            // ШАГ 6: Возврат результата
            return new CalculationResult
            {
                Area = area,
                ExtraArea = extraArea,
                SizeSurcharge = sizeSurcharge,
                IntermediatePrice = intermediatePrice,
                FinalPrice = finalPrice,
                StandardArea = StandardArea,
                SurchargePerSqm = SurchargePerSqm
            };
        }
        
        private double CalculateExtraArea(double area)
        {
            return Math.Max(0, area - StandardArea);
        }
    }

    // Результат расчёта стоимости двери
    public class CalculationResult
    {
        // Площадь дверного проёма (кв.м)
        public double Area { get; set; }
        //Дополнительная площадь сверх стандарта (кв.м)
        public double ExtraArea { get; set; }
        //Надбавка за нестандартный размер (руб.)
        public double SizeSurcharge { get; set; }
        //Промежуточная цена (с надбавкой) (руб.)
        public double IntermediatePrice { get; set; }
        //Итоговая цена (с учётом отделки) (руб.)
        public double FinalPrice { get; set; }
        //Стандартная площадь (кв.м)
        public double StandardArea { get; set; }
        //Стоимость надбавки за 1 кв.м (руб.)
        public double SurchargePerSqm { get; set; }

        // Получение отформатированного значения площади
        public string GetAreaDisplay()
        {
            return $"{Area:F2} кв.м";
        }

        // Получение отформатированного значения надбавки
        public string GetSurchargeDisplay()
        {
            return $"{SizeSurcharge:F0} ₽ (доп. {ExtraArea:F2} кв.м × {SurchargePerSqm} ₽)";
        }

        // Получение отформатированной итоговой цены
        public string GetPriceDisplay()
        {
            return $"ИТОГО: {FinalPrice:N0} ₽";
        }
    }
}