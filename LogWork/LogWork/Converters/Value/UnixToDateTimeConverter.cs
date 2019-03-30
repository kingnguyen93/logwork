using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms.Extensions;

namespace Xamarin.Forms.Converters
{
    public class UnixToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (DateTimeExtension.TryParseToDateTime(System.Convert.ToDouble(value), out DateTime dt, true))
                    return dt;

                return default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException());
                return default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is DateTime dt)
                    return dt.ToUnixTimestamp();

                return default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException());
                return default;
            }
        }
    }
}