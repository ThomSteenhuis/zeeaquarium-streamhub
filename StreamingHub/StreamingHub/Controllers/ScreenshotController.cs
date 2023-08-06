using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamingHub.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StreamingHub.Controllers
{
    [Route("api/screenshot")]
    [ApiController]
    public class ScreenshotController : Controller
    {
        private readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=zeeaquariumopslag;AccountKey=iWCPvu0JKtSncCJKeugIS02UdfETHFTmM+zqpO+1w7myUfM3kbM97/QX4RiDfE6IqVCW+7RK84w9TU3BSlH33A==;EndpointSuffix=core.windows.net";

        private readonly string containerReference = "zeeaquarium-blob";

        private readonly string fileName = "zeeaquarium-screenshot.jpeg";

        [HttpPost]
        [Authorize(Roles = Role.Streamer)]
        public async Task<IActionResult> SetScreenshot([FromBody] ScreenshotModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerReference);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            var screenshotBytes = model.Screenshot
                .Select(s => Convert.ToByte(s))
                .ToArray();

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(screenshotBytes, 0, screenshotBytes.Length);

                memoryStream.Position = 0;

                var blobResult = await blobClient.UploadAsync(memoryStream, true);

                return Ok(blobResult.ToString());
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ScreenshotModel> GetScreenshot()
        {

            var blobServiceClient = new BlobServiceClient(connectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerReference);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    var download = await blobClient.DownloadToAsync(memoryStream);

                    return new ScreenshotModel()
                    {
                        Screenshot = memoryStream
                            .ToArray()
                            .Select(b => (int)b)
                            .ToArray()
                    };
                }
                catch (RequestFailedException e)
                {
                    return null;
                }
            }
        }
    }
}
