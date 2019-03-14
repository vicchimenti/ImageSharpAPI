using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ImageProcessor.Operations;
using Microsoft.Extensions.Logging;

namespace ImageProcessor.Processors
{
    public class OperationParser
    {
        private static readonly IDictionary<String, Func<Match, Operation>> builders = 
            new Dictionary<String, Func<Match, Operation>>()
            {
                { "hflip", m => new FlipHorizontal(m) },
                { "vflip", m => new FlipVertical(m) },
                { "right", m => new RotateRight(m) },
                { "left", m => new RotateLeft(m) },
                { @"rotate\((?<degrees>[-]?[0-9]+)\)", m => new Rotate(m) },
                { @"gray\((?<percent>[0-9]+)\)", m => new Grayscale(m) },
                { @"gray", m => new Grayscale(m) },
                { @"resize\((?<ratio>[0-9]+)\)", m => new Resize(m) },
                { "thumb", m => new Thumbnail(m) },
            };

        private readonly string operations;

        private readonly ILoggerFactory loggerFactory;

        public OperationParser(string operations, ILoggerFactory loggerFactory)
        {
            this.operations = operations;
            this.loggerFactory = loggerFactory;
        }

        public OperationList Parse()
        {
            var logger = loggerFactory.CreateLogger<OperationList>();
            var operationsList = new OperationList();

            if (string.IsNullOrEmpty(operations) == false)
            {
                logger.LogInformation("raw operations presented {operations}", operations);

                var ops = operations
                    .ToLowerInvariant()
                    .Split(';', StringSplitOptions.RemoveEmptyEntries);

                var produceThumbnail = false;

                foreach (var op in ops)
                {
                    logger.LogInformation("trying op '{op}'", op);

                    var matched = false;

                    foreach (var builder in builders)
                    {
                        logger.LogInformation("  matching against {0}", builder.Key);

                        var regex = new Regex(builder.Key);
                        var match = regex.Match(op);

                        if (match.Success && "thumb".Equals(op))
                        {
                            produceThumbnail = true;
                            matched = true;
                            break;
                        }
                        else if (match.Success)
                        {
                            var operation = builder.Value(match);
                            logger.LogInformation("adding operation '{0}' to list", op);
                            operationsList.Add(operation);

                            matched = true;
                            break;
                        }
                    }

                    if (matched == false)
                    {
                        logger.LogInformation("invalid operation '{op}'", op);
                        throw new InvalidOperationException(string.Format("Operation '{0}' is not a valid transform", op));
                    }
                }

                if (produceThumbnail == true)
                {
                    var operation = builders["thumb"](null);
                    
                    logger.LogInformation("adding final thumbnail operation to list");
                    operationsList.Add(operation);
                }
            }

            logger.LogInformation("all transforms parsed, there are {0}", operationsList.NumberOfOperations);
            return operationsList;
        }
    }
}