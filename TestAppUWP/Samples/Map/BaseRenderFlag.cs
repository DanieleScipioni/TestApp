using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Samples.Map
{
    public abstract class BaseRenderFlag : Canvas
    {
        private readonly RenderTargetBitmap _renderTargetBitmap;
        private DisplayInformation _displayInformation;
        private float _rawDpiX;
        private float _rawDpiY;
        private readonly List<InMemoryRandomAccessStream> _inMemoryRandomAccessStreams =
            new List<InMemoryRandomAccessStream>();
        private readonly Dictionary<string, RandomAccessStreamReference> _randomAccessStreamReferenceById =
            new Dictionary<string, RandomAccessStreamReference>();
        private bool _disposed;

        protected BaseRenderFlag()
        {
            Unloaded += (sender, args) => Dispose();
            Loaded += (sender, args) =>
            {
                _displayInformation = DisplayInformation.GetForCurrentView();
                _rawDpiX = _displayInformation.RawDpiX;
                _rawDpiY = _displayInformation.RawDpiY;
                _displayInformation.DpiChanged += OnDisplayInformationOnDpiChanged;
            };

            _renderTargetBitmap = new RenderTargetBitmap();

            Width = Height = 0;
        }
        private void OnDisplayInformationOnDpiChanged(DisplayInformation displayInformation, object args)
        {
            _rawDpiX = _displayInformation.RawDpiX;
            _rawDpiY = _displayInformation.RawDpiY;
        }

        public async Task<RandomAccessStreamReference> GetRandomAccessStreamReference(Color background, Color foregroud,
            string text, bool multi, AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall, int visitsCreateRecurringAppointments)
        {
            if (_disposed) return null;

            string id = GetId(background, foregroud, text, multi, appointmentFlag, isPhoneCall, visitsCreateRecurringAppointments);

            if (_randomAccessStreamReferenceById.ContainsKey(id)) return _randomAccessStreamReferenceById[id];

            await GetRandomAccessStreamReferenceOverride(background, foregroud, text, multi, appointmentFlag, isPhoneCall, visitsCreateRecurringAppointments);

            var inMemoryRandomAccessStream = new InMemoryRandomAccessStream();
            await SaveUiElementToPngStream(inMemoryRandomAccessStream);
            if (_disposed) return null;

            StorageFile storageFile =
                await ApplicationData.Current.LocalFolder.CreateFileAsync($"{id}.png",
                    CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream randomAccessStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(inMemoryRandomAccessStream.GetInputStreamAt(0),
                    randomAccessStream.GetOutputStreamAt(0));
            }

            _inMemoryRandomAccessStreams.Add(inMemoryRandomAccessStream);
            return _randomAccessStreamReferenceById[id] =
                RandomAccessStreamReference.CreateFromStream(inMemoryRandomAccessStream);
        }

        protected abstract Task GetRandomAccessStreamReferenceOverride(Color background, Color foregroud, string text, bool multi, AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall, int visitsCreateRecurringAppointments);

        private static string GetId(Color background, Color foregroud, string text, bool multi,
            AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall,
            int visitsCreateRecurringAppointments) =>
            $"{background}_{foregroud}_{text}_{multi}_{appointmentFlag}_{isPhoneCall}_{visitsCreateRecurringAppointments}";

        private async Task SaveUiElementToPngStream(IRandomAccessStream stream)
        {
            if (_disposed) return;
            try
            {
                await _renderTargetBitmap.RenderAsync(this);
            }
            catch (ArgumentException)
            {
                // FlagRendered removed from visual tree.
                return;
            }
            if (_renderTargetBitmap.PixelHeight == 0 || _renderTargetBitmap.PixelWidth == 0) return;
            if (_disposed) return;

            IBuffer buffer = await _renderTargetBitmap.GetPixelsAsync();
            if (_disposed) return;

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            if (_disposed) return;

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                (uint)_renderTargetBitmap.PixelWidth, (uint)_renderTargetBitmap.PixelHeight,
                _rawDpiX, _rawDpiY, buffer.ToArray());
            await encoder.FlushAsync();
        }

        private void Dispose()
        {
            _disposed = true;
            _displayInformation.DpiChanged -= OnDisplayInformationOnDpiChanged;
            foreach (InMemoryRandomAccessStream inMemoryRandomAccessStream in _inMemoryRandomAccessStreams)
            {
                inMemoryRandomAccessStream.Dispose();
            }
            _inMemoryRandomAccessStreams.Clear();
            _randomAccessStreamReferenceById.Clear();
        }
    }
}
