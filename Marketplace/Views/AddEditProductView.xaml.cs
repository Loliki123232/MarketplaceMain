using System.Collections.Generic;
using System.Windows.Controls;
using Marketplace.BusinessLogic.Models;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class AddEditProductView : UserControl
    {
        public AddEditProductView(Product? product, List<Category> categories, MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = new AddEditProductViewModel(product, categories, viewModel);
        }
    }
}