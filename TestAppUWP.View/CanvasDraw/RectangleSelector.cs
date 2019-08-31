using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.View.CanvasDraw
{
    public class RectangleSelector
    {
        private readonly Rectangle _rectangle;
        private readonly Canvas _canvas;
        private readonly Ellipse _topLeftThumb;
        private readonly Ellipse _bottomRightThumb;
        private Ellipse _movingThumb;
        private readonly double _topLeftAdjustment;
        private readonly double _bottomRightAdjustment;

        public RectangleSelector(Rectangle rectangle, Canvas canvas)
        {
            _rectangle = rectangle;
            _canvas = canvas;
            double circleSize = _rectangle.StrokeThickness * 4;
            double borderSize = _rectangle.StrokeThickness / 2;
            double circleRadius = circleSize / 2;
            _topLeftThumb = new Ellipse
            {
                Fill = _rectangle.Stroke,
                Width = circleSize,
                Height = circleSize
            };
            _topLeftAdjustment = circleRadius - borderSize;
            Canvas.SetLeft(_topLeftThumb, Canvas.GetLeft(_rectangle) - _topLeftAdjustment);
            Canvas.SetTop(_topLeftThumb, Canvas.GetTop(_rectangle) - _topLeftAdjustment);
            Canvas.SetZIndex(_topLeftThumb, Canvas.GetZIndex(_rectangle) + 1);
            _canvas.Children.Add(_topLeftThumb);

            _bottomRightThumb = new Ellipse
            {
                Fill = _rectangle.Stroke,
                Width = circleSize,
                Height = circleSize
            };
            _bottomRightAdjustment = circleRadius + borderSize;
            Canvas.SetLeft(_bottomRightThumb, Canvas.GetLeft(_rectangle) + _rectangle.Width - _bottomRightAdjustment);
            Canvas.SetTop(_bottomRightThumb, Canvas.GetTop(_rectangle) + _rectangle.Height - _bottomRightAdjustment);
            Canvas.SetZIndex(_bottomRightThumb, Canvas.GetZIndex(_rectangle) + 1);
            _canvas.Children.Add(_bottomRightThumb);

            _topLeftThumb.PointerPressed += ThumbOnPointerPressed;
            _bottomRightThumb.PointerPressed += ThumbOnPointerPressed;
        }

        private void ThumbOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _canvas.PointerReleased += ThumbOnPointerReleased;
            _canvas.PointerMoved += ThumbOnPointerMoved;
            _canvas.PointerExited += ThumbOnPointerReleased;
            _movingThumb = (Ellipse)sender;
        }

        private void ThumbOnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _canvas.PointerReleased -= ThumbOnPointerReleased;
            _canvas.PointerMoved -= ThumbOnPointerMoved;
            _canvas.PointerExited -= ThumbOnPointerReleased;
        }

        private void ThumbOnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point point = e.GetCurrentPoint(_canvas).Position;
            if (_movingThumb == _topLeftThumb)
            {
                double currentX = Canvas.GetLeft(_rectangle);
                double nextWidth = _rectangle.Width - (point.X - currentX);
                if (nextWidth > 10)
                {
                    Canvas.SetLeft(_rectangle, point.X);
                    Canvas.SetLeft(_topLeftThumb, point.X - _topLeftAdjustment);
                    _rectangle.Width = nextWidth;
                }

                double currentY = Canvas.GetTop(_rectangle);
                double nextHeight = _rectangle.Height - (point.Y - currentY);
                if (nextHeight > 10)
                {
                    Canvas.SetTop(_rectangle, point.Y);
                    Canvas.SetTop(_topLeftThumb, point.Y - _topLeftAdjustment);
                    _rectangle.Height = nextHeight;
                }
            }
            else
            {
                double currentX = Canvas.GetLeft(_rectangle) + _rectangle.Width;
                double nextWidth = _rectangle.Width + (point.X - currentX);
                if (nextWidth > 10)
                {
                    _rectangle.Width = nextWidth;
                    Canvas.SetLeft(_bottomRightThumb, Canvas.GetLeft(_rectangle) + _rectangle.Width - _bottomRightAdjustment);
                }

                double currentY = Canvas.GetTop(_rectangle) + _rectangle.Height;
                double nextHeight = _rectangle.Height + (point.Y - currentY);
                if (nextHeight > 10)
                {
                    _rectangle.Height = nextHeight;
                    Canvas.SetTop(_bottomRightThumb, Canvas.GetTop(_rectangle) + _rectangle.Height - _bottomRightAdjustment);
                }
            }
        }

        public void Close()
        {
            _topLeftThumb.PointerPressed -= ThumbOnPointerPressed;
            _bottomRightThumb.PointerPressed -= ThumbOnPointerPressed;
            _canvas.Children.Remove(_topLeftThumb);
            _canvas.Children.Remove(_bottomRightThumb);
        }
    }
}
