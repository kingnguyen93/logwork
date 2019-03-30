using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class StringToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParse((string)value, out DateTime dt))
                return dt;

            return default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return dt;

            return default;
        }
    }
}