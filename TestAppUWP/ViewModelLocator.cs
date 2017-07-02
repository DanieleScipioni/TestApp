using System;
using Windows.UI.Xaml.Data;
using TestAppUWP.Pages.Frame;
using TestAppUWP.ViewModels;
using TestAppUWP.ViewModels.Frame;

namespace TestAppUWP
{
    public class ViewModelLocator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mainPage = value as MainPage;
            if (mainPage != null) return new MainPageViewModel();

            if (value is MustachePage) return null;
            
            if (value is FirstPage) return new FirstPageViewModel();
            if (value is SecondPage) return new SecondPageViewModel();

            if (value is FramePage) return new FramePageViewModel();
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
