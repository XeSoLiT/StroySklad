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
    public partial class Report : Window
    {
        public Report()
        {
            InitializeComponent();
            LoadReport();
        }
        private async void LoadReport()
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
                        var list = JsonSerializer.Deserialize<List<Material>>(json);
                        ReportDataGrid.ItemsSource = list;
                        decimal total = 0;
                        foreach (var item in list)
                        {
                            total += item.cena * item.kolichestvo;
                        }
                        TotalLabel.Content = $"Общая стоимость склада: {total:C}";
                    }
                }
            }
            catch { }
        }
        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Menu M = new Menu();
            M.Show();
            this.Close();
        }
    }
}