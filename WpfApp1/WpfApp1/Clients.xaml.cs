using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public partial class Clients : Window
    {
        public Clients()
        {
            InitializeComponent();
            LoadClients();
        }
        private async void LoadClients()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    HttpResponseMessage response = await client.GetAsync("https://localhost:7269/api/Clients");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        ClientsDataGrid.ItemsSource = JsonSerializer.Deserialize<List<Client>>(json);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить список клиентов. Ошибка сервера.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private void ClientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client c)
            {
                NazvanieBOX.Text = c.nazvanie;
                GorodBOX.Text = c.gorod;
                TelefonBOX.Text = c.telefon;
                FamiliyaBOX.Text = c.familiya;
                ImyaBOX.Text = c.imya;
            }
        }
        private async void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NazvanieBOX.Text))
            {
                MessageBox.Show("Заполните название клиента.");
                return;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    var data = new { nazvanie = NazvanieBOX.Text, gorod = GorodBOX.Text, ulica = "ул. Ленина", dom = "1", telefon = TelefonBOX.Text, familiya = FamiliyaBOX.Text, imya = ImyaBOX.Text, otchestvo = (string)null, skidka = 0 };
                    var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7269/api/Clients", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Клиент успешно добавлен.");
                        LoadClients();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка добавления клиента.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private async void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client c)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                        HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7269/api/Clients/{c.id}");
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Клиент успешно удален.");
                            LoadClients();
                        }
                        else
                        {
                            MessageBox.Show("Невозможно удалить клиента, так как он используется в накладных.");
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка подключения к серверу API.");
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента из таблицы для удаления.");
            }
        }
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Menu M = new Menu();
            M.Show();
            this.Close();
        }
    }
    public class Client
    {
        public int id { get; set; }
        public string nazvanie { get; set; }
        public string gorod { get; set; }
        public string telefon { get; set; }
        public string familiya { get; set; }
        public string imya { get; set; }
    }
}