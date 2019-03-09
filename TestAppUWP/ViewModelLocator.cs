using System;
using TestAppUWP.Pages.Frame;
using TestAppUWP.Samples.Accounts;
using TestAppUWP.Samples.ColorAnimation;
using TestAppUWP.Samples.Map;
using TestAppUWP.ViewModels.Frame;
using Windows.UI.Xaml.Data;

namespace TestAppUWP
{
    public class ViewModelLocator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case MainPage _:
                    return new MainPageViewModel();
                case FirstPage _:
                    return new FirstPageViewModel();
                case SecondPage _:
                    return new SecondPageViewModel();
                case FramePage _:
                    return new FramePageViewModel();
                case ColorAnimation _:
                    return new ColorAnimationViewModel();
                case MapPage _:
                    return new MapViewModel();
                case AccountPage _:
                    return  new AccountPageViewModel();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
