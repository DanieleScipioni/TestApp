using TestAppUWP.ViewModels.Frame;

namespace TestAppUWP.Pages.Frame
{
    public sealed partial class FramePage
    {
        private readonly FramePageViewModel _viewModel;

        public FramePage()
        {
            InitializeComponent();
            _viewModel = DataContext as FramePageViewModel;
        }
    }
}
