using Windows.UI.Xaml;
using TestAppUWP.ViewModels.Frame;

namespace TestAppUWP.Pages.Frame
{
    public sealed partial class FirstPage
    {
        private FirstPageViewModel _viewModel;

        public FirstPage()
        {
            InitializeComponent();
            _viewModel = DataContext as FirstPageViewModel;

            Unloaded += (sender, args) =>
            {
                //Bindings.StopTracking();
                _viewModel = null;
            };
        }

        private void Increase1Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Increase1Command.Execute(null);
        }

        private void Increase2Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Increase2Command.Execute(null);
        }

        private void Decrease1Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Decrease1Command.Execute(null);
        }

        private void Decrease2Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Decrease2Command.Execute(null);
        }
    }
}
