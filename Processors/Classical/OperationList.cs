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
    public class OperationList
    {
        private readonly IList<Operation> operationsList = new List<Operation>();
        
        public IEnumerable<Operation> Operations { get => operationsList; }

        public int NumberOfOperations { get => operationsList.Count; }

        public void Add(Operation operation)
        {
            this.operationsList.Add(operation);
        }
    }
}