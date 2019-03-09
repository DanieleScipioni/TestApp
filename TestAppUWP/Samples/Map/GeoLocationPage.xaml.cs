using Windows.UI.Xaml.Controls;

namespace TestAppUWP.Samples.Map
{
    public sealed partial class GeoLocationPage
    {
        private readonly GeoLocationPageViewModel _pageViewModel;

        public GeoLocationPage()
        {
            InitializeComponent();
            _pageViewModel = (GeoLocationPageViewModel) DataContext;
        }

        private void TextBox_OnTextChanging(TextBox textBox, TextBoxTextChangingEventArgs args)
        {
            if (!args.IsContentChanging) return;
            _pageViewModel.Address = textBox.Text;
        }
    }
}
