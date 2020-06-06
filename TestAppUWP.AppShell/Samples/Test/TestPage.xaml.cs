using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Input;

namespace TestAppUWP.AppShell.Samples.Test
{
    public sealed partial class TestPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(async state =>
            {
                AppRestartFailureReason appRestartFailureReason = await CoreApplication.RequestRestartAsync("");
            });
        }
    }
}
