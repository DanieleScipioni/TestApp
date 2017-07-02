using System;
using Windows.UI.Core;
using MustacheDemo.Core;
using TestAppUWP.Pages.Frame;

namespace TestAppUWP.ViewModels.Frame
{
    public class FramePageViewModel : BindableBase
    {
        public Windows.UI.Xaml.Controls.Frame MainFrame { get; }

        public readonly DelegateCommand NavigateCommand;

        public int BackStack => MainFrame.BackStackDepth;

        public FramePageViewModel()
        {
            MainFrame = new Windows.UI.Xaml.Controls.Frame();
            string navigationState = MainFrame.GetNavigationState();
            MainFrame.Navigated += (sender, args) =>
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    MainFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            };
            MainFrame.Navigate(typeof(HomePage));
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            NavigateCommand = new DelegateCommand(NavigateCommandImpl, NavigateCommandCanExecute);
        }

        private static bool NavigateCommandCanExecute(object arg)
        {
            return true;
        }

        private void NavigateCommandImpl(object obj)
        {
            var s = obj as string;
            int i;
            if (!int.TryParse(s, out i)) return;
            Type targetPageType;
            switch (i)
            {
                default:
                    targetPageType = typeof(HomePage);
                    MainFrame.SetNavigationState("1,0");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    break;
                case 1:
                    targetPageType = typeof(FirstPage);
                    break;
                case 2:
                    targetPageType = typeof(SecondPage);
                    break;
            }

            if (MainFrame.SourcePageType == targetPageType) return;

            MainFrame.Navigate(targetPageType);
            OnPropertyChangedByName(nameof(BackStack));
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!MainFrame.CanGoBack) return;

            e.Handled = true;
            MainFrame.GoBack();
            OnPropertyChangedByName(nameof(BackStack));
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}