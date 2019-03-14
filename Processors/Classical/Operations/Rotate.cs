using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class Rotate : Operation
    {
        private float degrees = 0;

        public Rotate(Match match) 
        {
            if (match.Success == true)
            {
                degrees = Int32.Parse(match.Groups["degrees"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

                // we'll limit this to a single turn of the wheel, there's no point
                // in doing anything else
                degrees %= 360;
            }
        }
 
        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            if (degrees > -360 && degrees < 360) 
            {
                logger.LogInformation("rotate image {0}°", degrees);

                image.Mutate(i => i.Rotate(degrees));
            }
        }

        public override string ToString()
        {
            return "rotate(" + degrees + ")";
        }
    }
}