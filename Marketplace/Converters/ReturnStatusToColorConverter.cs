using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Marketplace.BusinessLogic.Models;

namespace Marketplace.Converters
{
    public class ReturnStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;

            return status switch
            {
                ReturnStatuses.Created => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107")), // Жёлтый
                ReturnStatuses.Approved => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28A745")), // Зелёный
                ReturnStatuses.Rejected => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC3545")), // Красный
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}