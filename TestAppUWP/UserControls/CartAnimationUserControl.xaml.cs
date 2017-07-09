using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Toolkit.Uwp.UI;
using MustacheDemo.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.UserControls
{
    public sealed partial class CartAnimationUserControl
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;

        private Compositor _compositor;
        private ContainerVisual _containerVisual;
        private CanvasDevice _canvasDevice;
        private CompositionGraphicsDevice _graphicsDevice;
        private Viewbox _viewbox;

        public CartAnimationUserControl()
        {
            DataContext = _cartAnimationViewModel = new CartAnimationViewModel();
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                _containerVisual = _compositor.CreateContainerVisual();
                ElementCompositionPreview.SetElementChildVisual(this, _containerVisual);
                _canvasDevice = new CanvasDevice();
                _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _canvasDevice);
                _viewbox = CartPlaceholder.FindDescendant<Viewbox>();
            };
            Unloaded += (sender, args) =>
            {
                _compositor.Dispose();
                _containerVisual.Dispose();
                _canvasDevice.Dispose();
                _graphicsDevice.Dispose();
            };
        }

        public async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null) return;

            DependencyObject dependencyObject = VisualTreeHelper.GetParent(frameworkElement);
            var image = (ContentPresenter) VisualTreeHelper.GetChild(dependencyObject, 0);

            Point point = image.TransformToVisual(this).TransformPoint(new Point(0,0));

            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();

            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(image);
            IBuffer buffer = await renderTargetBitmap.GetPixelsAsync();

            CompositionDrawingSurface surface;
            using (var stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight, displayInformation.LogicalDpi, displayInformation.LogicalDpi,
                    buffer.ToArray());
                await encoder.FlushAsync();

                using (CanvasBitmap canvasBitmap = await CanvasBitmap.LoadAsync(_canvasDevice, stream))
                {
                    surface = _graphicsDevice.CreateDrawingSurface(
                        new Size(renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight),
                        DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Ignore);
                    using (CanvasDrawingSession session = CanvasComposition.CreateDrawingSession(surface))
                    {
                        session.Clear(Color.FromArgb(0, 0, 0, 0));
                        session.DrawImage(canvasBitmap,
                            new Rect(0, 0, renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight),
                            new Rect(0, 0, canvasBitmap.Size.Width, canvasBitmap.Size.Height));
                    }
                }
            }
            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateSurfaceBrush(surface);
            spriteVisual.Size = new Vector2((float) frameworkElement.Width, (float)frameworkElement.Height);
            spriteVisual.Offset = new Vector3((float) point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtBottom(spriteVisual);

            Vector3 targetOffset = GetTargetOffset(_viewbox);
            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            SetAnimationDefautls(offsetAnimation);

            Vector2 targetSize = GetTargetSize(_viewbox);
            Vector2KeyFrameAnimation sizeAnimation = _compositor.CreateVector2KeyFrameAnimation();
            SetAnimationDefautls(sizeAnimation);

            var newWidth = (float) (image.ActualWidth * 1.3);
            var newHeight = (float) (image.ActualHeight * 1.3);
            var newX = (float) (point.X - (newWidth - image.ActualWidth) / 2);
            var newY = (float) (point.Y - (newHeight - image.ActualHeight) / 2);

            const float normalizedProgressKey0 = 0.3f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector3(newX, newY, 0f), _compositor.CreateLinearEasingFunction());
            sizeAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector2(newWidth, newHeight), _compositor.CreateLinearEasingFunction());

            const float normalizedProgressKey1 = 1f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey1, targetOffset, _compositor.CreateLinearEasingFunction());
            sizeAnimation.InsertKeyFrame(normalizedProgressKey1, targetSize, _compositor.CreateLinearEasingFunction());

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            spriteVisual.StartAnimation("Offset", offsetAnimation);
            spriteVisual.StartAnimation("Size", sizeAnimation);
            myScopedBatch.End();

            void BatchCompleted(object source, CompositionBatchCompletedEventArgs args)
            {
                spriteVisual.Dispose();
                surface.Dispose();
                myScopedBatch.Completed -= BatchCompleted;
                myScopedBatch.Dispose();
                offsetAnimation.Dispose();
                sizeAnimation.Dispose();
            }
            myScopedBatch.Completed += BatchCompleted;
        }

        private Vector3 GetTargetOffset(FrameworkElement frameworkElement)
        {
            Point targetPoint = frameworkElement.TransformToVisual(this)
                .TransformPoint(new Point(0, 0));
            return new Vector3((float) targetPoint.X, (float) targetPoint.Y, 0f);
        }

        private static Vector2 GetTargetSize(FrameworkElement frameworkElement)
        {
            return new Vector2((float) frameworkElement.ActualWidth, (float) frameworkElement.ActualHeight);
        }

        private static void SetAnimationDefautls(KeyFrameAnimation animation)
        {
            animation.Duration = TimeSpan.FromMilliseconds(700);
            animation.IterationBehavior = AnimationIterationBehavior.Count;
            animation.IterationCount = 1;
        }
    }

    public class StringItem : BindableBase
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;
        public string Text { get; }
        public Uri ImageUri { get; }

        public DelegateCommand Add { get; }

        public StringItem(string text, Uri imageUri, CartAnimationViewModel cartAnimationViewModel)
        {
            Text = text;
            ImageUri = imageUri;
            _cartAnimationViewModel = cartAnimationViewModel;
            Add = new DelegateCommand(AddExecute);
        }

        private void AddExecute(object parameter)
        {
            _cartAnimationViewModel.Add(this);
        }
        
    }

    public class CartAnimationViewModel : BindableBase
    {
        private string _count;
        public List<StringItem> ItemSource { get; }

        public string Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public CartAnimationViewModel()
        {
            _count = "0";

            ItemSource = new List<StringItem>();
            for (var i = 0; i < 46; i++)
            {
                string fileNumber = i.ToString("00");
                ItemSource.Add(new StringItem(fileNumber, new Uri($"ms-appx:///Assets/Photos/pic{fileNumber}.jpg"),
                    this));
            }
        }

        public void Add(StringItem stringItem)
        {
            if (int.TryParse(Count, out int count))
            {
                Count = (count + 1).ToString();
            }
        }
    }
}
