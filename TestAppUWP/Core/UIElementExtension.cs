using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Core
{
    // ReSharper disable once InconsistentNaming
    public static class UIElementExtension
    {
        public static async Task<BitmapImage> GetBitmapImage(this UIElement uiElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            var bitmapImage = new BitmapImage();
            using (InMemoryRandomAccessStream stream = await GetBitmapImageStream(uiElement))
            {
                bitmapImage.SetSource(stream);
            }
            return bitmapImage;
        }

        public static async Task<InMemoryRandomAccessStream> GetBitmapImageStream(this UIElement uiElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(await renderTargetBitmap.GetPixelsAsync());
            stream.Seek(0);
            return stream;
        }
    }
}
