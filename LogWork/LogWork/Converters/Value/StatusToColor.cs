using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class StatusToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case 1:
                    return Color.Green;

                case 2:
                    return Color.Black;

                case 3:
                    return Color.Orange;

                case 4:
                    return Color.IndianRed;

                case 5:
                    return Color.Red;

                default:
                    return Color.Default;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default;
        }
    }
}