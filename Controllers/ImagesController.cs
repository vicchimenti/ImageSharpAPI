using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageProcessor.Support;
using Microsoft.Extensions.Logging;
using ImageProcessor.Operations;
using ImageProcessor.Processors;

namespace ImageProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger logger;

        private readonly ILoggerFactory loggerFactory;

        public ImagesController(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<ImagesController>();
        }

        [HttpPost]
        public void ProcessImage([FromBody] byte[] imageData, [FromQuery(Name="ops")] string operations, [FromQuery] string strategy)
        {
            if (imageData == null || imageData.Length == 0)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                return;
            }

            if (string.IsNullOrEmpty(strategy))
            {
                strategy = "functional";
            }

            try
            {
                logger.LogInformation("executing {0} strategy", strategy);

                var processor = ProcessorFactory.GetProcessor(strategy);
                if (processor == null)
                {
                    logger.LogInformation("unknown strategy {0}", strategy);
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return;
                }

                byte[] resultingImage = processor.ProcessImage(operations, imageData, loggerFactory);
                if (resultingImage == null || resultingImage.Count() == 0)
                {
                    logger.LogInformation("failed to process image");
                    Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    return;
                }

                logger.LogInformation("image processed, new image size is {0} bytes", resultingImage.Count());
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                // the underlying library is leving the image type intact, so we're just going to return it
                Response.ContentType = Request.ContentType;

                // write the image data back on the stream, we don't deal with Content-Length because
                // we're relying on aspnet core to use Transfer-Encoding: Chunked here
                Response.Body.Write(resultingImage);
            } 
            catch (InvalidOperationException ioe)
            {
                logger.LogInformation("{0}", ioe);
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }
            catch (Exception e)
            {
                logger.LogInformation("{0}", e);
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
