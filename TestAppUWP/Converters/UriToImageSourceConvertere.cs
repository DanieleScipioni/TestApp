using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Converters
{
    class UriToImageSourceConvertere : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var uri = (Uri)value;
            return new BitmapImage(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
