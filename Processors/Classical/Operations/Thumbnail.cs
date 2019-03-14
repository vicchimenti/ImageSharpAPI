using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class Thumbnail : Operation
    {
        private static float WIDTH = 75F;
        private static float HEIGHT = 100F;

        public Thumbnail(Match match) {}
 
        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            var size = image.Size();

            var newWidth = size.Width;
            var newHeight = size.Height;

            float widthRatio = size.Width / WIDTH;
            float heightRatio = size.Height / HEIGHT;

            if (widthRatio > heightRatio)
            {
                newWidth = (int)WIDTH;
                newHeight = (int)(size.Height / widthRatio);
            }
            else
            {
                newWidth = (int)(size.Width / heightRatio);
                newHeight = (int)HEIGHT;
            }

            logger.LogInformation("old height={0}, width={1}", size.Height, size.Width);
            logger.LogInformation("new height={0}, width={1}", newHeight, newWidth);

            logger.LogInformation("generating thumbnail of ({0}, {1})", newWidth, newHeight);

            image.Mutate(i => i.Resize(newWidth, newHeight));
        }

        public override string ToString()
        {
            return "thumb";
        }
    }
}