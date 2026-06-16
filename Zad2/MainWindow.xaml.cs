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

namespace Zad2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Подписка на события (можно делать в XAML, но можно и в коде)
            btnGreet.Click += BtnGreet_Click;

            // Дополнительные события для демонстрации
            txtName.KeyDown += TxtName_KeyDown;
            txtName.GotFocus += TxtName_GotFocus;
            txtName.LostFocus += TxtName_LostFocus;

            // Событие загрузки окна
            this.Loaded += MainWindow_Loaded;
        }

        // Событие при нажатии на кнопку
        private void BtnGreet_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                lblResult.Text = "Пожалуйста, введите ваше имя!";
                lblResult.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                lblResult.Text = $"Привет, {name}! Добро пожаловать в WPF!";
                lblResult.Foreground = new SolidColorBrush(Colors.Green);

                // Дополнительная реакция: меняем текст кнопки
                btnGreet.Content = "Ещё раз?";
            }
        }

        // Событие при нажатии Enter в текстовом поле
        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnGreet_Click(sender, e);
            }
        }

        // Событие при фокусе на текстовом поле
        private void TxtName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtName.Background = new SolidColorBrush(Colors.LightYellow);
            if (string.IsNullOrEmpty(txtName.Text))
            {
                lblResult.Text = "Введите ваше имя и нажмите Enter или кнопку";
                lblResult.Foreground = new SolidColorBrush(Colors.Blue);
            }
        }

        // Событие при потере фокуса
        private void TxtName_LostFocus(object sender, RoutedEventArgs e)
        {
            txtName.Background = new SolidColorBrush(Colors.White);
        }

        // Событие при загрузке окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Focus();
            lblResult.Text = "Готов к работе! Введите имя...";
        }
    }
}