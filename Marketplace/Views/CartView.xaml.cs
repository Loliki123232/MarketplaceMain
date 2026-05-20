using System.Collections.ObjectModel;
using System.Windows.Controls;
using Marketplace.BusinessLogic.Models;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class CartView : UserControl
    {
        public CartView(ObservableCollection<OrderItem> cart, MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Привязываем корзину
            var itemsControl = FindName("CartItemsControl") as ItemsControl;
            if (itemsControl != null)
                itemsControl.ItemsSource = cart;
        }
    }
}