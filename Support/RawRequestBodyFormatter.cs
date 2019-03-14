using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ImageProcessor.Support
{
    /// <summary>
    /// Formatter that allows content of type text/plain and application/octet stream
    /// or no content type to be parsed to raw data. Allows for a single input parameter
    /// in the form of:
    /// 
    /// public string RawString([FromBody] string data)
    /// public byte[] RawData([FromBody] byte[] data)
    /// </summary>
    public class RawRequestBodyFormatter : InputFormatter
    {
        private static List<string> supportedMediaTypes = new List<string>()
        {
            // specific image types
            "image/bmp",
            "image/gif",
            "image/jpeg",
            "image/png",

            // a generic byte array
            "application/octet-stream"
        };
        
        private readonly ILogger logger;

        public RawRequestBodyFormatter(ILogger<RawRequestBodyFormatter> logger)
        {
            this.logger = logger;
            this.logger.LogInformation("in the constructor");

            foreach (var supportedMediaType in supportedMediaTypes)
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(supportedMediaType));
            }
        }


        /// <summary>
        /// Allow text/plain, application/octet-stream and no content type to
        /// be processed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null) 
            {
                throw new ArgumentNullException(nameof(context));
            }

            logger.LogInformation("contentType is {0}", context.HttpContext.Request.ContentType);

            return supportedMediaTypes.Contains(context.HttpContext.Request.ContentType);
        }

        /// <summary>
        /// Handle text/plain or no content type for string results
        /// Handle application/octet-stream for byte[] results
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            this.logger.LogInformation("attempting to read the request");

            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;
            var contentLength = request.ContentLength ?? 2048;

            if (supportedMediaTypes.Contains(contentType))
            {
                using (var ms = new MemoryStream((int)contentLength))
                {
                    await request.Body.CopyToAsync(ms);
                    var content = ms.ToArray();
                    
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }    
}