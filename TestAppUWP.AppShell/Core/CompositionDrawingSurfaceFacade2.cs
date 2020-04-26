using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.AppShell.Core
{
    public static class CompositionDrawingSurfaceFacade2
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
            IBuffer pixels = await renderTargetBitmap.GetPixelsAsync();

            using (var canvasDevice = new CanvasDevice())
            using (CompositionGraphicsDevice graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice))
            {
                float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                using (var canvasBitmap = CanvasBitmap.CreateFromBytes(canvasDevice, pixels.ToArray(),
                    renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight,
                    DirectXPixelFormat.B8G8R8A8UIntNormalized, dpi))
                {
                    CompositionDrawingSurface surface = graphicsDevice.CreateDrawingSurface(
                        new Size(renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight),
                        DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
                    using (CanvasDrawingSession session = CanvasComposition.CreateDrawingSession(surface))
                    {
                        session.DrawImage(canvasBitmap, 0, 0,
                            new Rect(0, 0, canvasBitmap.Size.Width, canvasBitmap.Size.Height));
                    }

                    return surface;
                }
            }
        }
    }
}
