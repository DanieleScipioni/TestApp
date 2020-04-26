﻿using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.AppShell.Core
{
    // ReSharper disable once InconsistentNaming
    public static class IBufferExtension
    {
        public static async Task<InMemoryRandomAccessStream> ToInMemoryRandomAccessStream(this IBuffer buffer)
        {
            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(buffer);
            stream.Seek(0);
            return stream;
        }

        public static async Task<BitmapImage> ToBitmapImage(this IBuffer buffer)
        {
            using (var stream = await buffer.ToInMemoryRandomAccessStream())
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
                return bitmapImage;
            }
        }
    }
}