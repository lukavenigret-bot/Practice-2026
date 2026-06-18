using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DoorLoop.Models;

namespace DoorLoop
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<DoorOrder> orders = new ObservableCollection<DoorOrder>();
        private int nextId = 1;
        private bool isSeasonalActive;

        public MainWindow()
        {
            InitializeComponent();

            lstOrders.ItemsSource = orders;

            CheckSeasonalDiscount();

            txtWidth.KeyDown += TxtInput_KeyDown;
            txtHeight.KeyDown += TxtInput_KeyDown;
            txtCount.KeyDown += TxtInput_KeyDown;

            txtWidth.PreviewTextInput += TxtInput_PreviewTextInput;
            txtHeight.PreviewTextInput += TxtInput_PreviewTextInput;
            txtCount.PreviewTextInput += TxtInput_PreviewTextInput;

            txtWidth.LostFocus += TxtInput_LostFocus;
            txtHeight.LostFocus += TxtInput_LostFocus;
            txtCount.LostFocus += TxtInput_LostFocus;

            UpdateTotals();
        }


        private void CheckSeasonalDiscount()
        {
            int currentMonth = DateTime.Now.Month;

            if (currentMonth == 6)
            {
                isSeasonalActive = true;
                lblSeasonalStatus.Text = "🌞 Летняя распродажа: скидка 7% (АКТИВНА!)";
                lblSeasonalStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
                lblSeasonalInfo.Text = "🌞 Летняя распродажа: скидка 7% (АКТИВНА в июне!)";
                lblSeasonalInfo.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                isSeasonalActive = false;
                lblSeasonalStatus.Text = $"ООО ZaDoor — Циклический расчёт (летняя скидка неактивна)";
                lblSeasonalStatus.Foreground = System.Windows.Media.Brushes.Gray;
                lblSeasonalInfo.Text = $"❄️ Летняя распродажа: скидка 7% (неактивна, сейчас {GetMonthName(currentMonth)})";
                lblSeasonalInfo.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private string GetMonthName(int month)
        {
            string[] months = { "январь", "февраль", "март", "апрель", "май", "июнь",
                               "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
            return months[month - 1];
        }


        private DoorOrder CreateOrderFromForm()
        {
            ComboBoxItem selectedModel = cmbModel.SelectedItem as ComboBoxItem;
            double basePrice = ParseNumber(selectedModel.Tag.ToString());
            string modelName = selectedModel.Content.ToString().Split('(')[0].Trim();

            double widthMm = ParseNumber(txtWidth.Text);
            double heightMm = ParseNumber(txtHeight.Text);

            ComboBoxItem selectedFinish = cmbFinish.SelectedItem as ComboBoxItem;
            double finishCoeff = ParseNumber(selectedFinish.Tag.ToString());
            string finishName = selectedFinish.Content.ToString().Split('(')[0].Trim();

            bool hasLoyaltyCard = chkLoyaltyCard.IsChecked ?? false;

            ComboBoxItem selectedQuantity = cmbQuantity.SelectedItem as ComboBoxItem;
            int quantity = int.Parse(selectedQuantity.Tag.ToString());

            double sizeSurchargePercent = CalculateSizeSurcharge(widthMm, heightMm);
            double sizeSurchargeAmount = basePrice * (sizeSurchargePercent / 100);
            double priceWithSize = basePrice + sizeSurchargeAmount;

            double priceWithFinish = priceWithSize * finishCoeff;

            double loyaltyDiscountPercent = hasLoyaltyCard ? 5 : 0;

            double quantityDiscountPercent = 0;
            if (quantity >= 3 && quantity <= 4)
                quantityDiscountPercent = 10;
            else if (quantity >= 5)
                quantityDiscountPercent = 15;

            double seasonalDiscountPercent = isSeasonalActive ? 7 : 0;

            double totalDiscountPercent = loyaltyDiscountPercent + quantityDiscountPercent + seasonalDiscountPercent;
            if (totalDiscountPercent > 30)
                totalDiscountPercent = 30;

            double loyaltyDiscountAmount = priceWithFinish * (loyaltyDiscountPercent / 100);
            double quantityDiscountAmount = priceWithFinish * (quantityDiscountPercent / 100);
            double seasonalDiscountAmount = priceWithFinish * (seasonalDiscountPercent / 100);
            double totalDiscountAmount = priceWithFinish * (totalDiscountPercent / 100);

            double finalPrice = priceWithFinish - totalDiscountAmount;

            return new DoorOrder
            {
                Id = nextId++,
                Model = modelName,
                Width = widthMm,
                Height = heightMm,
                Finish = finishName,
                FinishCoeff = finishCoeff,
                BasePrice = basePrice,
                SizeSurchargePercent = sizeSurchargePercent,
                SizeSurchargeAmount = sizeSurchargeAmount,
                PriceWithSize = priceWithSize,
                PriceWithFinish = priceWithFinish,
                LoyaltyDiscountPercent = loyaltyDiscountPercent,
                LoyaltyDiscountAmount = loyaltyDiscountAmount,
                QuantityDiscountPercent = quantityDiscountPercent,
                QuantityDiscountAmount = quantityDiscountAmount,
                SeasonalDiscountPercent = seasonalDiscountPercent,
                SeasonalDiscountAmount = seasonalDiscountAmount,
                TotalDiscountPercent = totalDiscountPercent,
                TotalDiscountAmount = totalDiscountAmount,
                FinalPrice = finalPrice,
                CreatedAt = DateTime.Now,
                IsSeasonalActive = isSeasonalActive
            };
        }

        private double CalculateSizeSurcharge(double width, double height)
        {
            bool isWidthStandard = (width >= 800 && width <= 900);
            bool isHeightStandard = (height >= 2000 && height <= 2100);
            bool isWidthSmallDeviation = (width >= 700 && width < 800) || (width > 900 && width <= 1000);
            bool isHeightSmallDeviation = (height >= 1900 && height < 2000) || (height > 2100 && height <= 2200);
            bool isWidthLargeDeviation = (width < 700 || width > 1000);
            bool isHeightLargeDeviation = (height < 1900 || height > 2200);

            if (isWidthStandard && isHeightStandard)
                return 0;
            else if ((isWidthSmallDeviation && isHeightStandard) ||
                     (isWidthStandard && isHeightSmallDeviation) ||
                     (isWidthSmallDeviation && isHeightSmallDeviation))
                return 10;
            else if (isWidthLargeDeviation || isHeightLargeDeviation)
                return 25;
            else
                return 25;
        }

        private void AddDoor(string loopType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtWidth.Text) || string.IsNullOrWhiteSpace(txtHeight.Text))
                {
                    UpdateStatus("❌ Заполните все поля!", "#F44336");
                    return;
                }

                double width = ParseNumber(txtWidth.Text);
                double height = ParseNumber(txtHeight.Text);

                if (width <= 0 || height <= 0)
                {
                    UpdateStatus("❌ Размеры должны быть положительными!", "#F44336");
                    return;
                }

                DoorOrder order = CreateOrderFromForm();
                orders.Add(order);

                string seasonalInfo = isSeasonalActive ? " + сезонная 7%" : " (сезонная неактивна)";
                UpdateStatus($"✅ Дверь #{order.Id} добавлена через {loopType}. Цена: {order.FinalPrice:N0} ₽{seasonalInfo}", "#4CAF50");
                UpdateTotals();

                lstOrders.ScrollIntoView(order);
                lstOrders.SelectedItem = order;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Ошибка: {ex.Message}", "#F44336");
            }
        }


        private void BtnAddFor_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1; i++)
            {
                AddDoor("for");
            }
        }


        private void BtnAddWhile_Click(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            while (counter < 1)
            {
                AddDoor("while");
                counter++;
            }
        }


        private void BtnAutoAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = (int)ParseNumber(txtCount.Text);

                if (count <= 0)
                {
                    UpdateStatus("❌ Введите положительное число!", "#F44336");
                    return;
                }

                if (count > 20)
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"Вы собираетесь добавить {count} дверей. Продолжить?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                UpdateStatus($"⏳ Добавление {count} дверей через цикл for...", "#FF9800");

                for (int i = 0; i < count; i++)
                {
                    AddDoor($"for (авто) [{i + 1}/{count}]");
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() => { }));
                }

                UpdateStatus($"✅ Добавлено {count} дверей через цикл for", "#4CAF50");
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Ошибка: {ex.Message}", "#F44336");
            }
        }


        private void BtnRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lstOrders.SelectedItem is DoorOrder selected)
            {
                orders.Remove(selected);
                UpdateStatus($"🗑 Дверь #{selected.Id} удалена", "#FF9800");
                UpdateTotals();
            }
            else
            {
                UpdateStatus("❌ Выберите дверь для удаления", "#F44336");
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (orders.Count == 0)
            {
                UpdateStatus("ℹ️ Список уже пуст", "#FF9800");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить все {orders.Count} дверей?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                orders.Clear();
                nextId = 1;
                UpdateStatus("🗑 Все двери удалены", "#FF9800");
                UpdateTotals();
            }
        }

        private void LstOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstOrders.SelectedItem is DoorOrder selected)
            {
                lblSelectedInfo.Text =
                    $"🔹 {selected.GetShortDetails()}\n" +
                    $"💰 Цена: {selected.FinalPrice:N0} ₽ | Скидка: {selected.TotalDiscountPercent}%\n" +
                    $"📅 Добавлена: {selected.CreatedAt:HH:mm:ss}";
            }
            else
            {
                lblSelectedInfo.Text = "Выберите дверь для просмотра деталей";
            }
        }



        private void UpdateTotals()
        {
            int count = orders.Count;
            lblTotalCount.Text = count.ToString();

            if (count == 0)
            {
                lblTotalBasePrice.Text = "0 ₽";
                lblTotalSizeSurcharge.Text = "0 ₽";
                lblTotalWithSize.Text = "0 ₽";
                lblTotalDiscount.Text = "0 ₽";
                lblTotalFinalPrice.Text = "ИТОГО: 0 ₽";
                lblAveragePrice.Text = "";
                lblDetailedReport.Text = "Добавьте двери для формирования отчёта";
                lblLoopDemo.Text = "Добавьте двери с помощью кнопок 'for' или 'while'";
                return;
            }

            double totalBasePrice = 0;
            double totalSizeSurcharge = 0;
            double totalWithSize = 0;
            double totalDiscount = 0;
            double totalFinalPrice = 0;

            foreach (var order in orders)
            {
                totalBasePrice += order.BasePrice;
                totalSizeSurcharge += order.SizeSurchargeAmount;
                totalWithSize += order.PriceWithSize;
                totalDiscount += order.TotalDiscountAmount;
                totalFinalPrice += order.FinalPrice;
            }

            lblTotalBasePrice.Text = $"{totalBasePrice:N0} ₽";
            lblTotalSizeSurcharge.Text = $"{totalSizeSurcharge:N0} ₽";
            lblTotalWithSize.Text = $"{totalWithSize:N0} ₽";
            lblTotalDiscount.Text = $"-{totalDiscount:N0} ₽";
            lblTotalFinalPrice.Text = $"ИТОГО: {totalFinalPrice:N0} ₽";
            lblAveragePrice.Text = $"Средняя цена: {totalFinalPrice / count:N0} ₽ за дверь";

            string seasonalStatus = isSeasonalActive ? "✅ АКТИВНА (июнь)" : "❌ НЕАКТИВНА";

            lblDetailedReport.Text =
                $"📊 ДЕТАЛЬНЫЙ ОТЧЁТ ПО {count} ДВЕРЯМ:\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"💰 БАЗОВАЯ СТОИМОСТЬ: {totalBasePrice:N0} ₽\n" +
                $"📐 НАЦЕНКА ЗА РАЗМЕР: +{totalSizeSurcharge:N0} ₽\n" +
                $"💎 СТОИМОСТЬ С НАЦЕНКОЙ: {totalWithSize:N0} ₽\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"🎯 СКИДКИ (всего): -{totalDiscount:N0} ₽\n" +
                $"   • Карта лояльности: -{totalDiscount * 0.3:N0} ₽ (прим.)\n" +
                $"   • Оптовая скидка: -{totalDiscount * 0.5:N0} ₽ (прим.)\n" +
                $"   • Сезонная скидка: {(isSeasonalActive ? $"-{totalDiscount * 0.2:N0} ₽" : "0 ₽")} {seasonalStatus}\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"✅ ИТОГОВАЯ СТОИМОСТЬ: {totalFinalPrice:N0} ₽\n" +
                $"📊 СРЕДНЯЯ ЦЕНА: {totalFinalPrice / count:N0} ₽ за дверь";

            int forCount = 0;
            int whileCount = 0;

            lblLoopDemo.Text =
                $"🔄 ЦИКЛЫ В ДЕЙСТВИИ:\n" +
                $"• Всего добавлено дверей: {count}\n" +
                $"• Цикл for используется для добавления N дверей\n" +
                $"• Цикл while используется для добавления с условием\n" +
                $"• Каждая дверь обработана последовательно (линейно)\n" +
                $"• {(count > 0 ? "✅" : "⏳")} Партия сформирована";
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

        private void UpdateStatus(string message, string colorHex = "#666666")
        {
            lblStatus.Text = message;

            if (colorHex.StartsWith("#"))
            {
                var converter = new System.Windows.Media.BrushConverter();
                lblStatus.Foreground = (System.Windows.Media.Brush)converter.ConvertFromString(colorHex);
            }
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddDoor("for (Enter)");
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
    }
}