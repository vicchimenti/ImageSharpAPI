using Microsoft.Extensions.Logging;

namespace ImageProcessor.Processors
{
    public class FunctionalProcessorStrategy : IProcessorStrategy
    {
        public byte[] ProcessImage(string operations, byte[] imageData, ILoggerFactory loggerFactory)
        {
            var functionsParser = new FunctionParser(operations, loggerFactory);
            var functionsList = functionsParser.Parse();

            var processor = new FunctionProcessor(functionsList, loggerFactory);
            return processor.Execute(imageData);
        }
    }
}