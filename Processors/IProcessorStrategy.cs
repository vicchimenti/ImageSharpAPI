using Microsoft.Extensions.Logging;

namespace ImageProcessor.Processors
{
    public interface IProcessorStrategy
    {
        byte[] ProcessImage(string operations, byte[] imageData, ILoggerFactory loggerFactory);
    }
}