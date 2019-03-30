using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class StringToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (TimeSpan.TryParse((string)value, out TimeSpan ts))
                    return ts;

                return TimeSpan.Zero;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is TimeSpan ts)
                    return ts.ToString(@"hh\:mm");

                return "00:00";
            }
            catch
            {
                return "00:00";
            }
        }
    }
}