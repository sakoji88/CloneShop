using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CloneShop.ApplicationData;

namespace CloneShop.Pages
{
    public partial class ProductDetailsPage : Page
    {
        private Products currentProduct;

        public ProductDetailsPage(Products product)
        {
            InitializeComponent();
            currentProduct = product;
            LoadProductData();
        }

        private void LoadProductData()
        {
            if (currentProduct == null)
                return;

            tbName.Text = currentProduct.ProductName;
            tbPrice.Text = "Цена: " + currentProduct.Price + " ₽";
            tbCategory.Text = "Категория: " + currentProduct.Categories.CategoryName;
            tbBrand.Text = "Бренд: " + currentProduct.Brands.BrandName;
            tbStatus.Text = "Статус: " + currentProduct.ProductStatuses.StatusName;
            tbQuantity.Text = "Остаток: " + currentProduct.QuantityInStock;
            tbDescription.Text = currentProduct.Description;

            LoadMainImage();
            LoadAdditionalImages();
        }

        private void LoadMainImage()
        {
            if (string.IsNullOrWhiteSpace(currentProduct.MainImage))
                return;

            string imagePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                currentProduct.MainImage);

            if (File.Exists(imagePath))
            {
                imgMain.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        private void LoadAdditionalImages()
        {
            var images = AppConnect.model01.ProductImages
                .Where(x => x.ProductID == currentProduct.ProductID)
                .ToList();

            List<string> imagePaths = new List<string>();

            foreach (var image in images)
            {
                string imagePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Images",
                    image.ImagePath);

                if (File.Exists(imagePath))
                {
                    imagePaths.Add(imagePath);
                }
            }

            lbImages.ItemsSource = imagePaths;
        }

        private void lbImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedImage = lbImages.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(selectedImage))
                return;

            imgMain.Source = new BitmapImage(new Uri(selectedImage));
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var userCart = AppConnect.model01.Carts
                .FirstOrDefault(x => x.UserID == AppConnect.CurrentUser.UserID);

            if (userCart == null)
            {
                MessageBox.Show("Корзина пользователя не найдена");
                return;
            }

            var existingCartItem = AppConnect.model01.CartItems
                .FirstOrDefault(x =>
                    x.CartID == userCart.CartID &&
                    x.ProductID == currentProduct.ProductID);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                CartItems newCartItem = new CartItems();
                newCartItem.CartID = userCart.CartID;
                newCartItem.ProductID = currentProduct.ProductID;
                newCartItem.Quantity = 1;
                newCartItem.PriceAtMoment = currentProduct.Price;

                AppConnect.model01.CartItems.Add(newCartItem);
            }

            AppConnect.model01.SaveChanges();

            MessageBox.Show("Товар добавлен в корзину");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.GoBack();
        }
    }
}