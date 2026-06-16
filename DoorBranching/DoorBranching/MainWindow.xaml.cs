using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoorBranching
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btnCalculate.Click += BtnCalculate_Click;
            btnClear.Click += BtnClear_Click;

            txtWidth.KeyDown += TxtInput_KeyDown;
            txtHeight.KeyDown += TxtInput_KeyDown;

            txtWidth.PreviewTextInput += TxtInput_PreviewTextInput;
            txtHeight.PreviewTextInput += TxtInput_PreviewTextInput;

            txtWidth.LostFocus += TxtInput_LostFocus;
            txtHeight.LostFocus += TxtInput_LostFocus;

            CheckSeasonalDiscount();
        }

        private void CheckSeasonalDiscount()
        {
            int currentMonth = DateTime.Now.Month;
            if (currentMonth == 6) // Июнь
            {
                lblSeasonalDiscount.Text = "🌞 Летняя распродажа: скидка 7% (АКТИВНА!)";
                lblSeasonalDiscount.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                lblSeasonalDiscount.Text = $"🌞 Летняя распродажа: скидка 7% (активна в июне)";
                lblSeasonalDiscount.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Выполняется расчёт...", "#FF9800");

            try
            {

                ComboBoxItem selectedModel = cmbModel.SelectedItem as ComboBoxItem;
                double basePrice = ParseNumber(selectedModel.Tag.ToString());
                string modelName = selectedModel.Content.ToString().Split('(')[0].Trim();

                double widthMm = ParseNumber(txtWidth.Text);
                double heightMm = ParseNumber(txtHeight.Text);

                if (widthMm <= 0 || heightMm <= 0)
                {
                    MessageBox.Show("Размеры должны быть положительными числами!",
                                  "Ошибка ввода",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                ComboBoxItem selectedFinish = cmbFinish.SelectedItem as ComboBoxItem;
                double finishCoeff = ParseNumber(selectedFinish.Tag.ToString());
                string finishName = selectedFinish.Content.ToString().Split('(')[0].Trim();

                ComboBoxItem selectedQuantity = cmbQuantity.SelectedItem as ComboBoxItem;
                int quantity = int.Parse(selectedQuantity.Tag.ToString());
                string quantityText = selectedQuantity.Content.ToString();

                bool hasLoyaltyCard = chkLoyaltyCard.IsChecked ?? false;


                double sizeSurchargePercent = 0;
                string sizeCategory = "";

                bool isWidthStandard = (widthMm >= 800 && widthMm <= 900);
                bool isHeightStandard = (heightMm >= 2000 && heightMm <= 2100);
                bool isWidthSmallDeviation = (widthMm >= 700 && widthMm < 800) || (widthMm > 900 && widthMm <= 1000);
                bool isHeightSmallDeviation = (heightMm >= 1900 && heightMm < 2000) || (heightMm > 2100 && heightMm <= 2200);
                bool isWidthLargeDeviation = (widthMm < 700 || widthMm > 1000);
                bool isHeightLargeDeviation = (heightMm < 1900 || heightMm > 2200);

                if (isWidthStandard && isHeightStandard)
                {
                    sizeSurchargePercent = 0;
                    sizeCategory = "✅ Стандартный размер (без наценки)";
                }
                else if ((isWidthSmallDeviation && isHeightStandard) ||
                         (isWidthStandard && isHeightSmallDeviation) ||
                         (isWidthSmallDeviation && isHeightSmallDeviation))
                {
                    sizeSurchargePercent = 10;
                    sizeCategory = "⚠️ Небольшое отклонение (+10%)";
                }
                else if (isWidthLargeDeviation || isHeightLargeDeviation)
                {
                    sizeSurchargePercent = 25;
                    sizeCategory = "❌ Индивидуальный размер (+25%)";
                }
                else
                {
                    sizeSurchargePercent = 25;
                    sizeCategory = "❌ Индивидуальный размер (+25%)";
                }

                double priceWithSize = basePrice * (1 + sizeSurchargePercent / 100);
                double sizeSurchargeAmount = basePrice * (sizeSurchargePercent / 100);

                double priceWithFinish = priceWithSize * finishCoeff;

                double loyaltyDiscountPercent = 0;
                double quantityDiscountPercent = 0;
                double seasonalDiscountPercent = 0;

                if (hasLoyaltyCard)
                {
                    loyaltyDiscountPercent = 5;
                }

                if (quantity >= 3 && quantity <= 4)
                {
                    quantityDiscountPercent = 10;
                }
                else if (quantity >= 5)
                {
                    quantityDiscountPercent = 15;
                }

                int currentMonth = DateTime.Now.Month;
                if (currentMonth == 6)
                {
                    seasonalDiscountPercent = 7;
                }

                double totalDiscountPercent = loyaltyDiscountPercent + quantityDiscountPercent + seasonalDiscountPercent;

                if (totalDiscountPercent > 30)
                {
                    totalDiscountPercent = 30;
                    UpdateStatus("⚠️ Максимальная скидка ограничена 30%", "#E85D04");
                }

                double discountAmountPerDoor = priceWithFinish * (totalDiscountPercent / 100);
                double finalPricePerDoor = priceWithFinish - discountAmountPerDoor;

                double totalFinalPrice = finalPricePerDoor * quantity;
                double totalDiscountAmount = discountAmountPerDoor * quantity;
                double totalBasePrice = basePrice * quantity;



                lblBasePrice.Text = $"{basePrice:N0} ₽ (за 1 дверь)";
                lblSizeSurcharge.Text = $"{sizeSurchargePercent}% ({sizeSurchargeAmount:N0} ₽)";
                lblSizeCategory.Text = sizeCategory;
                lblFinish.Text = $"{finishName} (×{finishCoeff:F2})";

                lblLoyaltyDiscount.Text = hasLoyaltyCard ? $"-{loyaltyDiscountPercent}% (есть карта)" : "0% (нет карты)";
                lblQuantityDiscount.Text = quantity >= 3 ? $"-{quantityDiscountPercent}% ({quantity} двери)" : "0%";
                lblSeasonalDiscountValue.Text = seasonalDiscountPercent > 0 ? $"-{seasonalDiscountPercent}% (июнь!)" : "0%";
                lblTotalDiscount.Text = $"-{totalDiscountPercent:F0}% ({totalDiscountAmount:N0} ₽ за {quantity} шт.)";

                if (quantity > 1)
                {
                    lblFinalPrice.Text = $"ИТОГО: {totalFinalPrice:N0} ₽ (за {quantity} двери)\n{finalPricePerDoor:N0} ₽ за дверь";
                }
                else
                {
                    lblFinalPrice.Text = $"ИТОГО: {finalPricePerDoor:N0} ₽";
                }

                lblDetails.Text =
                    $"📊 Детальный расчёт:\n" +
                    $"• Базовая цена: {basePrice:N0} ₽ × {quantity} шт. = {totalBasePrice:N0} ₽\n" +
                    $"• Наценка за размер: {sizeSurchargePercent}% ({sizeSurchargeAmount:N0} ₽)\n" +
                    $"• Цена с наценкой: {priceWithSize:N0} ₽\n" +
                    $"• Коэффициент отделки: ×{finishCoeff:F2} → {priceWithFinish:N0} ₽\n" +
                    $"• Скидка: {totalDiscountPercent:F0}% ({discountAmountPerDoor:N0} ₽/шт × {quantity} = {totalDiscountAmount:N0} ₽)\n" +
                    $"• Цена за 1 дверь со скидкой: {finalPricePerDoor:N0} ₽\n" +
                    $"• Итоговая цена за {quantity} дверей: {totalFinalPrice:N0} ₽";

                UpdateStatus($"✅ Расчёт выполнен. Цена за 1 дверь: {finalPricePerDoor:N0} ₽ | За {quantity}: {totalFinalPrice:N0} ₽", "#4CAF50");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}", "#F44336");
                MessageBox.Show($"Произошла ошибка при расчёте:\n{ex.Message}\n\n" +
                              "Пожалуйста, проверьте введённые данные.",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtWidth.Text = "800";
            txtHeight.Text = "2000";
            cmbModel.SelectedIndex = 0;
            cmbFinish.SelectedIndex = 0;
            cmbQuantity.SelectedIndex = 0;
            chkLoyaltyCard.IsChecked = false;

            lblBasePrice.Text = "—";
            lblSizeSurcharge.Text = "—";
            lblSizeCategory.Text = "Размер: стандартный";
            lblFinish.Text = "—";
            lblLoyaltyDiscount.Text = "—";
            lblQuantityDiscount.Text = "—";
            lblSeasonalDiscountValue.Text = "—";
            lblTotalDiscount.Text = "—";
            lblFinalPrice.Text = "ИТОГО: 0 ₽";
            lblDetails.Text = "Нажмите «Рассчитать» для подробного расчёта";

            UpdateStatus("Очищено. Готов к расчёту.", "#666666");
        }

        private double ParseNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            text = text.Trim().Replace(',', '.');

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;

            return 0;
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnCalculate_Click(sender, e);
            }
        }

        private void TxtInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != '.' && e.Text[0] != ',')
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;
            if (textBox != null && (e.Text[0] == '.' || e.Text[0] == ','))
            {
                if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                {
                    e.Handled = true;
                }
            }
        }

        private void TxtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "0";
                }
            }
        }

        private void UpdateStatus(string message, string colorHex = "#666666")
        {
            lblStatus.Text = message;

            if (colorHex.StartsWith("#"))
            {
                var converter = new System.Windows.Media.BrushConverter();
                lblStatus.Foreground = (System.Windows.Media.Brush)converter.ConvertFromString(colorHex);
            }
        }
    }
}