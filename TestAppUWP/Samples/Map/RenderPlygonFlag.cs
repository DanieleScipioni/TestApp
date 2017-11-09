using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.Samples.Map
{
    public class RenderPlygonFlag : BaseRenderFlag
    {
        private readonly Polygon _polygon;
        private readonly TextBlock _textBlock;

        private Polygon _polygon1;
        private Polygon _polygon2;
        private Line _line1;
        private Line _line2;

        public RenderPlygonFlag()
        {
            _polygon = new Polygon
            {
                Points = new PointCollection { new Point(0, 0), new Point(36, 16), new Point(0, 32) },
                FillRule = FillRule.EvenOdd,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2.5
            };

            _textBlock = new TextBlock { FontSize = 14, CharacterSpacing = -50 };
            SetLeft(_textBlock, 2);
            SetTop(_textBlock, 5.5);

            Children.Add(_polygon);
            Children.Add(_textBlock);

            Width = Height = 0;

            CreateMulti();
        }

        private void CreateMulti()
        {
            _polygon1 = new Polygon
            {
                Points = new PointCollection { new Point(0, 0), new Point(36, 16), new Point(0, 32) },
                Stroke = _polygon.Stroke,
                StrokeThickness = _polygon.StrokeThickness
            };
            SetTop(_polygon1, 4); SetLeft(_polygon1, 1);

            _line1 = new Line
            {
                X1 = 37, Y1 = 17,
                X2 = -0.5, Y2 = 33.5,
                StrokeThickness = 1.5,
                Stroke = new SolidColorBrush(Colors.Gray)
            };
            SetTop(_line1, 2); SetLeft(_line1, 0.5);

            _polygon2 = new Polygon
            {
                Points = new PointCollection { new Point(0, 0), new Point(36, 16), new Point(0, 32) },
                Stroke = _polygon.Stroke,
                StrokeThickness = _polygon.StrokeThickness
            };
            SetTop(_polygon2, 2); SetLeft(_polygon2, 0.5);

            _line2 = new Line
            {
                X1 = _line1.X1, Y1 = _line1.Y1,
                X2 = _line1.X2, Y2 = _line1.Y2,
                StrokeThickness = _line1.StrokeThickness
            };
        }

        private void AddMulti()
        {
            if (Children.Count == 6) return;
            Children.Insert(0, _polygon1);
            Children.Insert(0, _line1);
            Children.Insert(0, _polygon2);
            Children.Insert(0, _line2);
        }

        private void RemomveMulti()
        {
            if (Children.Count != 6) return;
            Children.RemoveAt(0);
            Children.RemoveAt(0);
            Children.RemoveAt(0);
            Children.RemoveAt(0);
        }

        protected override Task GetRandomAccessStreamReferenceOverride(Color background, Color foregroud, string text,
            bool multi, AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall,
            int visitsCreateRecurringAppointments)
        {
            if (multi) AddMulti();
            else RemomveMulti();

            _polygon.Fill = new SolidColorBrush(background);
            _textBlock.Foreground = new SolidColorBrush(foregroud);
            _textBlock.Text = text + text;

            return Task.CompletedTask;
        }
    }
}
