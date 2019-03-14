using Microsoft.Extensions.Logging;

namespace ImageProcessor.Processors
{
    public class ClassicalProcessorStrategy : IProcessorStrategy
    {
        public byte[] ProcessImage(string operations, byte[] imageData, ILoggerFactory loggerFactory)
        {
            var operationsParser = new OperationParser(operations, loggerFactory);
            var operationsList = operationsParser.Parse();

            var processor = new OperationProcessor(operationsList, loggerFactory);
            return processor.Execute(imageData);
        }
    }
}