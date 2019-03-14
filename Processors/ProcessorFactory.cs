namespace ImageProcessor.Processors
{
    public static class ProcessorFactory
    {
        public static IProcessorStrategy GetProcessor(string strategy)
        {
            // default to the classical strategy
            if (string.IsNullOrEmpty(strategy) == true)
            {
                strategy = "classical";
            }

            switch (strategy.ToLowerInvariant())
            {
                case "classical":
                    return new ClassicalProcessorStrategy();

                case "functional":
                    return new FunctionalProcessorStrategy();

                default:
                    return null;
            }
        }
    }
}