using System;
using System.Globalization;

namespace Xamarin.Forms.Converters
{
    public class PriorityToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case 1:
                    return "LOW";

                case 2:
                    return "NORMAL";

                case 3:
                    return "HIGH";

                case 4:
                    return "URGENT";

                case 5:
                    return "IMMEDIATE";

                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "LOW":
                    return 1;

                case "NORMAL":
                    return 2;

                case "HIGH":
                    return 3;

                case "URGENT":
                    return 4;

                case "IMMEDIATE":
                    return 5;

                default:
                    return 0;
            }
        }
    }
}