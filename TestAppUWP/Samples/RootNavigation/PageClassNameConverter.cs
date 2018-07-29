using System;
using TestAppUWP.Samples.Animations;
using TestAppUWP.Samples.BlankPage;
using TestAppUWP.Samples.CartAnimation;
using TestAppUWP.Samples.Map;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.Samples.RootNavigation
{
    public class PageClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Type type)
            {
                if (parameter as string == "symbol") return ConvertSymbol(type);
                return type.Name;
            }
            return null;
        }

        private static string ConvertSymbol(Type type)
        {
            if (type == typeof(AnimationsOverviewPage)) return "\xE790";
            if (type == typeof(CertTutorial.CertTutorial)) return "\xEB95";
            if (type == typeof(InterControlAnimation.InterControlAnimation)) return "\xED5F";
            if (type == typeof(MapPage)) return "\xE909";
            if (type == typeof(BigDynamicListPage)) return "\xE7BC";
            return "\xF142";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
