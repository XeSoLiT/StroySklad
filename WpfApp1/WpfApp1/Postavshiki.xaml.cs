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
    public partial class Postavshiki : Window
    {
        public Postavshiki()
        {
            InitializeComponent();
            LoadPostavshiki();
        }
        private async void LoadPostavshiki()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    HttpResponseMessage response = await client.GetAsync("https://localhost:7269/api/Postavshiki");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        PostavshikiDataGrid.ItemsSource = JsonSerializer.Deserialize<List<Postavshik>>(json);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить список поставщиков. Ошибка сервера.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private void PostavshikiDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PostavshikiDataGrid.SelectedItem is Postavshik p)
            {
                NazvanieBOX.Text = p.nazvanie;
                GorodBOX.Text = p.gorod;
                TelefonBOX.Text = p.telefon;
                FamiliyaBOX.Text = p.familiya;
                ImyaBOX.Text = p.imya;
            }
        }
        private async void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NazvanieBOX.Text))
            {
                MessageBox.Show("Заполните название поставщика.");
                return;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    var data = new { nazvanie = NazvanieBOX.Text, gorod = GorodBOX.Text, ulica = "ул. Ленина", dom = "1", telefon = TelefonBOX.Text, familiya = FamiliyaBOX.Text, imya = ImyaBOX.Text, otchestvo = (string)null };
                    var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7269/api/Postavshiki", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Поставщик успешно добавлен.");
                        LoadPostavshiki();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка добавления поставщика.");
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
            if (PostavshikiDataGrid.SelectedItem is Postavshik p)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                        HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7269/api/Postavshiki/{p.id}");
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Поставщик успешно удален.");
                            LoadPostavshiki();
                        }
                        else
                        {
                            MessageBox.Show("Невозможно удалить поставщика, так как он используется в накладных.");
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
                MessageBox.Show("Выберите поставщика из таблицы для удаления.");
            }
        }
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Menu M = new Menu();
            M.Show();
            this.Close();
        }
    }
    public class Postavshik
    {
        public int id { get; set; }
        public string nazvanie { get; set; }
        public string gorod { get; set; }
        public string telefon { get; set; }
        public string familiya { get; set; }
        public string imya { get; set; }
    }
}