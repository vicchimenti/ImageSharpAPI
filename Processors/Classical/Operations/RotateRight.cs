using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class RotateRight : Operation
    {
        public RotateRight(Match match) {}
 
        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            logger.LogInformation("rotating right");
            image.Mutate(i => i.Rotate(RotateMode.Rotate90));
        }

        public override string ToString()
        {
            return "right";
        }
    }
}