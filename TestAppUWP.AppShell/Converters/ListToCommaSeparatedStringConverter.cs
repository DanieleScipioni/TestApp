using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.AppShell.Converters
{
    public class ListToCommaSeparatedStringConverter : IValueConverter
    {
        public static string ShowNullParameter = "ShowNull";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is IList list ? string.Join(", ", list) : (string) parameter == ShowNullParameter ? "null" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
