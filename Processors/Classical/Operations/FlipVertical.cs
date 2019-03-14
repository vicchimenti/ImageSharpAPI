using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class FlipVertical : Operation 
    {
        public FlipVertical(Match match) {}
    
        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            logger.LogInformation("flipping image vertically");
            image.Mutate(i => i.Flip(FlipMode.Vertical));
        }

        public override string ToString()
        {
            return "vflip";
        }
    }
}