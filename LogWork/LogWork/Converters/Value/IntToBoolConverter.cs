using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32(value) != 0;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)value ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}