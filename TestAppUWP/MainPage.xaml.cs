using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP
{
    public sealed partial class MainPage
    {
        private readonly MainPageViewModel _mainPageViewModel;
        private readonly List<string> _a;

        public MainPage()
        {
            InitializeComponent();
            _mainPageViewModel = DataContext as MainPageViewModel;
            _a = new List<string> {"a", "b", "c"};
            Loaded += MainPage_Loaded;
            DataContextChanged += (sender, args) =>
            {
                //Bindings.Update();
            };
        }

        private static void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //var contentDialog = new ContentDialog
            //{
            //    Title = "tilte",
            //    Content = new AnUserControl(),
            //    PrimaryButtonText = "P",

            //    Style = Application.Current.Resources["ContentDialogStyle"] as Style,
            //};
            //await contentDialog.ShowAsync();
        }
    }
}
