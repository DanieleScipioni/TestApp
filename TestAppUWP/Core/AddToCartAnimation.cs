using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Core
{
    public class AddToCartAnimation : IDisposable
    {
        private readonly FrameworkElement _rootElement;
        private readonly Compositor _compositor;
        private readonly ContainerVisual _containerVisual;
        private readonly CanvasDevice _canvasDevice;
        private readonly CompositionGraphicsDevice _graphicsDevice;

        public AddToCartAnimation(FrameworkElement rootElement)
        {
            _rootElement = rootElement;
            _compositor = ElementCompositionPreview.GetElementVisual(_rootElement).Compositor;
            _containerVisual = _compositor.CreateContainerVisual();
            _canvasDevice = new CanvasDevice();
            _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _canvasDevice);
            ElementCompositionPreview.SetElementChildVisual(_rootElement, _containerVisual);
        }

        public void Dispose()
        {
            _compositor.Dispose();
            _containerVisual.Dispose();
            _canvasDevice.Dispose();
            _graphicsDevice.Dispose();
        }

        public async Task StartAnimation(FrameworkElement sourceElement, FrameworkElement targetElement)
        {
            Point point = sourceElement.TransformToVisual(_rootElement).TransformPoint(new Point(0, 0));

            CompositionDrawingSurface surface = await GetCompositionDrawingSurface(sourceElement);

            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateSurfaceBrush(surface);
            spriteVisual.Size = new Vector2((float)surface.Size.Width, (float)surface.Size.Height);
            spriteVisual.Offset = new Vector3((float)point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtBottom(spriteVisual);

            Vector3 targetOffset = GetTargetOffset(targetElement);
            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            SetAnimationDefautls(offsetAnimation);

            Vector2 targetSize = GetTargetSize(targetElement);
            Vector2KeyFrameAnimation sizeAnimation = _compositor.CreateVector2KeyFrameAnimation();
            SetAnimationDefautls(sizeAnimation);

            var newWidth = (float)(sourceElement.ActualWidth * 1.3);
            var newHeight = (float)(sourceElement.ActualHeight * 1.3);
            var newX = (float)(point.X - (newWidth - sourceElement.ActualWidth) / 2);
            var newY = (float)(point.Y - (newHeight - sourceElement.ActualHeight) / 2);

            const float normalizedProgressKey0 = 0.3f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector3(newX, newY, 0f));
            sizeAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector2(newWidth, newHeight));

            const float normalizedProgressKey1 = 1f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey1, targetOffset, _compositor.CreateLinearEasingFunction());
            sizeAnimation.InsertKeyFrame(normalizedProgressKey1, targetSize, _compositor.CreateLinearEasingFunction());

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            spriteVisual.StartAnimation("Offset", offsetAnimation);
            spriteVisual.StartAnimation("Size", sizeAnimation);
            myScopedBatch.End();

            void BatchCompleted(object source, CompositionBatchCompletedEventArgs args)
            {
                myScopedBatch.Completed -= BatchCompleted;
                myScopedBatch.Dispose();
                spriteVisual.Dispose();
                surface.Dispose();
                offsetAnimation.Dispose();
                sizeAnimation.Dispose();
            }
            myScopedBatch.Completed += BatchCompleted;

        }

        private async Task<CompositionDrawingSurface> GetCompositionDrawingSurface(FrameworkElement frameworkElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(frameworkElement);

            CanvasBitmap canvasBitmap = await GetCanvasBitmap(renderTargetBitmap);

            using (canvasBitmap)
            {
                return GetCompositionDrawingSurface(canvasBitmap);
            }
        }

        private CompositionDrawingSurface GetCompositionDrawingSurface(CanvasBitmap canvasBitmap)
        {
            CompositionDrawingSurface surface = _graphicsDevice.CreateDrawingSurface(
                new Size(canvasBitmap.Size.Width, canvasBitmap.Size.Height),
                DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Ignore);
            using (CanvasDrawingSession session = CanvasComposition.CreateDrawingSession(surface))
            {
                session.Clear(Color.FromArgb(0, 0, 0, 0));
                session.DrawImage(canvasBitmap,
                    new Rect(0, 0, surface.Size.Width, surface.Size.Height),
                    new Rect(0, 0, canvasBitmap.Size.Width, canvasBitmap.Size.Height));
            }
            return surface;
        }

        private async Task<CanvasBitmap> GetCanvasBitmap(RenderTargetBitmap renderTargetBitmap)
        {
            CanvasBitmap canvasBitmap;
            using (var stream = new InMemoryRandomAccessStream())
            {
                IBuffer buffer = await renderTargetBitmap.GetPixelsAsync();
                DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight, displayInformation.LogicalDpi, displayInformation.LogicalDpi,
                    buffer.ToArray());
                await encoder.FlushAsync();
                canvasBitmap = await CanvasBitmap.LoadAsync(_canvasDevice, stream);
            }
            return canvasBitmap;
        }

        private Vector3 GetTargetOffset(FrameworkElement frameworkElement)
        {
            Point targetPoint = frameworkElement.TransformToVisual(_rootElement)
                .TransformPoint(new Point(0, 0));
            return new Vector3((float)targetPoint.X, (float)targetPoint.Y, 0f);
        }

        private static Vector2 GetTargetSize(FrameworkElement frameworkElement)
        {
            return new Vector2((float)frameworkElement.ActualWidth, (float)frameworkElement.ActualHeight);
        }

        private static void SetAnimationDefautls(KeyFrameAnimation animation)
        {
            animation.Duration = TimeSpan.FromMilliseconds(700);
            animation.IterationBehavior = AnimationIterationBehavior.Count;
            animation.IterationCount = 1;
        }

    }
}
