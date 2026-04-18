using CloneShop.ApplicationData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Microsoft.VisualBasic;
namespace CloneShop.Pages
{
    public partial class AddEditProductPage : Page
    {
        private Products currentProduct;

        public AddEditProductPage(Products product)
        {
            InitializeComponent();

            LoadComboBoxes();

            if (product == null)
            {
                currentProduct = new Products();
                tbTitle.Text = "Добавление товара";
            }
            else
            {
                currentProduct = product;
                tbTitle.Text = "Редактирование товара";

                txtName.Text = currentProduct.ProductName;
                txtDescription.Text = currentProduct.Description;
                txtPrice.Text = currentProduct.Price.ToString();
                txtQuantity.Text = currentProduct.QuantityInStock.ToString();

                cmbCategory.SelectedValue = currentProduct.CategoryID;
                cmbBrand.SelectedValue = currentProduct.BrandID;
                cmbStatus.SelectedValue = currentProduct.StatusID;
                txtImage.Text = currentProduct.MainImage;
                LoadPreviewImage(currentProduct.MainImage);
                LoadAdditionalImages();
            }
        }

        private void LoadPreviewImage(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return;

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", imageName);

            if (File.Exists(imagePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgPreview.Source = bitmap;
            }
        }

        private void LoadComboBoxes()
        {
            cmbCategory.ItemsSource = AppConnect.model01.Categories.ToList();
            cmbCategory.DisplayMemberPath = "CategoryName";
            cmbCategory.SelectedValuePath = "CategoryID";

            cmbBrand.ItemsSource = AppConnect.model01.Brands.ToList();
            cmbBrand.DisplayMemberPath = "BrandName";
            cmbBrand.SelectedValuePath = "BrandID";

            cmbStatus.ItemsSource = AppConnect.model01.ProductStatuses.ToList();
            cmbStatus.DisplayMemberPath = "StatusName";
            cmbStatus.SelectedValuePath = "StatusID";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string productName = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();
            string priceText = txtPrice.Text.Trim();
            string quantityText = txtQuantity.Text.Trim();
            string imageName = txtImage.Text.Trim();

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Введите название товара");
                txtName.Focus();
                return;
            }

            if (productName.Length < 2 || productName.Length > 150)
            {
                MessageBox.Show("Название товара должно быть от 2 до 150 символов");
                txtName.Focus();
                return;
            }

            if (description.Length > 1000)
            {
                MessageBox.Show("Описание слишком длинное. Максимум 1000 символов");
                txtDescription.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(priceText))
            {
                MessageBox.Show("Введите цену");
                txtPrice.Focus();
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Цена введена неверно");
                txtPrice.Focus();
                return;
            }

            if (price <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля");
                txtPrice.Focus();
                return;
            }

            if (price > 99999999)
            {
                MessageBox.Show("Цена слишком большая");
                txtPrice.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(quantityText))
            {
                MessageBox.Show("Введите количество товара");
                txtQuantity.Focus();
                return;
            }

            if (!int.TryParse(quantityText, out int quantity))
            {
                MessageBox.Show("Количество введено неверно");
                txtQuantity.Focus();
                return;
            }

            if (quantity < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным");
                txtQuantity.Focus();
                return;
            }

            if (quantity > 999999)
            {
                MessageBox.Show("Количество слишком большое");
                txtQuantity.Focus();
                return;
            }

            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию");
                return;
            }

            if (cmbBrand.SelectedValue == null)
            {
                MessageBox.Show("Выберите бренд");
                return;
            }

            if (cmbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус");
                return;
            }

            if (string.IsNullOrWhiteSpace(imageName))
            {
                MessageBox.Show("Выберите основное изображение");
                return;
            }

            if (imageName.Length > 255)
            {
                MessageBox.Show("Имя файла изображения слишком длинное");
                return;
            }

            currentProduct.ProductName = productName;
            currentProduct.Description = string.IsNullOrWhiteSpace(description) ? null : description;
            currentProduct.Price = price;
            currentProduct.QuantityInStock = quantity;
            currentProduct.CategoryID = (int)cmbCategory.SelectedValue;
            currentProduct.BrandID = (int)cmbBrand.SelectedValue;
            currentProduct.StatusID = (int)cmbStatus.SelectedValue;
            currentProduct.MainImage = imageName;

            try
            {
                if (currentProduct.ProductID == 0)
                {
                    AppConnect.model01.Products.Add(currentProduct);
                }

                AppConnect.model01.SaveChanges();

                var oldImages = AppConnect.model01.ProductImages
                    .Where(x => x.ProductID == currentProduct.ProductID)
                    .ToList();

                foreach (var oldImage in oldImages)
                {
                    AppConnect.model01.ProductImages.Remove(oldImage);
                }

                foreach (var additionalImageName in additionalImages)
                {
                    if (string.IsNullOrWhiteSpace(additionalImageName))
                        continue;

                    if (additionalImageName.Length > 255)
                        continue;

                    ProductImages newImage = new ProductImages();
                    newImage.ProductID = currentProduct.ProductID;
                    newImage.ImagePath = additionalImageName;

                    AppConnect.model01.ProductImages.Add(newImage);
                }

                AppConnect.model01.SaveChanges();

                MessageBox.Show("Товар сохранен");
                AppFrame.frmMain.GoBack();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                foreach (var entityErrors in ex.EntityValidationErrors)
                {
                    sb.AppendLine("Сущность: " + entityErrors.Entry.Entity.GetType().Name);

                    foreach (var validationError in entityErrors.ValidationErrors)
                    {
                        sb.AppendLine("Поле: " + validationError.PropertyName);
                        sb.AppendLine("Ошибка: " + validationError.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                MessageBox.Show(sb.ToString(), "Ошибка валидации");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.GoBack();
        }
        private void btnChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                string fileName = Path.GetFileName(dialog.FileName);
                string destinationPath = Path.Combine(imagesFolder, fileName);

                File.Copy(dialog.FileName, destinationPath, true);

                txtImage.Text = fileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(destinationPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgPreview.Source = bitmap;
            }
        }
        private void LoadAdditionalImages()
        {
            additionalImages = AppConnect.model01.ProductImages
                .Where(x => x.ProductID == currentProduct.ProductID)
                .Select(x => x.ImagePath)
                .ToList();

            lbAdditionalImages.ItemsSource = null;
            lbAdditionalImages.ItemsSource = additionalImages;
        }
        private void btnAddAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                string fileName = Path.GetFileName(dialog.FileName);
                string destinationPath = Path.Combine(imagesFolder, fileName);

                File.Copy(dialog.FileName, destinationPath, true);

                additionalImages.Add(fileName);

                lbAdditionalImages.ItemsSource = null;
                lbAdditionalImages.ItemsSource = additionalImages;
            }
        }
        private void btnRemoveAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            string selectedImage = lbAdditionalImages.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(selectedImage))
            {
                MessageBox.Show("Выберите изображение");
                return;
            }

            additionalImages.Remove(selectedImage);

            lbAdditionalImages.ItemsSource = null;
            lbAdditionalImages.ItemsSource = additionalImages;
        }
        private List<string> additionalImages = new List<string>();
        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string newCategoryName = Microsoft.VisualBasic.Interaction.InputBox (
                "Введите название новой категории:",
                "Добавление категории",
                "");

            if (string.IsNullOrWhiteSpace(newCategoryName))
                return;

            var existingCategory = AppConnect.model01.Categories
                .FirstOrDefault(x => x.CategoryName == newCategoryName);


            if (existingCategory != null)
            {
                MessageBox.Show("Такая категория уже существует");
                return;
            }
            if (string.IsNullOrWhiteSpace(newCategoryName))
                return;

            if (newCategoryName.Length > 100)
            {
                MessageBox.Show("Название слишком длинное. Максимум 100 символов.");
                return;
            }

                Categories newCategory = new Categories();
            newCategory.CategoryName = newCategoryName;

            AppConnect.model01.Categories.Add(newCategory);
            AppConnect.model01.SaveChanges();

            LoadComboBoxes();
            cmbCategory.SelectedValue = newCategory.CategoryID;

            MessageBox.Show("Категория добавлена");
        }
        private void btnAddBrand_Click(object sender, RoutedEventArgs e)
        {
            string newBrandName = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите название нового бренда:",
                "Добавление бренда",
                "");

            if (string.IsNullOrWhiteSpace(newBrandName))
                return;

            var existingBrand = AppConnect.model01.Brands
                .FirstOrDefault(x => x.BrandName == newBrandName);
            if (string.IsNullOrWhiteSpace(newBrandName))
                return;

            if (newBrandName.Length > 50)
            {
                MessageBox.Show("Название слишком длинное. Максимум 50 символов.");
                return;
            }
            if (existingBrand != null)
            {
                MessageBox.Show("Такой бренд уже существует");
                return;
            }


            Brands newBrand = new Brands();
            newBrand.BrandName = newBrandName;

            AppConnect.model01.Brands.Add(newBrand);
            AppConnect.model01.SaveChanges();

            LoadComboBoxes();
            cmbBrand.SelectedValue = newBrand.BrandID;

            MessageBox.Show("Бренд добавлен");
        }
    }
}