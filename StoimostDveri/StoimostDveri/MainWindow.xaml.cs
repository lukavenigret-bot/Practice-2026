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
    // Главное окно приложения "Калькулятор стоимости двери"
    // Предоставляет интерфейс для расчёта стоимости дверей
    public partial class MainWindow : Window
    {
        // Внедрение зависимостей
        private readonly DoorCalculator _calculator = new DoorCalculator();
        private readonly DoorDataFactory _factory = new DoorDataFactory();

        // Конструктор главного окна
        // Инициализирует компоненты и подписывает события
        public MainWindow()
        {
            InitializeComponent();
            SubscribeEvents();
        }

        // 1. ПОДПИСКА НА СОБЫТИЯ
        // Подписывает все элементы управления на соответствующие события
        private void SubscribeEvents()
        {
            // События кнопок
            btnCalculate.Click += BtnCalculate_Click;
            btnClear.Click += BtnClear_Click;

            // События клавиатуры (Enter для расчёта)
            txtWidth.KeyDown += TxtInput_KeyDown;
            txtHeight.KeyDown += TxtInput_KeyDown;

            // События валидации ввода (только цифры)
            txtWidth.PreviewTextInput += TxtInput_PreviewTextInput;
            txtHeight.PreviewTextInput += TxtInput_PreviewTextInput;

            // События валидации при потере фокуса
            txtWidth.LostFocus += TxtInput_LostFocus;
            txtHeight.LostFocus += TxtInput_LostFocus;
        }

        // 2. ОСНОВНАЯ ЛОГИКА РАСЧЁТА
        // Обрабатывает нажатие кнопки "Рассчитать стоимость"
        // Выполняет расчёт и отображает результат
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Выполняется расчёт...", "#FF9800");

            try
            {
                // 2.1 Получение данных из формы через фабрику
                DoorParameters parameters = _factory.CreateFromForm(
                    cmbModel,
                    cmbFinish,
                    txtWidth.Text,
                    txtHeight.Text
                );

                // 2.3 Выполнение расчёта
                CalculationResult result = _calculator.Calculate(parameters);

                // 2.4 Отображение результатов
                DisplayResults(parameters, result);

                // 2.5 Обновление статуса
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

        // Отображает результаты расчёта на форме
        private void DisplayResults(DoorParameters parameters, CalculationResult result)
        {
            lblModel.Text = parameters.ModelName;
            lblArea.Text = $"{result.GetAreaDisplay()} ({parameters.GetSizeDescription()})";
            lblSizeSurcharge.Text = result.GetSurchargeDisplay();
            lblFinishCoeff.Text = $"{parameters.FinishName} ({parameters.FinishCoeff:F2})";
            lblFinalPrice.Text = result.GetPriceDisplay();
        }

        // 3. ОЧИСТКА ПОЛЕЙ
        // Обрабатывает нажатие кнопки "Очистить"
        // Сбрасывает все поля ввода и результаты
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Сброс полей ввода
            txtWidth.Text = "800";
            txtHeight.Text = "2000";
            cmbModel.SelectedIndex = 0;
            cmbFinish.SelectedIndex = 0;

            // Сброс результатов
            lblModel.Text = "-";
            lblArea.Text = "-";
            lblSizeSurcharge.Text = "-";
            lblFinishCoeff.Text = "-";
            lblFinalPrice.Text = "ИТОГО: 0 ₽";

            UpdateStatus("Очищено. Готов к расчёту.", "#666666");
        }

        // 4. ОБРАБОТКА ВВОДА
        // Обрабатывает нажатие Enter в текстовых полях
        // Выполняет расчёт при нажатии Enter
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnCalculate_Click(sender, e);
                e.Handled = true;
            }
        }

        // Ограничивает ввод только цифрами
        private void TxtInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !StringHelper.IsDigitsOnly(e.Text);
        }

        // Проверяет ввод при потере фокуса
        // Устанавливает стандартное значение при некорректном вводе
        private void TxtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            // Проверка на пустое значение
            if (StringHelper.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
                return;
            }

            // Проверка на отрицательное значение
            int value = StringHelper.ParseInt(textBox.Text, 800);
            if (value <= 0)
            {
                textBox.Text = "800";
                UpdateStatus("Размер не может быть отрицательным. Установлено стандартное значение.", "#FF9800");
            }
        }

        // 5. ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // Обновляет текст статусной строки с указанным цветом
        // <param name="message">Текст сообщения</param>
        // <param name="colorHex">Цвет в HEX-формате (например, #FF9800)</param>
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