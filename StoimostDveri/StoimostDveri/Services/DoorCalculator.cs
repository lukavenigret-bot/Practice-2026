using StoimostDveri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoimostDveri.Services
{
    class DoorCalculator
    {
        private const double StandardArea = 1.6;        // Стандартная площадь (кв.м)
        private const double SurchargePerSqm = 3000;    // Надбавка за кв.м (руб.)

        public CalculationResult Calculate(DoorParameters parameters)
        {
            double area = parameters.GetAreaM2();
            double extraArea = CalculateExtraArea(area);
            double sizeSurcharge = extraArea * SurchargePerSqm;
            double intermediatePrice = parameters.BasePrice + sizeSurcharge;
            double finalPrice = intermediatePrice * parameters.FinishCoeff;

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

    public class CalculationResult
    {
        public double Area { get; set; }
        public double ExtraArea { get; set; }
        public double SizeSurcharge { get; set; }
        public double IntermediatePrice { get; set; }
        public double FinalPrice { get; set; }
        public double StandardArea { get; set; }
        public double SurchargePerSqm { get; set; }

        public string GetAreaDisplay()
        {
            return $"{Area:F2} кв.м";
        }

        public string GetSurchargeDisplay()
        {
            return $"{SizeSurcharge:F0} ₽ (доп. {ExtraArea:F2} кв.м × {SurchargePerSqm} ₽)";
        }

        public string GetPriceDisplay()
        {
            return $"ИТОГО: {FinalPrice:N0} ₽";
        }
    }
}