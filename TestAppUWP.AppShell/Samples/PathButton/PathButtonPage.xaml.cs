﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.AppShell.Samples.PathButton
{
    public sealed partial class PathButtonPage
    {
        public PathButtonPage()
        {
            InitializeComponent();

            SetupPath(IconFitWindow);
            SetupPath(IconOpen);
        }

        private static void SetupPath(Path path)
        {
            ContentPresenter contentControl = null;
            long token = 0;
            path.Loaded += (sender, args) =>
            {
                contentControl = (ContentPresenter) VisualTreeHelper.GetParent(path);
                path.Fill = contentControl.Foreground;
                token = contentControl.RegisterPropertyChangedCallback(ContentPresenter.ForegroundProperty,
                    (o, dp) => { path.Fill = contentControl.Foreground; });
            };
            path.Unloaded += (sender, args) =>
            {
                contentControl.UnregisterPropertyChangedCallback(ContentPresenter.ForegroundProperty, token);
            };
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            FitWindow.IsEnabled = !FitWindow.IsEnabled;
            Open.IsEnabled = !Open.IsEnabled;
        }
    }
}
