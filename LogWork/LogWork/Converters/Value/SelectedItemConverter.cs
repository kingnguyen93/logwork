using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class SelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SelectedItemChangedEventArgs eventArgs))
                return null;

            return eventArgs.SelectedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}