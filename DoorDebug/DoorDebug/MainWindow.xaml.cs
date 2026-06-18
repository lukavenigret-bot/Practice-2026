using System;
using System.Windows;
using System.Windows.Controls;

namespace DoorDebug
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnCalculate.Click += BtnCalculate_Click;
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            lblDebugInfo.Text = "";
            lblModelWarning.Text = "";
            lblWidthWarning.Text = "";
            lblHeightWarning.Text = "";

            try
            {
                string widthText = txtWidth.Text;
                string heightText = txtHeight.Text;

                int width = int.Parse(widthText);
                int height = int.Parse(heightText);

                double area = (width / 1000) * (height / 1000);

                ComboBoxItem selectedModel = cmbModel.SelectedItem as ComboBoxItem;
                string modelName = selectedModel.Content.ToString();
                double basePrice = double.Parse(selectedModel.Tag.ToString());

                double standardArea = 1.6;
                double surchargePerSqm = 3000;

                double extraArea = area - standardArea;
                double sizeSurcharge = extraArea * surchargePerSqm;
                double finalPrice = basePrice + sizeSurcharge;

                lblResult.Text = "Цена: " + finalPrice + " руб.";
                lblResult.Foreground = System.Windows.Media.Brushes.Green;

                lblDebugInfo.Text =
                    $"✅ Расчёт выполнен (но возможны ошибки!)\n" +
                    $"Модель: {modelName}\n" +
                    $"Размер: {width}×{height} мм\n" +
                    $"Площадь: {area:F2} кв.м\n" +
                    $"Надбавка: {sizeSurcharge:N0} ₽\n" +
                    $"Итоговая цена: {finalPrice:N0} ₽";
            }
            catch (FormatException)
            {
                lblResult.Text = "❌ ОШИБКА: Введите корректные числа!";
                lblResult.Foreground = System.Windows.Media.Brushes.Red;
                lblWidthWarning.Text = "⚠️ Введите число";
                lblHeightWarning.Text = "⚠️ Введите число";
                lblDebugInfo.Text = "FormatException: неверный формат ввода";
            }
            catch (NullReferenceException)
            {
                lblResult.Text = "❌ ОШИБКА: Выберите модель двери!";
                lblResult.Foreground = System.Windows.Media.Brushes.Red;
                lblModelWarning.Text = "⚠️ Выберите модель";
                lblDebugInfo.Text = "NullReferenceException: модель не выбрана";
            }
            catch (Exception ex)
            {
                lblResult.Text = $"❌ ОШИБКА: {ex.Message}";
                lblResult.Foreground = System.Windows.Media.Brushes.Red;
                lblDebugInfo.Text = $"Исключение: {ex.GetType().Name}";
            }
        }

        private void BtnDebug_Click(object sender, RoutedEventArgs e)
        {
            lblInstructions.Text =
                "🔍 ПОШАГОВАЯ ИНСТРУКЦИЯ ПО ОТЛАДКЕ:\n" +
                "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                "\n" +
                "1️⃣ УСТАНОВКА ТОЧЕК ОСТАНОВА (Breakpoints):\n" +
                "   • Откройте MainWindow.xaml.cs\n" +
                "   • Кликните на серое поле слева от номера строки 33\n" +
                "   • Появится красный кружок ●\n" +
                "   • Повторите для строк 40, 48, 60, 69\n" +
                "   • Красные кружки = программа остановится здесь\n" +
                "\n" +
                "2️⃣ ЗАПУСК ОТЛАДКИ:\n" +
                "   • Нажмите F5 (или кнопку Start Debugging)\n" +
                "   • Введите данные: Ширина=800, Высота=2000\n" +
                "   • Выберите модель (любую)\n" +
                "   • Нажмите «Рассчитать (с ошибками)»\n" +
                "\n" +
                "3️⃣ УПРАВЛЕНИЕ ОТЛАДКОЙ:\n" +
                "   • F10 (Step Over) — выполнить строку и перейти к следующей\n" +
                "   • F11 (Step Into) — зайти внутрь метода\n" +
                "   • Shift+F11 (Step Out) — выйти из метода\n" +
                "   • F5 (Continue) — продолжить до следующей точки\n" +
                "\n" +
                "4️⃣ ПРОСМОТР ЗНАЧЕНИЙ ПЕРЕМЕННЫХ:\n" +
                "   • Locals — показывает все переменные (автоматически)\n" +
                "   • Watch — показывает выбранные вами переменные\n" +
                "   • Наведите мышь на переменную в коде → подсказка\n" +
                "\n" +
                "5️⃣ ЧТО ИСКАТЬ (ошибки в коде):\n" +
                "   • При пустых полях → FormatException\n" +
                "   • При невыбранной модели → NullReferenceException\n" +
                "   • При 800×2000 → площадь должна быть 1.6, а не 0\n" +
                "   • При маленьком размере → надбавка не должна быть отрицательной\n" +
                "   • Цена должна выводиться с разделителями: 15 000 ₽";
        }
    }
}