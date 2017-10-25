using Windows.UI.Xaml;

namespace TestAppUWP.Samples.CertTutorial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CertTutorial
    {
        private string _param;
        public static CertTutorial Instance { get; } = new CertTutorial();

        private CertTutorial()
        {
            InitializeComponent();
            Frame.Navigated += (sender, args) =>
            {
                TextBlock.Text = Frame.BackStack.Count.ToString();
            };
            Frame.Navigate(typeof(SamePage), _param);
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private void Go_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SamePage), _param);
            _param = null;
        }

        #region Navigation

        public void SetNavigationParam(string param)
        {
            _param = param;
        }

        #endregion
    }
}
