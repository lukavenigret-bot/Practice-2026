using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoimostDveri.Models
{
    public class DoorParameters
    {
        public string ModelName { get; set; }
        public double BasePrice { get; set; }
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public string FinishName { get; set; }
        public double FinishCoeff { get; set; }

        public double GetAreaM2()
        {
            return (WidthMm / 1000.0) * (HeightMm / 1000.0);
        }

        public string GetSizeDescription()
        {
            return $"ширина {WidthMm:F0}мм × высота {HeightMm:F0}мм";
        }
    }
}
