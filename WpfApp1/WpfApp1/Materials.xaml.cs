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
    public partial class Materials : Window
    {
        public Materials()
        {
            InitializeComponent();
            LoadMaterials();
        }
        private async void LoadMaterials()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    HttpResponseMessage response = await client.GetAsync("https://localhost:7269/api/Materials");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        MaterialsDataGrid.ItemsSource = JsonSerializer.Deserialize<List<Material>>(json);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить список материалов. Ошибка сервера.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private void MaterialsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is Material m)
            {
                NaimenovanieBOX.Text = m.naimenovanie;
                CenaBOX.Text = m.cena.ToString();
                KolichestvoBOX.Text = m.kolichestvo.ToString();
                EdIzmBOX.Text = m.iD_ed_izm.ToString();
                ProizvoditelBOX.Text = m.iD_proizvoditelya.ToString();
            }
        }
        private async void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NaimenovanieBOX.Text) || string.IsNullOrWhiteSpace(CenaBOX.Text) || string.IsNullOrWhiteSpace(KolichestvoBOX.Text))
            {
                MessageBox.Show("Заполните обязательные поля (Наименование, Цена, Количество).");
                return;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    var data = new { id_ed_izm = int.Parse(EdIzmBOX.Text), id_proizvoditelya = int.Parse(ProizvoditelBOX.Text), naimenovanie = NaimenovanieBOX.Text, cena = decimal.Parse(CenaBOX.Text), kolichestvo = decimal.Parse(KolichestvoBOX.Text), photoPath = (string)null };
                    var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7269/api/Materials", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Материал успешно добавлен.");
                        LoadMaterials();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка добавления. Проверьте, существуют ли указанные ID единицы измерения и производителя.");
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("В поля Цена и Количество можно вводить только числа.");
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к серверу API.");
            }
        }
        private async void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is Material m)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                        var data = new { id = m.id, id_ed_izm = int.Parse(EdIzmBOX.Text), id_proizvoditelya = int.Parse(ProizvoditelBOX.Text), naimenovanie = NaimenovanieBOX.Text, cena = decimal.Parse(CenaBOX.Text), kolichestvo = decimal.Parse(KolichestvoBOX.Text), photoPath = (string)null };
                        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PutAsync($"https://localhost:7269/api/Materials/{m.id}", content);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Материал успешно обновлен.");
                            LoadMaterials();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка обновления. Проверьте корректность введенных данных.");
                        }
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("В поля Цена и Количество можно вводить только числа.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка подключения к серверу API.");
                }
            }
            else
            {
                MessageBox.Show("Выберите материал из таблицы для изменения.");
            }
        }
        private async void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is Material m)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                        HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7269/api/Materials/{m.id}");
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Материал успешно удален.");
                            LoadMaterials();
                        }
                        else
                        {
                            MessageBox.Show("Невозможно удалить материал, так как он используется в накладных.");
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
                MessageBox.Show("Выберите материал из таблицы для удаления.");
            }
        }
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Menu M = new Menu();
            M.Show();
            this.Close();
        }
    }
    public class Material
    {
        public int id { get; set; }
        public int iD_ed_izm { get; set; }
        public int iD_proizvoditelya { get; set; }
        public string naimenovanie { get; set; }
        public decimal cena { get; set; }
        public decimal kolichestvo { get; set; }
        public string photoPath { get; set; }
        public string ImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(photoPath)) return "";
                return $"pack://application:,,,/images/{photoPath}";
            }
        }
    }
}