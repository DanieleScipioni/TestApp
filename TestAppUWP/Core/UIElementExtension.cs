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
        public static async Task<IBuffer> RenderTargetBitmapBuffer(this UIElement uiElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            return await renderTargetBitmap.GetPixelsAsync();
        }
    }
}
