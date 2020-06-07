using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;

namespace TestAppUWP.AppShell.Samples.RootNavigation
{
    public sealed partial class RootNavigationPage
    {
        private RootNavigationViewModel _viewModel;

        public RootNavigationPage()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                var rootTestAppViewModel = args.NewValue as RootNavigationViewModel;
                if (_viewModel == rootTestAppViewModel) return;
                _viewModel = rootTestAppViewModel;
                Bindings.Update();
            };
        }

        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            HamburgerSplitView.IsPaneOpen = !HamburgerSplitView.IsPaneOpen;
        }

        private async void OpenLocalState_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }
    }
}
