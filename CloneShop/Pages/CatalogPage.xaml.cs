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
            
            LoadCategories();
            LoadSort();
            LoadProducts();
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

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = AppConnect.model01.Products.ToList();

            if (cmbCategory.SelectedIndex > 0)
            {
                string selectedCategory = cmbCategory.SelectedItem.ToString();

                products = products.Where(x =>
                    x.Categories.CategoryName == selectedCategory
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                products = products.Where(x =>
                    x.ProductName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    (x.Description != null && x.Description.ToLower().Contains(txtSearch.Text.ToLower()))
                ).ToList();
            }
            if (cmbSort.SelectedIndex == 1)
            {
                products = products.OrderBy(x => x.Price).ToList();
            }
            else if (cmbSort.SelectedIndex == 2)
            {
                products = products.OrderByDescending(x => x.Price).ToList();
            }

            lvProducts.ItemsSource = products;
        }


        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }
        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Все категории");

            var categories = AppConnect.model01.Categories.ToList();

            foreach (var category in categories)
            {
                cmbCategory.Items.Add(category.CategoryName);
            }

            cmbCategory.SelectedIndex = 0;
        }
        private void LoadSort()
        {
            cmbSort.Items.Clear();

            cmbSort.Items.Add("Без сортировки");
            cmbSort.Items.Add("Цена по возрастанию");
            cmbSort.Items.Add("Цена по убыванию");

            cmbSort.SelectedIndex = 0;
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }
    }

}
