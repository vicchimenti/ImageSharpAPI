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
    public class FunctionList
    {
        private readonly IList<Action<Image<Rgba32>, ILogger<Operation>>> functionsList = 
            new List<Action<Image<Rgba32>, ILogger<Operation>>>();
        
        public IEnumerable<Action<Image<Rgba32>, ILogger<Operation>>> Functions { get => functionsList; }

        public int NumberOfOperations { get => functionsList.Count; }
        
        public void Add(Action<Image<Rgba32>, ILogger<Operation>> function)
        {
            this.functionsList.Add(function);
        }

    }
}