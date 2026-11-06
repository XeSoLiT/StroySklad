using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
            UserInfoLBL.Content = $"Сотрудник: {MainWindow.UserFIO} | Роль: {MainWindow.UserRole}";
            if (MainWindow.UserRole != "Администратор")
            {
                UsersBTN.Visibility = Visibility.Collapsed;
            }
        }
        private void MaterialsBTN_Click(object sender, RoutedEventArgs e)
        {
            Materials M = new Materials();
            M.Show();
            this.Close();
        }
        private void ClientsBTN_Click(object sender, RoutedEventArgs e)
        {
            Clients C = new Clients();
            C.Show();
            this.Close();
        }
        private void PostavshikiBTN_Click(object sender, RoutedEventArgs e)
        {
            Postavshiki P = new Postavshiki();
            P.Show();
            this.Close();
        }
        private void NakladnyeBTN_Click(object sender, RoutedEventArgs e)
        {
            Nakladnye N = new Nakladnye();
            N.Show();
            this.Close();
        }
        private void ReportBTN_Click(object sender, RoutedEventArgs e)
        {
            Report R = new Report();
            R.Show();
            this.Close();
        }
        private void UsersBTN_Click(object sender, RoutedEventArgs e)
        {
            Users U = new Users();
            U.Show();
            this.Close();
        }
        private void ExitBTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}