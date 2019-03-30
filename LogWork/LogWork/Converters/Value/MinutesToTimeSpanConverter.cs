using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class MinutesToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return TimeSpan.FromMinutes(System.Convert.ToDouble(value));
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
                if (value is TimeSpan time)
                {
                    return time.TotalMinutes;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}