using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Buffer = Windows.Storage.Streams.Buffer;

namespace TestAppUWP.Samples.Map
{
    public class RenderFlag : Canvas
    {
        private readonly Polygon _polygon;
        private readonly TextBlock _textBlock;
        private readonly RenderTargetBitmap _renderTargetBitmap;
        private DisplayInformation _displayInformation;
        private float _rawDpiX;
        private float _rawDpiY;
        private readonly List<InMemoryRandomAccessStream> _inMemoryRandomAccessStreams =
            new List<InMemoryRandomAccessStream>();
        private readonly Dictionary<string, RandomAccessStreamReference> _randomAccessStreamReferenceById =
            new Dictionary<string, RandomAccessStreamReference>();

        private bool _disposed;
        private Polygon _polygon1;
        private Polygon _polygon2;
        private Line _line1;
        private Line _line2;

        public RenderFlag()
        {
            Unloaded += (sender, args) => Dispose();
            Loaded += (sender, args) =>
            {
                _displayInformation = DisplayInformation.GetForCurrentView();
                _rawDpiX = _displayInformation.RawDpiX;
                _rawDpiY = _displayInformation.RawDpiY;
                _displayInformation.DpiChanged += OnDisplayInformationOnDpiChanged;
            };

            _renderTargetBitmap = new RenderTargetBitmap();

            _polygon = new Polygon
            {
                Points = new PointCollection { new Point(0, 0), new Point(36, 16), new Point(0, 32) },
                FillRule = FillRule.EvenOdd,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2.5
            };

            //_textBlock = new TextBlock {FontSize = 14, CharacterSpacing = -50};
            //_textBlock = new TextBlock {FontSize = 15, CharacterSpacing = 0 };
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

        private void OnDisplayInformationOnDpiChanged(DisplayInformation displayInformation, object args)
        {
            _rawDpiX = _displayInformation.RawDpiX;
            _rawDpiY = _displayInformation.RawDpiY;
        }

        public async Task<RandomAccessStreamReference> GetRandomAccessStreamReference(Color background, Color foregroud,
            string text, bool multi)
        {
            if (_disposed) return null;

            if (multi) AddMulti(); else RemomveMulti();

            string id = GetId(background, foregroud, text, multi);

            if (_randomAccessStreamReferenceById.ContainsKey(id)) return _randomAccessStreamReferenceById[id];

            _polygon.Fill = new SolidColorBrush(background);
            _textBlock.Foreground = new SolidColorBrush(foregroud);
            _textBlock.Text = text + text;

            var inMemoryRandomAccessStream = new InMemoryRandomAccessStream();
            await SaveUiElementToPngStream(inMemoryRandomAccessStream);
            if (_disposed) return null;

            StorageFile storageFile =
                await ApplicationData.Current.LocalFolder.CreateFileAsync($"{id}.png",
                    CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream randomAccessStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(inMemoryRandomAccessStream.GetInputStreamAt(0),
                    randomAccessStream.GetOutputStreamAt(0));
            }

            _inMemoryRandomAccessStreams.Add(inMemoryRandomAccessStream);
            return _randomAccessStreamReferenceById[id] =
                RandomAccessStreamReference.CreateFromStream(inMemoryRandomAccessStream);
        }

        private static string GetId(Color background, Color foregroud, string text, bool multi) =>
            $"{background}_{foregroud}_{text}_{multi}";

        private async Task SaveUiElementToPngStream(IRandomAccessStream stream)
        {
            if (_disposed) return;
            try
            {
                await _renderTargetBitmap.RenderAsync(this);
            }
            catch (ArgumentException)
            {
                // FlagRendered removed from visual tree.
                return;
            }
            if (_disposed) return;

            IBuffer buffer = await _renderTargetBitmap.GetPixelsAsync();
            if (_disposed) return;

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            if (_disposed) return;

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                (uint)_renderTargetBitmap.PixelWidth, (uint)_renderTargetBitmap.PixelHeight,
                _rawDpiX, _rawDpiY, buffer.ToArray());
            await encoder.FlushAsync();
        }

        private void Dispose()
        {
            _disposed = true;
            _displayInformation.DpiChanged -= OnDisplayInformationOnDpiChanged;
            foreach (InMemoryRandomAccessStream inMemoryRandomAccessStream in _inMemoryRandomAccessStreams)
            {
                inMemoryRandomAccessStream.Dispose();
            }
            _inMemoryRandomAccessStreams.Clear();
            _randomAccessStreamReferenceById.Clear();
        }
    }
}
