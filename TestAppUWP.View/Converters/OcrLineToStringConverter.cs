using System;
using System.Linq;
using Windows.Media.Ocr;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.View.Converters
{
    public class OcrLineToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var ocrLine = (OcrLine) value;
            return string.Join(" ", ocrLine.Words.Select(w => w.Text));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}