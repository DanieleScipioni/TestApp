using TestAppUWP.Samples.CertTutorial;
using TestAppUWP.Samples.RootNavigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace TestAppUWP
{
    sealed partial class App
    {
        private RootNavigationViewModel _rootNavigationViewModel;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e) {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootContent = Window.Current.Content;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootContent == null)
            {
                _rootNavigationViewModel = new RootNavigationViewModel(e.PreviousExecutionState);
                rootContent = new RootNavigationPage { DataContext = _rootNavigationViewModel };
                Window.Current.Content = rootContent;
            }

            var type = typeof(SamePage);
            if (e.TileId.StartsWith(type.Name))
            {
                var landingPageType = typeof(CertTutorial);
                string landingParams = e.Arguments;
                _rootNavigationViewModel.SetLandingPage(landingPageType, landingParams);
            }

            if (e.PrelaunchActivated == false)
            {
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            ActivationKind activationKind = args.Kind;
        }
    }
}
