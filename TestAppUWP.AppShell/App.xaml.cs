using System;
using System.Diagnostics;
using TestAppUWP.AppShell.Samples.RootNavigation;
using TestAppUWP.Samples.CertTutorial;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace TestAppUWP.AppShell
{
    sealed partial class App
    {
        private RootNavigationViewModel _rootNavigationViewModel;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args) {
#if DEBUG
            if (Debugger.IsAttached)
            {
                //DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            UIElement rootContent = Window.Current.Content;

            if (rootContent == null)
            {
                _rootNavigationViewModel = new RootNavigationViewModel(args.PreviousExecutionState);
                rootContent = new RootNavigationPage { DataContext = _rootNavigationViewModel };
                Window.Current.Content = rootContent;
            }

            Type type = typeof(SamePage);
            if (args.TileId.StartsWith(type.Name))
            {
                Type landingPageType = typeof(CertTutorial);
                string landingParams = args.Arguments;
                _rootNavigationViewModel.SetLandingPage(landingPageType, landingParams);
            }

            if (args.PrelaunchActivated) return;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Debugger.Break();
        }
    }
}
