using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Operations
{
    public class Grayscale : Operation
    {
        private float percent = 0;

        public Grayscale(Match match) 
        {
            if (match.Success == true)
            {
                try
                {
                    percent = Int32.Parse(match.Groups["percent"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    // this is a grayscale without a percentage, let it ride
                }
                catch (Exception)
                {
                    // something else is wrong, throw 
                    throw new InvalidOperationException(string.Format("Parameter for gray transformation was in an incorrect format"));
                }
            }
        }
 
        public void Run(Image<Rgba32> image, ILogger<Operation> logger)
        {
            if (percent > 0 && percent < 100)
            {
                logger.LogInformation("applying grayscale {0}", percent / 100.0F);
                image.Mutate(i => i.Grayscale(percent / 100.0F));
            }
            else
            {
                logger.LogInformation("applying grayscale");
                image.Mutate(i => i.Grayscale());
            }
        }

        public override string ToString()
        {
            return percent == 0 ? "gray" : "gray(" + percent + "%)";
        }
    }
}