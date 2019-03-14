using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class RotateLeft : Operation
    {
        public RotateLeft(Match match) {}
 
        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            logger.LogInformation("rotating left");
            image.Mutate(i => i.Rotate(RotateMode.Rotate270));
        }

        public override string ToString()
        {
            return "left";
        }
    }
}