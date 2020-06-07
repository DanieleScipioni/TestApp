using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using TestAppUWP.Logic.Logs;

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

        private async void RequestRestartAsync_OnClick(object sender, RoutedEventArgs e)
        {
            Logger logger = LogFactory.GetLogger(nameof(RequestRestartAsync_OnClick));

            await Task.Delay(5000);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () => {
                    try
                    {
                        AppRestartFailureReason arfr = await CoreApplication.RequestRestartAsync("");
                        if (arfr == AppRestartFailureReason.NotInForeground || arfr == AppRestartFailureReason.Other)
                        {
                            logger.Log($"AppRestartFailureReason {arfr}");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log($"AppRestartFailureReason Exception {ex.Message}");
                    }
                    logger.Flush();
                });
            ;
        }
    }
}
