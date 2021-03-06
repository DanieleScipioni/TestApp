﻿using System;
using TestAppUWP.AppShell.Samples.Accounts;
using TestAppUWP.AppShell.Samples.Animations.ColorAnimation;
using TestAppUWP.AppShell.Samples.Chess;
using TestAppUWP.Pages.Frame;
using TestAppUWP.Samples.Animations.ColorAnimation;
using TestAppUWP.Samples.Map;
using TestAppUWP.Samples.Ocr;
using TestAppUWP.ViewModels.Frame;
using TestAppUWP.ViewModels.Ocr;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.AppShell
{
    public class ViewModelLocator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
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
                case GeoLocationPage _:
                    return new GeoLocationPageViewModel();
                case OcrPage _:
                    return new OcrPageViewModel();
                case ChessBoardPage _:
                    return new ChessBoardViewModel();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
