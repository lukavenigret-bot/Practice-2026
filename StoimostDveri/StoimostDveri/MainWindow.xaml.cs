using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StoimostDveri.Helpers;
using StoimostDveri.Models;
using StoimostDveri.Services;

namespace StoimostDveri
{
    public partial class MainWindow : Window
    {
        private readonly DoorCalculator _calculator = new DoorCalculator();
        private readonly DoorDataFactory _factory = new DoorDataFactory();

        public MainWindow()
        {
            InitializeComponent();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            btnCalculate.Click += BtnCalculate_Click;
            btnClear.Click += BtnClear_Click;

            txtWidth.KeyDown += TxtInput_KeyDown;
            txtHeight.KeyDown += TxtInput_KeyDown;

            txtWidth.PreviewTextInput += TxtInput_PreviewTextInput;
            txtHeight.PreviewTextInput += TxtInput_PreviewTextInput;

            txtWidth.LostFocus += TxtInput_LostFocus;
            txtHeight.LostFocus += TxtInput_LostFocus;
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Выполняется расчёт...", "#FF9800");

            try
            {
                DoorParameters parameters = _factory.CreateFromForm(
                    cmbModel,
                    cmbFinish,
                    txtWidth.Text,
                    txtHeight.Text
                );

                CalculationResult result = _calculator.Calculate(parameters);

                DisplayResults(parameters, result);

                UpdateStatus($"Базовая цена: {parameters.BasePrice:N0} ₽ | Итоговая цена: {result.FinalPrice:N0} ₽", "#4CAF50");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}", "#F44336");
                MessageBox.Show($"Произошла ошибка:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void DisplayResults(DoorParameters parameters, CalculationResult result)
        {
            lblModel.Text = parameters.ModelName;
            lblArea.Text = $"{result.GetAreaDisplay()} ({parameters.GetSizeDescription()})";
            lblSizeSurcharge.Text = result.GetSurchargeDisplay();
            lblFinishCoeff.Text = $"{parameters.FinishName} ({parameters.FinishCoeff:F2})";
            lblFinalPrice.Text = result.GetPriceDisplay();
        }
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtWidth.Text = "800";
            txtHeight.Text = "2000";
            cmbModel.SelectedIndex = 0;
            cmbFinish.SelectedIndex = 0;

            lblModel.Text = "-";
            lblArea.Text = "-";
            lblSizeSurcharge.Text = "-";
            lblFinishCoeff.Text = "-";
            lblFinalPrice.Text = "ИТОГО: 0 ₽";

            UpdateStatus("Очищено. Готов к расчёту.", "#666666");
        }


        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnCalculate_Click(sender, e);
                e.Handled = true;
            }
        }

        private void TxtInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !StringHelper.IsDigitsOnly(e.Text);
        }

        private void TxtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            if (StringHelper.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
                return;
            }

            int value = StringHelper.ParseInt(textBox.Text, 800);
            if (value <= 0)
            {
                textBox.Text = "800";
                UpdateStatus("Размер не может быть отрицательным. Установлено стандартное значение.", "#FF9800");
            }
        }

        private void UpdateStatus(string message, string colorHex = "#666666")
        {
            lblStatus.Text = message;

            if (!string.IsNullOrEmpty(colorHex) && colorHex.StartsWith("#"))
            {
                try
                {
                    var converter = new BrushConverter();
                    lblStatus.Foreground = (Brush)converter.ConvertFromString(colorHex);
                }
                catch
                {
                    lblStatus.Foreground = SystemColors.ControlTextBrush;
                }
            }
        }
    }
}