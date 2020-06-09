using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TestAppUWP.AppShell.Samples.RootNavigation;
using TestAppUWP.Logic.Logs;
using TestAppUWP.Samples.CertTutorial;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;

namespace TestAppUWP.AppShell
{
    sealed partial class App
    {
        private RootNavigationViewModel _rootNavigationViewModel;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += (sender, args) =>
            {
                Logger logger = LogFactory.GetLogger(nameof(UnhandledException));
                logger.Log($"Exception.Message {args.Exception.Message}");
                logger.Flush();
            };
            CoreApplication.UnhandledErrorDetected += (sender, args) =>
            {
                try
                {
                    // intentionally propagating exception to get the exception object that crashed the app.
                    args.UnhandledError.Propagate();
                }
                catch (Exception e)
                {
                    Logger logger = LogFactory.GetLogger(nameof(CoreApplication.UnhandledErrorDetected));
                    logger.Log($"Exception.Message {e.Message}");
                    logger.Flush();

                    // if we don't throw exception - app will not be crashed. We need to throw to not change the app behavior.
                    // known issue: stack trace will contain SDK methods from now on.
                    throw;
                }
            };
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Logger logger = LogFactory.GetLogger(nameof(TaskScheduler.UnobservedTaskException));
                logger.Log($"Exception.Message {args.Exception.Message}");
                logger.Flush();
            };
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            LogFactory.Init(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs"));

            Logger logger = LogFactory.GetLogger(nameof(OnLaunched));
            logger.Log($"PreviousExecutionState {args.PreviousExecutionState}");

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

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Logger logger = LogFactory.GetLogger(nameof(OnSuspending));
            logger.Log($"SuspendingOperation.Deadline {e.SuspendingOperation.Deadline}");
            logger.Flush();
        }
    }
}
