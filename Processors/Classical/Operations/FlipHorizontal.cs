using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class FlipHorizontal : Operation
    {
        public FlipHorizontal(Match match) {}

        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            logger.LogInformation("flipping image horizontally");
            image.Mutate(i => i.Flip(FlipMode.Horizontal));
        }

        public override string ToString()
        {
            return "hflip";
        }
    }
}