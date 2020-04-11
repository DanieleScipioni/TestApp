using System;
using Windows.UI.Xaml;

namespace TestAppUWP.Samples.CertTutorial
{
    public sealed partial class CertTutorial
    {
        public static CertTutorial Instance { get; private set; }

        public CertTutorial()
        {
            InitializeComponent();
            CertTutorialFrame.Navigated += (sender, args) => TextBlock.Text = CertTutorialFrame.BackStack.Count.ToString();
            CertTutorialFrame.Navigate(typeof(SamePage));
            Instance = this;

            Unloaded += (sender, args) => { Instance = null; };
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            if (CertTutorialFrame.CanGoBack) CertTutorialFrame.GoBack();
        }

        private void Go_OnClick(object sender, RoutedEventArgs e)
        {
            string param = null;
            if (CertTutorialFrame.Content is SamePage samePage)
            {
                param = samePage.Parameter;
            }
            CertTutorialFrame.Navigate(typeof(SamePage), param);
        }
    }
}
