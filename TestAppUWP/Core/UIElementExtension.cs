using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Core
{
    // ReSharper disable once InconsistentNaming
    public static class UIElementExtension
    {
        public static async Task<IBuffer> RenderTargetBitmapBuffer(this UIElement uiElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            return await renderTargetBitmap.GetPixelsAsync();
        }

        public static async Task SaveUiElementToStream(this UIElement uiElement, IRandomAccessStream stream)
        {
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();

            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            IBuffer buffer = await renderTargetBitmap.GetPixelsAsync();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight, displayInformation.LogicalDpi, displayInformation.LogicalDpi,
                buffer.ToArray());
            await encoder.FlushAsync();
        }

        public static async Task SaveUiElementToPngStream(this UIElement uiElement, IRandomAccessStream stream)
        {
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();

            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            IBuffer buffer = await renderTargetBitmap.GetPixelsAsync();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight, displayInformation.LogicalDpi, displayInformation.LogicalDpi,
                buffer.ToArray());
            await encoder.FlushAsync();
        }
    }
}
