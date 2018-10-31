using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageOverlay
{
    public class Program
    {
        private const string Usage = @"ImageOverlay <source image> <overlay image>";

        public static void Main(string[] args)
        {
            if (args.Length < 2) Console.WriteLine(Usage);

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
            string tmpSavePath = $"C:\\Repos\\bitbucket\\UWP\\AppW8RT\\Assets\\VisualAssets\\tmp.{Path.GetExtension(sourcePath)}";
            using (Image image1 = Image.FromFile(sourcePath))
            using (Image image2 = Image.FromFile(overlayPath))
            using (Graphics g = Graphics.FromImage(image1))
            {
                float x = (image1.Width / image1.HorizontalResolution - image2.Width / image2.HorizontalResolution) *
                          image1.HorizontalResolution;
                g.DrawImage(image2, x, 0);

                image1.Save(tmpSavePath, GetEncoder(image1.RawFormat), GetEncoderParameters());
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
    }
}
