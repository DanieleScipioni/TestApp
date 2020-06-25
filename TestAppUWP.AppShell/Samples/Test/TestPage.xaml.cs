using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TestAppUWP.AppShell.Samples.Test
{
    public sealed partial class TestPage
    {
        private const string PivotSelectedindexKey = "Pivot.SelectedIndex";

        public TestPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            Application.Current.Suspending -= OnSuspending;
            SavePageState();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (ApplicationData.Current.LocalSettings.Containers.TryGetValue(nameof(TestPage), out ApplicationDataContainer container))
            {
                if (container.Values.TryGetValue(PivotSelectedindexKey, out object value) && value is int selectedIndex)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Pivot.SelectedIndex = selectedIndex < Pivot.Items.Count ? selectedIndex : Pivot.Items.Count - 1;
                }
            }

            Application.Current.Suspending += OnSuspending;
        }

        private void OnSuspending(object sender, SuspendingEventArgs args)
        {
            SavePageState();
        }

        private void SavePageState()
        {
            ApplicationDataContainer dataContainer = ApplicationData.Current.LocalSettings.CreateContainer(
                nameof(TestPage), ApplicationDataCreateDisposition.Always);
            dataContainer.Values[PivotSelectedindexKey] = Pivot.SelectedIndex;
        }
    }

    internal class Book
    {
        public string Title;
    }
}
