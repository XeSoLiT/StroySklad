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
    public partial class Users : Window
    {
        public Users()
        {
            InitializeComponent();
            LoadUsers();
        }
        private async void LoadUsers()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MainWindow.UserToken);
                    HttpResponseMessage response = await client.GetAsync("https://localhost:7269/api/Users");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        UsersDataGrid.ItemsSource = JsonSerializer.Deserialize<List<User>>(json);
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
    public class User
    {
        public int id { get; set; }
        public string familiya { get; set; }
        public string imya { get; set; }
        public string otchestvo { get; set; }
        public string rol { get; set; }
        public string login { get; set; }
    }
}