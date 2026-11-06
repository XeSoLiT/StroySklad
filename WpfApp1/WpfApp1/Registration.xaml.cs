using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace WpfApp1
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }
        private async void RegBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FamiliyaBOX.Text) || string.IsNullOrWhiteSpace(ImyaBOX.Text) || string.IsNullOrWhiteSpace(LoginBOX.Text) || string.IsNullOrWhiteSpace(ParolBOX.Password))
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля.");
                return;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var regData = new { familiya = FamiliyaBOX.Text, imya = ImyaBOX.Text, otchestvo = OtchestvoBOX.Text, login = LoginBOX.Text, parol = ParolBOX.Password };
                    var content = new StringContent(JsonSerializer.Serialize(regData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7269/api/Auth/register", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Регистрация прошла успешно. Теперь вы можете войти.");
                        MainWindow MW = new MainWindow();
                        MW.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка регистрации. Возможно, логин уже занят.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }
    }
}