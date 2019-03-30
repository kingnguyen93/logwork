using System;
using System.Globalization;
using Xamarin.Forms.Extensions;

namespace Xamarin.Forms.Converters
{
    public class TranslateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return TranslateExtension.GetValue((string)value);
            }
            catch
            {
                return default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}