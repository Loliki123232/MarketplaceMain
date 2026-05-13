using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Marketplace.BusinessLogic.Models;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class ProductCard : UserControl
    {
        public ProductCard()
        {
            InitializeComponent();
            Loaded += ProductCard_Loaded;
        }

        private void ProductCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Product product && !string.IsNullOrEmpty(product.ImageUrl))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(product.ImageUrl);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ProductImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    // Если ошибка, ставим картинку по умолчанию
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("https://picsum.photos/70/70?random=1");
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        ProductImage.Source = bitmap;
                    }
                    catch { }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as Product;

            if (product != null)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                var viewModel = mainWindow?.DataContext as MainViewModel;
                viewModel?.AddToCartCommand?.Execute(product);
            }
        }
    }
}