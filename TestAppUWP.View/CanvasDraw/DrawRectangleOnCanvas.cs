using System.Threading.Tasks;
using Windows.Foundation;
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
        }

        #region Properties
        public SolidColorBrush Stroke { get; set; }
        public SolidColorBrush Fill { get; set; }

        #endregion

        public async Task<Rectangle> DrawRectangle()
        {
            _tcs = new TaskCompletionSource<Rectangle>();
            _canvas.PointerPressed += CanvasOnPointerPressed;
            return await _tcs.Task;
        }

        public void Close()
        {
            _canvas.PointerPressed -= CanvasOnPointerPressed;
            _canvas.PointerReleased -= CanvasOnPointerReleased;
            _canvas.PointerExited -= CanvasOnPointerReleased;
            _canvas.PointerMoved -= CanvasOnPointerMoved;
        }

        private void CanvasOnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Close();
            _tcs.SetResult(_rectangle);
        }

        private void CanvasOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _canvas.PointerReleased += CanvasOnPointerReleased;
            _canvas.PointerExited += CanvasOnPointerReleased;
            _canvas.PointerMoved += CanvasOnPointerMoved;

            Point currentPoint = e.GetCurrentPoint(_canvas).Position;
            _rectangle = new Rectangle {Fill = Fill, Stroke = Stroke, StrokeThickness = 2};
            Canvas.SetLeft(_rectangle, currentPoint.X);
            Canvas.SetTop(_rectangle, currentPoint.Y);
            _canvas.Children.Add(_rectangle);
        }

        private void CanvasOnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_rectangle == null) return;
            Point currentPoint = e.GetCurrentPoint(_canvas).Position;
            double nextWidth = currentPoint.X - Canvas.GetLeft(_rectangle);
            if (nextWidth > 10)_rectangle.Width = nextWidth;
            double nextHeight = currentPoint.Y - Canvas.GetTop(_rectangle);
            if (nextHeight > 10) _rectangle.Height = nextHeight;
        }
    }
}
