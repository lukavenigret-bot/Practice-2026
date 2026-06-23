using System;
using Microsoft.Maui.Controls;
using DoorMobileApp.Models;
using DoorMobileApp.Services;

namespace DoorMobileApp
{
    /// <summary>
    /// Главная страница приложения
    /// </summary>
    public partial class MainPage : ContentPage
    {
        // Сервисы
        private readonly DoorCalculator _calculator = new DoorCalculator();

        public MainPage()
        {
            InitializeComponent();

            // Подписка на события
            btnCalculate.Clicked += OnCalculateClicked;
            btnClear.Clicked += OnClearClicked;

            // Обработка нажатия Enter
            txtWidth.Completed += OnEntryCompleted;
            txtHeight.Completed += OnEntryCompleted;
        }

        // 1. ОБРАБОТКА РАСЧЁТА

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            try
            {
                // --- ШАГ 1: Получение данных ---

                // Модель
                string modelText = cmbModel.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(modelText))
                {
                    ShowError("Выберите модель двери");
                    return;
                }

                // Размеры
                if (!double.TryParse(txtWidth.Text, out double width) || width <= 0)
                {
                    ShowError("Ширина должна быть положительным числом");
                    return;
                }

                if (!double.TryParse(txtHeight.Text, out double height) || height <= 0)
                {
                    ShowError("Высота должна быть положительным числом");
                    return;
                }

                // Отделка
                string finishText = cmbFinish.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(finishText))
                {
                    ShowError("Выберите тип отделки");
                    return;
                }

                // --- ШАГ 2: Создание параметров ---

                var parameters = new DoorParameters
                {
                    ModelName = modelText.Split('-')[0].Trim(),
                    BasePrice = _calculator.GetBasePrice(modelText),
                    WidthMm = width,
                    HeightMm = height,
                    FinishName = finishText.Split('(')[0].Trim(),
                    FinishCoeff = _calculator.GetFinishCoeff(finishText)
                };

                // --- ШАГ 3: Валидация ---

                if (!_calculator.ValidateParameters(parameters, out string error))
                {
                    ShowError(error);
                    return;
                }

                // --- ШАГ 4: Расчёт ---

                var result = _calculator.Calculate(parameters);

                if (!result.IsValid)
                {
                    ShowError(result.ErrorMessage);
                    return;
                }

                // --- ШАГ 5: Отображение результатов ---

                lblResult.Text = result.GetPriceDisplay();
                lblResult.TextColor = Colors.Green;

                lblDetails.Text =
                    $"Модель: {parameters.ModelName}\n" +
                    $"Размер: {parameters.GetSizeDescription()}\n" +
                    $"Площадь: {result.GetAreaDisplay()}\n" +
                    $"Отделка: {parameters.FinishName} (×{parameters.FinishCoeff:F2})\n" +
                    $"Надбавка: {result.SizeSurcharge:N0} ₽";

                lblStatus.Text = $"✅ Расчёт выполнен. Цена: {result.FinalPrice:N0} ₽";
                lblStatus.TextColor = Colors.Green;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        // 2. ОЧИСТКА

        private void OnClearClicked(object sender, EventArgs e)
        {
            txtWidth.Text = "800";
            txtHeight.Text = "2000";
            cmbModel.SelectedIndex = 0;
            cmbFinish.SelectedIndex = 0;

            lblResult.Text = "Нажмите «Рассчитать»";
            lblResult.TextColor = Colors.Gray;
            lblDetails.Text = "Введите данные и нажмите «Рассчитать»";

            lblStatus.Text = "Очищено. Готов к расчёту";
            lblStatus.TextColor = Colors.Gray;
        }

        // 3. ОБРАБОТКА ВВОДА

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            // Автоматический расчёт при нажатии Enter
            OnCalculateClicked(sender, e);
        }

        // 4. ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

        private void ShowError(string message)
        {
            lblResult.Text = $"❌ {message}";
            lblResult.TextColor = Colors.Red;
            lblDetails.Text = "Исправьте ошибку и попробуйте снова";

            lblStatus.Text = $"Ошибка: {message}";
            lblStatus.TextColor = Colors.Red;
        }
    }
}