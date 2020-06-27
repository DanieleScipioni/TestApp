using System;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.AppShell.Samples.Chess
{
    public class YToTopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}