using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
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
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Core
{
    public static class CompositionDrawingSurfaceFacade1
    {
        public static async Task<CompositionDrawingSurface> GetCompositionDrawingSurface(UIElement uiElement,
            Compositor compositor)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            return await GetCompositionDrawingSurface(compositor, renderTargetBitmap);
        }

        private static async Task<CompositionDrawingSurface> GetCompositionDrawingSurface(Compositor compositor, RenderTargetBitmap renderTargetBitmap)
        {
            using (var canvasDevice = new CanvasDevice())
            using (CompositionGraphicsDevice graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice))
            using (CanvasBitmap canvasBitmap = await GetCanvasBitmap(renderTargetBitmap, canvasDevice))
            {
                CompositionDrawingSurface surface = graphicsDevice.CreateDrawingSurface(
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
        }

        private static async Task<CanvasBitmap> GetCanvasBitmap(RenderTargetBitmap renderTargetBitmap, CanvasDevice canvasDevice)
        {
            CanvasBitmap canvasBitmap;
            using (var stream = new InMemoryRandomAccessStream())
            {
                IBuffer buffer = await renderTargetBitmap.GetPixelsAsync();
                DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth, (uint)renderTargetBitmap.PixelHeight,
                    displayInformation.LogicalDpi, displayInformation.LogicalDpi,
                    buffer.ToArray());
                await encoder.FlushAsync();
                canvasBitmap = await CanvasBitmap.LoadAsync(canvasDevice, stream);
            }
            return canvasBitmap;
        }
    }
}
