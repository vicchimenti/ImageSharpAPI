using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ImageProcessor.Operations;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Processors
{
    public class FunctionParser
    {
        private readonly IDictionary<String, Func<Match, Action<Image<Rgba32>, ILogger<Operation>>>> builders = 
            new Dictionary<String, Func<Match, Action<Image<Rgba32>, ILogger<Operation>>>>()
            {
                { @"hflip", (match) => (image, logger) => 
                    { 
                        logger.LogInformation("executing hflip operation");
                        image.Mutate(i => i.Flip(FlipMode.Horizontal)); } 
                    },

                { @"vflip", (match) => (image, logger) => 
                    { 
                        logger.LogInformation("executing vflip operation");
                        image.Mutate(i => i.Flip(FlipMode.Vertical)); } 
                    },

                { @"gray", (match) => (image, logger) => 
                    { 
                        logger.LogInformation("executing gray operation");
                        image.Mutate(i => i.Grayscale()); } 
                    },

                { @"gray\((?<percent>[0-9]+)\)", (match) => (image, logger) => 
                    {
                        logger.LogInformation("executing gray operation");
                        var percent = Int32.Parse(match.Groups["percent"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        if (percent > 0 && percent < 100)
                        {
                            logger.LogInformation("grayscale image {0}°", percent);
                            image.Mutate(i => i.Grayscale(percent / 100.0F));
                        }
                    }
                },

                { @"resize\((?<ratio>[0-9]+)\)", (match) => (image, logger) => 
                    {
                        logger.LogInformation("executing resize operation");
                        var ratio = Int32.Parse(match.Groups["ratio"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
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
                },

                { @"rotate\((?<degrees>[-]?[0-9]+)\)", (match) => (image, logger) => 
                    {
                        logger.LogInformation("executing rotate operation");
                        var degrees = Int32.Parse(match.Groups["degrees"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        if (degrees > -360 && degrees < 360) 
                        {
                            logger.LogInformation("rotate image {0}°", degrees);
                            image.Mutate(i => i.Rotate(degrees));
                        }
                    }
                },

                { @"left", (match) => (image, logger) => 
                    { 
                        logger.LogInformation("executing left operation");
                        image.Mutate(i => i.Rotate(RotateMode.Rotate270)); } 
                    },
                
                { @"right", (match) => (image, logger) => 
                    { 
                        logger.LogInformation("executing right operation");
                        image.Mutate(i => i.Rotate(RotateMode.Rotate90)); } 
                    },

                { @"thumb", (match) => (image, logger) => 
                    { 
                        var HEIGHT = 100F;
                        var WIDTH = 75F;
                        
                        var size = image.Size();

                        var newWidth = size.Width;
                        var newHeight = size.Height;

                        float widthRatio = size.Width / WIDTH;
                        float heightRatio = size.Height / HEIGHT;

                        if (widthRatio > heightRatio)
                        {
                            newWidth = (int)WIDTH;
                            newHeight = (int)(size.Height / widthRatio);
                        }
                        else
                        {
                            newWidth = (int)(size.Width / heightRatio);
                            newHeight = (int)HEIGHT;
                        }

                        logger.LogInformation("old height={0}, width={1}", size.Height, size.Width);
                        logger.LogInformation("new height={0}, width={1}", newHeight, newWidth);

                        image.Mutate(i => i.Resize(newWidth, newHeight));
                    }
                }
            };
        
        private readonly string operations;

        private readonly ILoggerFactory loggerFactory;

        public FunctionParser(string operations, ILoggerFactory loggerFactory)
        {
            this.operations = operations;
            this.loggerFactory = loggerFactory;
        }

        public FunctionList Parse()
        {
            var logger = loggerFactory.CreateLogger<FunctionList>();
            var functionsList = new FunctionList();

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
                            var func = builder.Value(match);
                            logger.LogInformation("adding operation '{0}' to list", op);
                            functionsList.Add(func);

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
                    var func = builders["thumb"](null);

                    logger.LogInformation("adding final thumbnail operation to list");
                    functionsList.Add(func);
                }
            }

            logger.LogInformation("all transforms parsed, there are {0}", functionsList.NumberOfOperations);
            return functionsList;
        }
    }
}