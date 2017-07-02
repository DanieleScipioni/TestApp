using MustacheDemo.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.ViewModels.Frame
{
    public class SecondPageViewModel : BindableBase
    {
        private HorizontalAlignment _horizontalAlignment;
        private VerticalAlignment _verticalAlignment;

        public HorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            private set => SetProperty(ref _horizontalAlignment, value);
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            private set => SetProperty(ref _verticalAlignment, value);
        }

        public DelegateCommand AlignmentCommand { get; }

        public SecondPageViewModel()
        {
            _horizontalAlignment = HorizontalAlignment.Center;
            _verticalAlignment = VerticalAlignment.Center;

            AlignmentCommand = new DelegateCommand(AlignmentCommandImpl);
        }

        public void AlignmentCommandClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            AlignmentCommandImpl(button?.CommandParameter);
        }

        public void AlignmentCommandImpl(object parameter)
        {
            var direction = parameter as string;
            switch (direction)
            {
                case "up":
                    if (_verticalAlignment == VerticalAlignment.Bottom) VerticalAlignment = VerticalAlignment.Center;
                    else if (_verticalAlignment == VerticalAlignment.Center) VerticalAlignment = VerticalAlignment.Top;
                    break;
                case "down":
                    if (_verticalAlignment == VerticalAlignment.Top) VerticalAlignment = VerticalAlignment.Center;
                    else if (_verticalAlignment == VerticalAlignment.Center) VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case "left":
                    if (_horizontalAlignment == HorizontalAlignment.Right) HorizontalAlignment = HorizontalAlignment.Center;
                    else if (_horizontalAlignment == HorizontalAlignment.Center) HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case "right":
                    if (_horizontalAlignment == HorizontalAlignment.Left) HorizontalAlignment = HorizontalAlignment.Center;
                    else if (_horizontalAlignment == HorizontalAlignment.Center) HorizontalAlignment = HorizontalAlignment.Right;
                    break;
            }
        }
    }
}