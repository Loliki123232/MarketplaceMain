using System.Windows;
using System.Windows.Controls;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.Views
{
    public partial class ReturnHistoryView : UserControl
    {
        public ReturnHistoryView()
        {
            InitializeComponent();
        }

        private void ViewReason_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var returnRequest = button?.Tag as ReturnRequest;

            if (returnRequest != null)
            {
                MessageBox.Show($"Причина возврата:\n\n{returnRequest.Reason}",
                    $"Заявка №{returnRequest.Id}",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}