using System;
using System.Globalization;
using System.IO;

namespace Xamarin.Forms.Converters
{
    public class StreamToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Stream stream))
                return null;

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            return ImageSource.FromStream(() => stream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}