using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class Resize : Operation
    {
        private int ratio = 0;

        public Resize(Match match) 
        {
            if (match.Success == true)
            {
                ratio = Int32.Parse(match.Groups["ratio"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
        }

        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            // we silently limit the resize "size-up" option to be 500%
            if (ratio > 0 && ratio < 500) 
            {
                logger.LogInformation("resize image {0}%", ratio);

                var size = image.Size();

                var width = (int)(size.Width * (ratio / 100.0));
                var height = (int)(size.Height * (ratio / 100.0));

                logger.LogInformation("old height={0}, width={1}", size.Height, size.Width);
                logger.LogInformation("new height={0}, width={1}", height, width);

                image.Mutate(i => i.Resize(width, height));
            }
        }

        public override string ToString()
        {
            return "resize(" + ratio + ")";
        }
    }
}