using System.Collections.ObjectModel;
using System.Windows.Controls;
using Marketplace.BusinessLogic.Models;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class OrderHistoryView : UserControl
    {
        public OrderHistoryView(ObservableCollection<Order> orders, MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}