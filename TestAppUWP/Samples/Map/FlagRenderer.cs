using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.Samples.Map
{
    public class FlagRenderer : Canvas, IDisposable
    {
        private readonly Polygon _polygon;
        private readonly TextBlock _textBlock;
        private readonly RenderTargetBitmap _renderTargetBitmap;
        private readonly DisplayInformation _displayInformation;
        private float _rawDpiX;
        private float _rawDpiY;
        private readonly List<InMemoryRandomAccessStream> _inMemoryRandomAccessStreams =
            new List<InMemoryRandomAccessStream>();
        private readonly Dictionary<string, RandomAccessStreamReference> _randomAccessStreamReferenceById =
            new Dictionary<string, RandomAccessStreamReference>();

        public FlagRenderer()
        {
            _renderTargetBitmap = new RenderTargetBitmap();

            _polygon = new Polygon
            {
                Points = new PointCollection {new Point(0, 0), new Point(35, 13), new Point(0, 26)},
                FillRule = FillRule.EvenOdd,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2
            };

            _textBlock = new TextBlock {FontSize = 14, CharacterSpacing = -50};
            SetLeft(_textBlock, 2);
            SetTop(_textBlock, 2.5);

            Children.Add(_polygon);
            Children.Add(_textBlock);

            Width = _polygon.ActualWidth;
            Height = _polygon.ActualHeight;

            _displayInformation = DisplayInformation.GetForCurrentView();
            _rawDpiX = _displayInformation.RawDpiX;
            _rawDpiY = _displayInformation.RawDpiY;
            _displayInformation.DpiChanged += OnDisplayInformationOnDpiChanged;
        }

        private void OnDisplayInformationOnDpiChanged(DisplayInformation displayInformation, object args)
        {
            _rawDpiX = _displayInformation.RawDpiX;
            _rawDpiY = _displayInformation.RawDpiY;
        }

        public async Task<RandomAccessStreamReference> GetFlag(Color background, Color foregroud, string text)
        {
            string id = GetId(background, foregroud, text);
            if (_randomAccessStreamReferenceById.ContainsKey(id)) return _randomAccessStreamReferenceById[id];

            _polygon.Fill = new SolidColorBrush(background);
            _textBlock.Foreground = new SolidColorBrush(foregroud);
            _textBlock.Text = text;

            var inMemoryRandomAccessStream = new InMemoryRandomAccessStream();
            await SaveUiElementToPngStream(inMemoryRandomAccessStream);
            _inMemoryRandomAccessStreams.Add(inMemoryRandomAccessStream);

            return _randomAccessStreamReferenceById[id] =
                RandomAccessStreamReference.CreateFromStream(inMemoryRandomAccessStream);
        }

        private static string GetId(Color background, Color foregroud, string text) =>
            $"{background}_{foregroud}_{text}";

        private async Task SaveUiElementToPngStream(IRandomAccessStream stream)
        {
            await _renderTargetBitmap.RenderAsync(this);
            IBuffer buffer = await _renderTargetBitmap.GetPixelsAsync();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                (uint)_renderTargetBitmap.PixelWidth, (uint)_renderTargetBitmap.PixelHeight,
                _rawDpiX, _rawDpiY, buffer.ToArray());
            await encoder.FlushAsync();
        }

        public void Dispose()
        {
            _displayInformation.DpiChanged -= OnDisplayInformationOnDpiChanged;
            foreach (InMemoryRandomAccessStream inMemoryRandomAccessStream in _inMemoryRandomAccessStreams)
            {
                inMemoryRandomAccessStream.Dispose();
            }
        }
    }
}
