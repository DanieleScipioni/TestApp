using System;
using TestAppUWP.Samples.Accounts;
using TestAppUWP.Samples.Animations;
using TestAppUWP.Samples.Calendar;
using TestAppUWP.Samples.Controls;
using TestAppUWP.Samples.Map;
using TestAppUWP.Samples.Rsa;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.Samples.RootNavigation
{
    public class PageClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Type type)
            {
                return parameter as string == "symbol" ? ConvertSymbol(type) : type.Name;
            }
            return null;
        }

        private static string ConvertSymbol(Type type)
        {
            if (type == typeof(AnimationsOverviewPage)) return "\xE790";
            if (type == typeof(CertTutorial.CertTutorial)) return "\xEB95";
            if (type == typeof(InterControlAnimation.InterControlAnimation)) return "\xED5F";
            if (type == typeof(MapPage)) return "\xE909";
            if (type == typeof(GeoLocationPage)) return "\xE707";
            if (type == typeof(ControlsPage)) return "\xE7BC";
            if (type == typeof(CalendarExplorer)) return "\xE787";
            if (type == typeof(AccountPage)) return "\xE910";
            if (type == typeof(WebPage)) return "\xE701";
            return "\xF142";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
