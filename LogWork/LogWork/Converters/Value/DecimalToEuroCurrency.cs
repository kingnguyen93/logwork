using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class DecimalToEuroCurrency : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");

            return string.Format(cultureInfo, "€{0:###,###,###,##0.##}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}