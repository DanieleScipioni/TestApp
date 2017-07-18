using Windows.UI.Xaml;

namespace TestAppUWP.Samples.InterControlAnimation
{
    public sealed partial class InterControlAnimation
    {
        private static Control2 _control2;
        private static Control1 _control1;

        public InterControlAnimation()
        {
            InitializeComponent();
            ContentPresenter.Content = new Control1();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ContentPresenter.Content = ContentPresenter.Content is Control1 ? 
                (object) GetControl2() : GetControl1();
        }

        private static Control1 GetControl1()
        {
            return _control1 ?? (_control1 = new Control1());
        }

        private static Control2 GetControl2()
        {
            return _control2 ?? (_control2 = new Control2());
        }
    }
}
