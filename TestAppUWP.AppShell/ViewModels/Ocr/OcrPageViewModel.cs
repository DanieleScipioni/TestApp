using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.ViewModels.Ocr
{
    public class OcrPageViewModel : BindableBase
    {
        public SoftwareBitmap Bitmap { get; private set; }

        private BitmapSource _imgSource;
        public BitmapSource ImgSource
        {
            get => _imgSource;
            private set
            {
                if (SetProperty(ref _imgSource, value))
                {
                    DefineItemsEnabled = DefineQuantityEnabled = value != null;
                }
            }
        }

        private bool _defineItemsEnabled;
        public bool DefineItemsEnabled
        {
            get => _defineItemsEnabled;
            set => SetProperty(ref _defineItemsEnabled, value);
        }

        private bool _defineQuantityEnabled;
        public bool DefineQuantityEnabled
        {
            get => _defineQuantityEnabled;
            set => SetProperty(ref _defineQuantityEnabled, value);
        }

        public bool OcrEnabled => _itemsRectangle != null && _quantityRectangle != null;

        private Rectangle _itemsRectangle;

        public Rectangle ItemsRectangle
        {
            get => _itemsRectangle;
            set
            {
                if (SetProperty(ref _itemsRectangle, value))
                {
                    OnPropertyChangedByName(nameof(OcrEnabled));
                }
            }
        }

        private Rectangle _quantityRectangle;

        public Rectangle QuantityRectangle
        {
            get => _quantityRectangle;
            set
            {
                if (SetProperty(ref _quantityRectangle, value))
                {
                    OnPropertyChangedByName(nameof(OcrEnabled));
                }
            }
        }

        public List<OcrWord> Items { get; set; }
        public List<OcrWord> Quantities { get; set; }

        public async Task LoadImage(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                Bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                var writeableBitmap = new WriteableBitmap(Bitmap.PixelWidth, Bitmap.PixelHeight);
                Bitmap.CopyToBuffer(writeableBitmap.PixelBuffer);
                ImgSource = writeableBitmap;
            }
        }
    }
}