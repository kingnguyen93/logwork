using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class ItemTappedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ItemTappedEventArgs eventArgs))
                return null;

            return eventArgs.Item;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}