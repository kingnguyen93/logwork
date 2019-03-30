using System;
using System.Collections;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class IsEmptyDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if null then not visible
            if (value == null)
                return true;

            //if empty string then not visible
            if (value is string)
                return string.IsNullOrWhiteSpace((string)value);

            //if blank list not visible
            if (value is IList)
                return ((IList)value).Count == 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}