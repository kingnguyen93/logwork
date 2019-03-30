using System;
using System.Globalization;
using System.IO;

namespace Xamarin.Forms.Converters
{
    public class Base64ToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string base64))
                return null;

            MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(base64));

            if (ms.CanSeek)
                ms.Seek(0, SeekOrigin.Begin);

            return ImageSource.FromStream(() => ms);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}