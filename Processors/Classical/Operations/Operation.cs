using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.Operations
{
    public interface Operation
    {
        void Run(Image<Rgba32> image, ILogger<Operation> logger);
    }
}