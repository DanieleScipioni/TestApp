using System;
using TestAppUWP.Pages.Frame;
using TestAppUWP.Samples.ImplicitAnimation;
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
                case ImplicitAnimation _:
                    return new ImplicitAnimationViewModel();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
