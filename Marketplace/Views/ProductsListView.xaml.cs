using System.Collections.Generic;
using System.Windows.Controls;
using Marketplace.BusinessLogic.Models;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class ProductsListView : UserControl
    {
        public ProductsListView()
        {
            InitializeComponent();
        }

        public ProductsListView(MainViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }


        public ProductsListView(List<Product> products, MainViewModel viewModel) : this()
        {
            DataContext = viewModel;
            var itemsControl = FindName("ProductsItemsControl") as ItemsControl;
            if (itemsControl != null)
                itemsControl.ItemsSource = products;
        }
    }
}