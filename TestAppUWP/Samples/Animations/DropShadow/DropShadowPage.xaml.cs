using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Samples.Animations.DropShadow
{
    public sealed partial class DropShadowPage
    {
        public DropShadowPage()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                await DropShadowMethod1(compositor);
            };
        }

        private async Task DropShadowMethod1(Compositor compositor)
        {
            // Compare cart animation code with
            // https://stackoverflow.com/questions/38361081/uwp-composition-apply-opacity-mask-to-top-30px-of-a-listview

            Windows.UI.Composition.DropShadow dropShadow = compositor.CreateDropShadow();
            dropShadow.BlurRadius = 10f;
            dropShadow.Color = Colors.Black;
            dropShadow.Offset = new Vector3(10f, 10f, 0f);
            dropShadow.Mask = await A(Button, compositor);
            //dropShadow.SourcePolicy = CompositionDropShadowSourcePolicy.InheritFromVisualContent;

            LayerVisual layerVisual = compositor.CreateLayerVisual();

            SpriteVisual spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Shadow = dropShadow;

            ElementCompositionPreview.SetElementChildVisual(BorderExternal, spriteVisual);

            Visual buttonVisual = ElementCompositionPreview.GetElementVisual(Button);
            Visual borderVisual = ElementCompositionPreview.GetElementVisual(Border);

            Button.SizeChanged += (sender1, args1) =>
            {
                spriteVisual.Size = new Vector2((float) Button.ActualWidth, (float) Button.ActualHeight);
            };
        }

        private async Task<CompositionSurfaceBrush> A(UIElement uiElement, Compositor compositor)
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(uiElement);
            IBuffer pixels = await bitmap.GetPixelsAsync();

            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            var canvasDevice = new CanvasDevice();
            CompositionGraphicsDevice compositionDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

            CompositionDrawingSurface drawingSurface;
            using (CanvasBitmap canvasBitmap = CanvasBitmap.CreateFromBytes(
                    canvasDevice, pixels.ToArray(),
                    bitmap.PixelWidth, bitmap.PixelHeight,
                    DirectXPixelFormat.B8G8R8A8UIntNormalized, dpi))
            {
                drawingSurface =
                    compositionDevice.CreateDrawingSurface(
                        new Size(bitmap.PixelWidth, bitmap.PixelHeight),
                        DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
                using (CanvasDrawingSession session = CanvasComposition.CreateDrawingSession(drawingSurface))
                {
                    // here we draw just the part of the background image we wish to use to overlay
                    session.DrawImage(canvasBitmap, 0, 0,
                        new Rect(0, 0, canvasBitmap.Size.Width, canvasBitmap.Size.Height));
                }
            }

            return compositor.CreateSurfaceBrush(drawingSurface);
        }
    }
}
