using TestAppUWP.Samples.RootNavigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace TestAppUWP
{
    sealed partial class App
    {

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

            UIElement rootContent = Window.Current.Content;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootContent == null)
            {
                var rootNavigationViewModel = new RootNavigationViewModel(e.PreviousExecutionState);
                rootContent = new RootNavigationPage { DataContext = rootNavigationViewModel };
                Window.Current.Content = rootContent;
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
