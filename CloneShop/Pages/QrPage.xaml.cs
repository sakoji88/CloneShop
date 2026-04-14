using CloneShop.ApplicationData;
using QRCoder;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace CloneShop.Pages
{
    public partial class QrPage : Page
    {
        private TelegramLinks activeTelegramLink;

        public QrPage()
        {
            InitializeComponent();
            LoadTelegramData();
        }

        private void LoadTelegramData()
        {
            activeTelegramLink = AppConnect.model01.TelegramLinks
                .FirstOrDefault(x => x.IsActive == true);

            if (activeTelegramLink == null)
            {
                MessageBox.Show("Активная Telegram-ссылка не найдена");
                return;
            }

            tbTelegramLink.Text = activeTelegramLink.Url;
            imgQr.Source = GenerateQrImage(activeTelegramLink.Url);
        }

        private void btnOpenTelegram_Click(object sender, RoutedEventArgs e)
        {
            if (activeTelegramLink == null)
            {
                MessageBox.Show("Ссылка не найдена");
                return;
            }

            Process.Start(activeTelegramLink.Url);
        }

        private BitmapImage GenerateQrImage(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);
            using (Bitmap bitmap = qrCode.GetGraphic(20))
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memory;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.GoBack();
        }
    }
}