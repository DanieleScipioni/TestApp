using System.Drawing;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace ImageOverlay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "ImageOverlay",
                Description = "ImageOverlay"
            };
            app.HelpOption("-?|-h|--help");
            app.Arguments.Add(new CommandArgument {Name = "image 1", MultipleValues = true});
            app.Arguments.Add(new CommandArgument {Name = "image 2"});

            int execute = app.Execute(args);

            if (execute != 0 || args.Length <= 1) return;

            args[0] = "C:\\Repos\\bitbucket\\UWP\\AppW8RT\\Assets\\speedy.icon-150-wide.scale-100.png";
            args[1] = "C:\\Repos\\bitbucket\\UWP\\AppW8RT\\Assets\\beta.png";
            if (File.Exists(args[0]) && File.Exists(args[1]))
            {
                Image image1 = Image.FromFile(args[0]);
                Image image2 = Image.FromFile(args[1]);
                using (Graphics g = Graphics.FromImage(image1))
                {
                    int x = image1.Width - image2.Width;
                    int y = image1.Height - image2.Height;
                    g.DrawImage(image2, x, 0);
                }

                image1.Save("C:\\Repos\\bitbucket\\UWP\\AppW8RT\\Assets\\test.png");
            }
            else
            {
                
            }
        }
    }
}
