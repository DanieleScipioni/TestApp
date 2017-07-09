using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Core
{
    // ReSharper disable once InconsistentNaming
    public static class UIElementExtension
    {
        public static async Task<BitmapImage> ToBitmapImage(this UIElement uiElement)
        {
            IBuffer buffer = await uiElement.RenderTargetBitmapBuffer();

            using (InMemoryRandomAccessStream stream = await buffer.ToInMemoryRandomAccessStream())
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
                return bitmapImage;
            }
        }

        public static async Task<InMemoryRandomAccessStream> ToInMemoryRandomAccessStream(this IBuffer buffer)
        {
            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(buffer);
            stream.Seek(0);
            return stream;
        }

        public static async Task<IBuffer> RenderTargetBitmapBuffer(this UIElement uiElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            return await renderTargetBitmap.GetPixelsAsync();
        }
    }
}
