using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageOverlay
{
    public class Program
    {
        private const string Usage = @"ImageOverlay <source image> <overlay image>";

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(Usage);
                return;
            }

            args[0] = "C:\\Repos\\bitbucket\\UWP\\AppW8RT\\Assets\\VisualAssets\\App.Tile.*";
            args[1] = "C:\\Repos\\bitbucket\\UWP\\AppW8RT\\Assets\\VisualAssets\\beta.png";
            if (File.Exists(args[0]))
            {
                AddOverlay(args[0], args[1]);
            }
            else
            {
                string directoryName = Path.GetDirectoryName(args[0]);
                string fileName = Path.GetFileName(args[0]);
                ProcessDirectory(directoryName, fileName, args[1]);
            }
        }

        private static void ProcessDirectory(string directoryPath, string searchPattern, string overlayPath)
        {
            IEnumerable<string> enumerateFiles = Directory.EnumerateFiles(directoryPath, searchPattern);
            foreach (string filePath in enumerateFiles)
            {
                AddOverlay(filePath, overlayPath);
            }
        }

        private static void AddOverlay(string sourcePath, string overlayPath)
        {
            string tmpSavePath = Path.Combine(Path.GetDirectoryName(sourcePath), $"tmp{ Path.GetExtension(sourcePath)}");

            using (Image targetImage = Image.FromFile(sourcePath))
            using (Image overlayImage = Image.FromFile(overlayPath))
            using (Graphics g = Graphics.FromImage(targetImage))
            {
                double resizePercent = (double) targetImage.Height / overlayImage.Height;

                Bitmap resizeImage = ResizeImage(overlayImage, (int) (overlayImage.Width * resizePercent), (int) (overlayImage.Height * resizePercent));
                float x = (targetImage.Width / targetImage.HorizontalResolution - resizeImage.Width / resizeImage.HorizontalResolution) *
                          targetImage.HorizontalResolution;
                g.DrawImage(resizeImage, x, 0);

                targetImage.Save(tmpSavePath, GetEncoder(targetImage.RawFormat), GetEncoderParameters());
            }
            File.Delete(sourcePath);
            File.Move(tmpSavePath, sourcePath);
        }

        private static EncoderParameters GetEncoderParameters()
        {
            return new EncoderParameters(1) {Param = {[0] = new EncoderParameter(Encoder.Quality, 100L)}};
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
