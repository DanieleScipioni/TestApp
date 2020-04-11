using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.Converters
{
    public class ListToCommaSeparatedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is IList list ? string.Join(", ", list) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
