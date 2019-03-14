using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageProcessor.Operations;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.Processors
{
    public class FunctionProcessor
    {
        private readonly IEnumerable<Action<Image<Rgba32>, ILogger<Operation>>> functionsList;

        private readonly ILoggerFactory loggerFactory;

        public FunctionProcessor(FunctionList functionsList, ILoggerFactory loggerFactory) 
        {
            this.functionsList = functionsList.Functions;
            this.loggerFactory = loggerFactory;
        }

        public byte[] Execute(byte[] sourceImage)
        {
            var logger = loggerFactory.CreateLogger<FunctionProcessor>();
            var commandLogger = loggerFactory.CreateLogger<Operation>();

            if (functionsList == null || functionsList.Count() == 0)
            {
                return sourceImage;
            }

            if (sourceImage == null)
            {
                return null;
            }

            var format = Image.DetectFormat(sourceImage);
            logger.LogInformation("received image, format is {0}", format);

            using (var image = Image.Load(sourceImage))
            {
                var size = image.Size();
                logger.LogInformation("image size is ({0}, {1})", size.Width, size.Height);

                foreach (var function in functionsList)
                {
                    logger.LogInformation("executing operation {0}", function);
                    function(image, commandLogger);
                }

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    return ms.ToArray();
                }
            }
        }
    }
}