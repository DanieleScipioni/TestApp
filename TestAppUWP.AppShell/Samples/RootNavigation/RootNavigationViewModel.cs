﻿using System;
using System.Collections.Generic;
using TestAppUWP.AppShell.Samples.Accounts;
using TestAppUWP.AppShell.Samples.Calendar;
using TestAppUWP.AppShell.Samples.PathButton;
using TestAppUWP.Core;
using TestAppUWP.Samples.Animations;
using TestAppUWP.Samples.Controls;
using TestAppUWP.Samples.Map;
using TestAppUWP.Samples.Ocr;
using TestAppUWP.Samples.Rsa;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace TestAppUWP.AppShell.Samples.RootNavigation
{
    internal class RootNavigationViewModel : BindableBase
    {
        private const string NavigationState = "RootNavigationState";
        private const string LastPageType = "LastPage";

        public Frame RootFrame { get; }

        public List<Type> Pages { get; }

        private int _selectedPageIndex;

        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set
            {
                if (!SetProperty(ref _selectedPageIndex, value)) return;
                Type sourcePageType = Pages[_selectedPageIndex];
                RootFrame.Navigate(sourcePageType);
#if DEBUG
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
#endif
            }
        }

        public RootNavigationViewModel(ApplicationExecutionState previousExecutionState)
        {
            // Set _selectedPageIndex to -1 so that setting SelectedPageIndex property 
            // with any values >= 0 is processed correclty.
            _selectedPageIndex = -1;

            Pages = new List<Type>
            {
                typeof(AnimationsOverviewPage),
                typeof(TestAppUWP.Samples.CertTutorial.CertTutorial),
                typeof(TestAppUWP.Samples.InterControlAnimation.InterControlAnimation),
                typeof(MapPage),
                typeof(GeoLocationPage),
                typeof(ControlsPage),
                typeof(WebPage),
                typeof(PathButtonPage),
                typeof(CalendarExplorer),
                typeof(AccountPage),
                typeof(OcrPage)
            };

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            RootFrame = new Frame
            {
                ContentTransitions = new TransitionCollection
                {
                    new NavigationThemeTransition {DefaultNavigationTransitionInfo = new DrillInNavigationTransitionInfo()}
                }
            };
            RootFrame.Navigated += (sender, args) =>
            {
                Type pageType = args.SourcePageType;
                localSettings.Values[LastPageType] = pageType.ToString();
                _selectedPageIndex = Pages.IndexOf(pageType);
                OnPropertyChangedByName(nameof(SelectedPageIndex));

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = RootFrame.CanGoBack
                    ? AppViewBackButtonVisibility.Visible
                    : AppViewBackButtonVisibility.Collapsed;
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) =>
            {
                if (!RootFrame.CanGoBack) return;

                RootFrame.GoBack();
                args.Handled = true;
#if DEBUG
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
#endif
            };

            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                var navigationState = (string) localSettings.Values[NavigationState];
                SetNavigationState(navigationState);
            }
            else
            {
                var lastPageType = (string) localSettings.Values[LastPageType];
                if (lastPageType == null)
                {
                    SelectedPageIndex = 0;
                }
                else
                {
                    Type pageType = Type.GetType(lastPageType);
                    SelectedPageIndex = pageType == null ? 0 : Pages.IndexOf(pageType);
                }
            }

            Application.Current.Suspending += (sender, args) =>
            {
                string navigationState = RootFrame.GetNavigationState();
                localSettings.Values[NavigationState] = navigationState;
            };
        }

        private void SetNavigationState(string navigationState)
        {
            RootFrame.SetNavigationState(navigationState);
            Type type = RootFrame.SourcePageType;
            int indexOf = Pages.IndexOf(type);
            if (indexOf != -1) _selectedPageIndex = indexOf;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = RootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        public void SetLandingPage(Type type, object parameter)
        {
            int indexOf = Pages.IndexOf(type);
            if (indexOf == -1) return;

            _selectedPageIndex = indexOf;
            RootFrame.Navigate(type, parameter);
            OnPropertyChangedByName(nameof(SelectedPageIndex));
        }
    }
}