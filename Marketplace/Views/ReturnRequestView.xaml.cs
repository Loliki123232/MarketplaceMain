using System.Windows.Controls;
using Marketplace.ViewModels;

namespace Marketplace.Views
{
    public partial class ReturnRequestView : UserControl
    {
        public ReturnRequestView(ReturnRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}