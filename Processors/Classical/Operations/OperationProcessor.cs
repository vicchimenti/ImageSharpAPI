using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageProcessor.Operations;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ImageProcessor.Processors
{
    public class OperationProcessor
    {
        private readonly IEnumerable<Operation> operationsList;

        private readonly ILoggerFactory loggerFactory;

        public OperationProcessor(OperationList operationsList, ILoggerFactory loggerFactory) 
        {
            this.operationsList = operationsList.Operations;
            this.loggerFactory = loggerFactory;
        }

        public byte[] Execute(byte[] sourceImage)
        {
            var logger = loggerFactory.CreateLogger<OperationProcessor>();
            var commandLogger = loggerFactory.CreateLogger<Operation>();

            if (operationsList == null || operationsList.Count() == 0)
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

                foreach (var operation in operationsList)
                {
                    logger.LogInformation("executing operation {0}", operation);
                    operation.Run(image, commandLogger);
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