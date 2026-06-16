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
            btnGreet.Click += BtnGreet_Click;

            txtName.KeyDown += TxtName_KeyDown;
            txtName.GotFocus += TxtName_GotFocus;
            txtName.LostFocus += TxtName_LostFocus;

            this.Loaded += MainWindow_Loaded;
        }

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

                btnGreet.Content = "Ещё раз?";
            }
        }

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnGreet_Click(sender, e);
            }
        }

        private void TxtName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtName.Background = new SolidColorBrush(Colors.LightYellow);
            if (string.IsNullOrEmpty(txtName.Text))
            {
                lblResult.Text = "Введите ваше имя и нажмите Enter или кнопку";
                lblResult.Foreground = new SolidColorBrush(Colors.Blue);
            }
        }

        private void TxtName_LostFocus(object sender, RoutedEventArgs e)
        {
            txtName.Background = new SolidColorBrush(Colors.White);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Focus();
            lblResult.Text = "Готов к работе! Введите имя...";
        }
    }
}