using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TestAppUWP.View.CanvasDraw;
using TestAppUWP.ViewModels.Ocr;
using Windows.Foundation;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.Samples.Ocr
{
    public sealed partial class OcrPage
    {
        private readonly OcrPageViewModel _viewModel;

        private readonly ObservableCollection<OcrLine> _lines = new ObservableCollection<OcrLine>();

        private DrawRectangleOnCanvas _drawTool;

        private RectangleSelector _shapeSelector;

        public OcrPage()
        {
            InitializeComponent();
             _viewModel = (OcrPageViewModel) DataContext;
        }

        private void PreviewImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleTransform scaleTransform = ScaleTrasform();

            foreach (UIElement canvasChild in OverlayCanvas.Children)
            {
                if (canvasChild is Rectangle rectangle)
                {
                    var word = (OcrWord) rectangle.Tag;
                    if (word == null) continue;
                    Rect rect = scaleTransform.TransformBounds(word.BoundingRect);
                    rectangle.Width = rect.Width;
                    rectangle.Height = rect.Height;
                    Canvas.SetTop(rectangle, rect.Top);
                    Canvas.SetLeft(rectangle, rect.Left);
                }
            }

            //// Update image rotation center.
            //var rotate = OverlayCanvas.RenderTransform as RotateTransform;
            //if (rotate != null)
            //{
            //    rotate.CenterX = PreviewImage.ActualWidth / 2;
            //    rotate.CenterY = PreviewImage.ActualHeight / 2;
            //}
        }

        private async void LoadImage_OnClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" }
            };

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            ClearResults();

            await _viewModel.LoadImage(file);
        }

        private void ClearResults()
        {
            _lines.Clear();
            //OverlayCanvas.RenderTransform = null;
            OverlayCanvas.Children.Clear();
            //ExtractedTextBox.Text = String.Empty;
            //wordBoxes.Clear();
        }

        private ScaleTransform ScaleTrasform()
        {
            return new ScaleTransform
            {
                CenterX = 0,
                CenterY = 0,
                ScaleX = PreviewImage.ActualWidth / _viewModel.ImgSource.PixelWidth,
                ScaleY = PreviewImage.ActualHeight / _viewModel.ImgSource.PixelHeight
            };
        }

        private async void Ocr_OnClick(object sender, RoutedEventArgs e)
        {
            _drawTool?.Close();
            ClearResults();

            Rectangle itemsRectangle = _viewModel.ItemsRectangle;
            Rectangle quantityRectangle = _viewModel.QuantityRectangle;
            BitmapSource bitmapSource = _viewModel.ImgSource;

            if (bitmapSource == null || itemsRectangle == null || quantityRectangle == null) return;

            // Check if OcrEngine supports image resoulution.
            if (bitmapSource.PixelWidth > OcrEngine.MaxImageDimension ||
                bitmapSource.PixelHeight > OcrEngine.MaxImageDimension) return;

            OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            if (ocrEngine == null)
            {
                // Display message to user!
                return;
            }

            OcrResult ocrResult = await ocrEngine.RecognizeAsync(_viewModel.Bitmap);

            // Display recognized text.
            //ExtractedTextBox.Text = ocrResult.Text;

            if (ocrResult.TextAngle != null)
            {
                // If text is detected under some angle in this sample scenario we want to
                // overlay word boxes over original image, so we rotate overlay boxes.
                OverlayCanvas.RenderTransform = new RotateTransform
                {
                    Angle = (double)ocrResult.TextAngle,
                    CenterX = PreviewImage.ActualWidth / 2,
                    CenterY = PreviewImage.ActualHeight / 2
                };
            }

            ScaleTransform scaleTrasform = ScaleTrasform();

            var itemsRect = new Rect(Canvas.GetLeft(itemsRectangle), Canvas.GetTop(itemsRectangle), itemsRectangle.Width,
                itemsRectangle.Height);

            var quantityRect = new Rect(Canvas.GetLeft(quantityRectangle), Canvas.GetTop(quantityRectangle), quantityRectangle.Width,
                quantityRectangle.Height);


            var items = new List<OcrWord>();
            var quantities = new List<OcrWord>();
            foreach (OcrLine line in ocrResult.Lines)
            {
                //bool isHorizontal = IsHorizontal(line);

                foreach (OcrWord word in line.Words)
                {
                    Rect rect = scaleTrasform.TransformBounds(word.BoundingRect);

                    Rect intersectRect1 = itemsRect;
                    intersectRect1.Intersect(rect);

                    Rect intersectRect2 = quantityRect;
                    intersectRect2.Intersect(rect);

                    bool itemsWord = intersectRect1 == rect;
                    bool quantityWord = intersectRect2 == rect;

                    if (!itemsWord && !quantityWord) continue;

                    var overlay = new Rectangle
                    {
                        Fill = new SolidColorBrush(Color.FromArgb(125, 0, 0, 255)),
                        Width = rect.Width,
                        Height = rect.Height,
                        Tag = word
                    };
                    Canvas.SetTop(overlay, rect.Top);
                    Canvas.SetLeft(overlay, rect.Left);
                    OverlayCanvas.Children.Add(overlay);

                    if (itemsWord)
                    {
                        items.Add(word);
                    }
                    else
                    {
                        quantities.Add(word);
                    }
                }
                    
                _lines.Add(line);
            }

            _viewModel.Items = items;
            _viewModel.Quantities = quantities;
        }

        //private static bool IsHorizontal(OcrLine line)
        //{
        //    Rect lineRect = Rect.Empty;
        //    foreach (OcrWord word in line.Words)
        //    {
        //        lineRect.Union(word.BoundingRect);
        //    }
        //    return lineRect.Height > lineRect.Width;
        //}

        private async void ItemsArea_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayCanvas.Children.Remove(_viewModel.ItemsRectangle);
            _viewModel.ItemsRectangle = null;
            _viewModel.ItemsRectangle = await DrawRectangle(Colors.Yellow);
        }

        private async void QuantityArea_OnClick(object sender, RoutedEventArgs e)
        {
            OverlayCanvas.Children.Remove(_viewModel.QuantityRectangle);
            _viewModel.QuantityRectangle = null;
            _viewModel.QuantityRectangle = await DrawRectangle(Colors.Red);
        }

        private async Task<Rectangle> DrawRectangle(Color color)
        {
            _drawTool?.Close();
            _shapeSelector?.Close();
            _drawTool = new DrawRectangleOnCanvas(OverlayCanvas)
            {
                Stroke = new SolidColorBrush(color),
                Fill = new SolidColorBrush(Color.FromArgb(125, color.R, color.G, color.B))
            };
            Rectangle rectangle = await _drawTool.DrawRectangle();
            _shapeSelector = new RectangleSelector(rectangle, OverlayCanvas);
            return rectangle;
        }

        private void OverlayCanvas_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Point position = e.GetPosition(null);
            var rectangle = (Rectangle) VisualTreeHelper
                .FindElementsInHostCoordinates(position, OverlayCanvas).FirstOrDefault(r => r is Rectangle);
            _shapeSelector?.Close();
            if (rectangle != null) _shapeSelector = new RectangleSelector(rectangle, OverlayCanvas);
        }
    }
}
