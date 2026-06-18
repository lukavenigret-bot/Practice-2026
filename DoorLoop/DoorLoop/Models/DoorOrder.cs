using System;

namespace DoorLoop.Models
{
    public class DoorOrder
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Finish { get; set; }
        public double FinishCoeff { get; set; }

        // Цены
        public double BasePrice { get; set; }                          // Базовая цена модели
        public double SizeSurchargePercent { get; set; }               // Наценка за размер %
        public double SizeSurchargeAmount { get; set; }                // Наценка за размер в рублях
        public double PriceWithSize { get; set; }                      // Цена с наценкой за размер
        public double PriceWithFinish { get; set; }                    // Цена с учётом отделки

        // Скидки
        public double LoyaltyDiscountPercent { get; set; }             // Скидка по карте %
        public double LoyaltyDiscountAmount { get; set; }              // Скидка по карте в рублях
        public double QuantityDiscountPercent { get; set; }            // Оптовая скидка %
        public double QuantityDiscountAmount { get; set; }             // Оптовая скидка в рублях
        public double SeasonalDiscountPercent { get; set; }            // Сезонная скидка %
        public double SeasonalDiscountAmount { get; set; }             // Сезонная скидка в рублях
        public double TotalDiscountPercent { get; set; }               // Общая скидка %
        public double TotalDiscountAmount { get; set; }                // Общая скидка в рублях

        public double FinalPrice { get; set; }                         // Итоговая цена
        public DateTime CreatedAt { get; set; }
        public bool IsSeasonalActive { get; set; }                     // Активна ли сезонная скидка

        public override string ToString()
        {
            return $"#{Id} | {Model} | {Width}×{Height}мм | {Finish} | {FinalPrice:N0} ₽";
        }

        public string GetShortDetails()
        {
            return $"#{Id}: {Model}, {Width}×{Height}мм, {FinalPrice:N0} ₽";
        }

        public string GetFullDetails()
        {
            return
                $"╔═══════════════════════════════════════════════════════════╗\n" +
                $"║  ДВЕРЬ #{Id}                                           \n" +
                $"╠═══════════════════════════════════════════════════════════╣\n" +
                $"║  📋 ПАРАМЕТРЫ                                            \n" +
                $"║  • Модель: {Model}                                        \n" +
                $"║  • Размер: {Width}×{Height} мм                            \n" +
                $"║  • Отделка: {Finish} (×{FinishCoeff:F2})                   \n" +
                $"╠═══════════════════════════════════════════════════════════╣\n" +
                $"║  💰 РАСЧЁТ ЦЕНЫ                                           \n" +
                $"║  • Базовая цена: {BasePrice:N0} ₽                          \n" +
                $"║  • Наценка за размер: {SizeSurchargePercent}% ({SizeSurchargeAmount:N0} ₽)   \n" +
                $"║  • Цена с наценкой: {PriceWithSize:N0} ₽                   \n" +
                $"║  • Коэффициент отделки: ×{FinishCoeff:F2}                  \n" +
                $"║  • Цена с отделкой: {PriceWithFinish:N0} ₽                 \n" +
                $"╠═══════════════════════════════════════════════════════════╣\n" +
                $"║  🎯 СКИДКИ                                                \n" +
                $"║  • Карта лояльности: {LoyaltyDiscountPercent}% ({LoyaltyDiscountAmount:N0} ₽)       \n" +
                $"║  • Оптовая скидка: {QuantityDiscountPercent}% ({QuantityDiscountAmount:N0} ₽)        \n" +
                $"║  • Сезонная скидка: {SeasonalDiscountPercent}% ({SeasonalDiscountAmount:N0} ₽){(IsSeasonalActive ? " ✅ АКТИВНА" : " ❌ НЕАКТИВНА")} \n" +
                $"║  ────────────────────────────────────────────────────────  \n" +
                $"║  • Общая скидка: {TotalDiscountPercent}% ({TotalDiscountAmount:N0} ₽)         \n" +
                $"╠═══════════════════════════════════════════════════════════╣\n" +
                $"║  ✅ ИТОГО: {FinalPrice:N0} ₽                                  \n" +
                $"╚═══════════════════════════════════════════════════════════╝";
        }
    }
}