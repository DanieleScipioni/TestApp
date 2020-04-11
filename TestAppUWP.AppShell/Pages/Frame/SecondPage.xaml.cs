using TestAppUWP.ViewModels.Frame;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.Pages.Frame
{
    public sealed partial class SecondPage
    {
        private SecondPageViewModel _viewModel;

        public SecondPage()
        {
            InitializeComponent();
            _viewModel = DataContext as SecondPageViewModel;

            Unloaded += (sender, args) =>
            {
                //Bindings.StopTracking();
                _viewModel = null;
            };
        }

        private void AlignmentCommandClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            _viewModel.AlignmentCommandImpl(button?.CommandParameter);
        }
    }
}
