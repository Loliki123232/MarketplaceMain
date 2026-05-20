using System.Windows;
using System.Windows.Controls;
using Marketplace.BusinessLogic.Models;
using Marketplace.BusinessLogic.Services;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class SelectProductForReturnView : UserControl
    {
        private readonly Order _order;
        private readonly MainViewModel _mainViewModel;

        public SelectProductForReturnView(Order order, MainViewModel mainViewModel)
        {
            InitializeComponent();
            _order = order;
            _mainViewModel = mainViewModel;
            DataContext = this;
        }

        public Order Order => _order;

        private void SelectProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderItem = button?.Tag as OrderItem;

            if (orderItem != null)
            {
                var validator = new ReturnValidator();
                var validation = validator.CanReturnProduct(_order, orderItem, orderItem.Quantity);

                if (!validation.IsValid)
                {
                    MessageBox.Show(validation.Message, "Возврат невозможен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var returnViewModel = new ReturnRequestViewModel(_order, orderItem, _mainViewModel);
                var returnView = new ReturnRequestView(returnViewModel);

                _mainViewModel.CurrentContent = returnView;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowOrders();
        }
    }
}