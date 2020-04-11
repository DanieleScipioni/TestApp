using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace TestAppUWP.Samples.CertTutorial
{
    public class OrientationStateTrigger : StateTriggerBase, IDisposable
    {
        public OrientationStateTrigger()
        {
            Window.Current.SizeChanged += OnWindowOnSizeChanged;
            NewMethod();
        }

        private void NewMethod() => SetActive(ApplicationView.GetForCurrentView().Orientation == _applicationViewOrientation);

        private void OnWindowOnSizeChanged(object sender, WindowSizeChangedEventArgs args) => NewMethod();

        private ApplicationViewOrientation _applicationViewOrientation;
        public ApplicationViewOrientation ApplicationViewOrientation
        {
            set => _applicationViewOrientation = value;
        }

        public void Dispose()
        {
            Window.Current.SizeChanged -= OnWindowOnSizeChanged;
        }
    }
}
