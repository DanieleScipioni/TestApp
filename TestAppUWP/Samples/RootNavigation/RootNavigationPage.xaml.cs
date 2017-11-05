using Windows.UI.Xaml;

namespace TestAppUWP.Samples.RootNavigation
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
    }
}
