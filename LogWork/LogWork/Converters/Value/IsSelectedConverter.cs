using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class IsSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (parameter is ViewCell viewCell && (value == viewCell.BindingContext || value == viewCell.View?.BindingContext));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default;
        }
    }
}