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
    public partial class MainWindow : Window
    {
        public static string UserRole = "";
        public static string UserToken = "";
        public static string UserFIO = "";
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBOX.Text) || string.IsNullOrWhiteSpace(ParolBOX.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var loginData = new { login = LoginBOX.Text, parol = ParolBOX.Password };
                    var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7269/api/Auth/login", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(responseString))
                        {
                            JsonElement root = doc.RootElement;
                            UserToken = root.GetProperty("token").GetString();
                            UserRole = root.GetProperty("role").GetString();
                            UserFIO = root.GetProperty("fio").GetString();
                        }
                        Menu M = new Menu();
                        M.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private void RegBTN_Click(object sender, RoutedEventArgs e)
        {
            Registration R = new Registration();
            R.Show();
            this.Close();
        }
    }
}