using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.View.CanvasDraw
{
    public class DrawRectangleOnCanvas
    {
        private readonly Canvas _canvas;
        private Rectangle _rectangle;
        private TaskCompletionSource<Rectangle> _tcs;

        public DrawRectangleOnCanvas(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.Background = new SolidColorBrush(Colors.Transparent);
        }

        #region Properties
        public SolidColorBrush Stroke { get; set; }
        public SolidColorBrush Fill { get; set; }

        #endregion

        public async Task<Rectangle> DrawRectangle()
        {
            _tcs = new TaskCompletionSource<Rectangle>();
            _canvas.PointerPressed += CanvasOnPointerPressed;
            _canvas.PointerReleased += CanvasOnPointerReleased;
            _canvas.PointerExited += CanvasOnPointerReleased;
            _canvas.PointerMoved += CanvasOnPointerMoved;
            return await _tcs.Task;
        }

        private void CanvasOnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _canvas.Background = null;
            _canvas.PointerPressed -= CanvasOnPointerPressed;
            _canvas.PointerReleased -= CanvasOnPointerReleased;
            _canvas.PointerExited -= CanvasOnPointerReleased;
            _canvas.PointerMoved -= CanvasOnPointerMoved;
            _tcs.SetResult(_rectangle);
        }

        private void CanvasOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint currentPoint = e.GetCurrentPoint(_canvas);
            _rectangle = new Rectangle {Fill = Fill, Stroke = Stroke, StrokeThickness = 2};
            Canvas.SetLeft(_rectangle, currentPoint.Position.X);
            Canvas.SetTop(_rectangle, currentPoint.Position.Y);
            _canvas.Children.Add(_rectangle);
        }

        private void CanvasOnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_rectangle == null) return;
                PointerPoint currentPoint = e.GetCurrentPoint(_canvas);
            _rectangle.Width = currentPoint.Position.X - Canvas.GetLeft(_rectangle);
            _rectangle.Height = currentPoint.Position.Y - Canvas.GetTop(_rectangle);
        }
    }
}
