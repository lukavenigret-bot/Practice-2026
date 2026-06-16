using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoimostDveri
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        // Подписка на события
            btnCalculate.Click += BtnCalculate_Click;
            btnClear.Click += BtnClear_Click;

            // Автоматический расчёт при нажатии Enter в полях
            txtWidth.KeyDown += TxtInput_KeyDown;
            txtHeight.KeyDown += TxtInput_KeyDown;

            // Ограничение ввода только чисел
            txtWidth.PreviewTextInput += TxtInput_PreviewTextInput;
            txtHeight.PreviewTextInput += TxtInput_PreviewTextInput;

            // Валидация при потере фокуса
            txtWidth.LostFocus += TxtInput_LostFocus;
            txtHeight.LostFocus += TxtInput_LostFocus;
        }

        /// <summary>
        /// ЛИНЕЙНЫЙ АЛГОРИТМ расчёта стоимости двери
        /// Все вычисления выполняются строго последовательно
        /// </summary>
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Выполняется расчёт...", "#FF9800");

            try
            {
                // ШАГ 1: Получение входных данных
                ComboBoxItem selectedModel = cmbModel.SelectedItem as ComboBoxItem;
                double basePrice = double.Parse(selectedModel.Tag.ToString(), CultureInfo.InvariantCulture);
                string modelName = selectedModel.Content.ToString().Split('-')[0].Trim();

                // Используем int для размеров (только целые числа)
                int widthMm = int.Parse(txtWidth.Text.Trim());
                int heightMm = int.Parse(txtHeight.Text.Trim());

                ComboBoxItem selectedFinish = cmbFinish.SelectedItem as ComboBoxItem;
                double finishCoeff = double.Parse(selectedFinish.Tag.ToString(), CultureInfo.InvariantCulture);
                string finishName = selectedFinish.Content.ToString().Split('(')[0].Trim();

                // ШАГ 2: Вычисления
                double widthM = widthMm / 1000.0;
                double heightM = heightMm / 1000.0;
                double area = widthM * heightM;

                double standardArea = 1.6;
                double surchargePerSqm = 3000;
                double extraArea = Math.Max(0, area - standardArea);
                double sizeSurcharge = extraArea * surchargePerSqm;
                double intermediatePrice = basePrice + sizeSurcharge;
                double finalPrice = intermediatePrice * finishCoeff;

                // ШАГ 3: Вывод результатов
                lblModel.Text = $"{modelName}";
                lblArea.Text = $"{area:F2} кв.м (ширина {widthMm}мм × высота {heightMm}мм)";
                lblSizeSurcharge.Text = $"{sizeSurcharge:F0} ₽ (доп. {extraArea:F2} кв.м × {surchargePerSqm} ₽)";
                lblFinishCoeff.Text = $"{finishName} ({finishCoeff:F2})";
                lblFinalPrice.Text = $"ИТОГО: {finalPrice:N0} ₽";

                UpdateStatus($"Базовая цена: {basePrice:N0} ₽ | Итоговая цена: {finalPrice:N0} ₽", "#4CAF50");
            }
            catch (FormatException)
            {
                UpdateStatus("Ошибка: введите целые числа!", "#F44336");
                MessageBox.Show("Пожалуйста, введите целые числа для размеров двери.\n" +
                              "Например: 800, 2000, 900, 2100",
                              "Ошибка ввода",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}", "#F44336");
            }
        }
        

        /// <summary>
        /// Очистка всех полей и сброс результатов
        /// </summary>
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

        /// <summary>
        /// Обработка нажатия Enter для быстрого расчёта
        /// </summary>
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnCalculate_Click(sender, e);
            }
        }

        /// <summary>
        /// Ограничение ввода: только цифры и десятичная точка
        /// </summary>
        private void TxtInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = e.Text;
            bool isValid = true;

            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    isValid = false;
                    break;
                }
            }

            e.Handled = !isValid;
        }

        /// <summary>
        /// Валидация при потере фокуса
        /// </summary>
        private void TxtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "0";
                }

                // Дополнительная проверка на пустоту
                if (int.TryParse(textBox.Text, out int value))
                {
                    if (value <= 0)
                    {
                        textBox.Text = "800";
                        UpdateStatus("Размер не может быть отрицательным. Установлено стандартное значение.", "#FF9800");
                    }
                }
            }
        }

        /// <summary>
        /// Вспомогательный метод для обновления статус-бара
        /// </summary>
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
