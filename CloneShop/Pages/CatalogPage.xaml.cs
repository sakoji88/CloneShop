using CloneShop.ApplicationData;
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

namespace CloneShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
            if (AppConnect.CurrentUser != null)
            {
                tbWelcome.Text = "Добро пожаловать, " + AppConnect.CurrentUser.FullName;
            }
            if (AppConnect.CurrentUser.RoleID == 1)
            {
                tbRole.Text = "Роль: Администратор";
                btnAdmin.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                tbRole.Text = "Роль: Пользователь";
                btnAdmin.Visibility = System.Windows.Visibility.Collapsed;
            }
            lvProducts.ItemsSource = AppConnect.model01.Products.ToList();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            AppConnect.CurrentUser = null;
            AppFrame.frmMain.Navigate(new AutorizationPage());

        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Го в админку");
        }
    }
}
