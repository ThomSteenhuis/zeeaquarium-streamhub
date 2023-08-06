namespace StreamingHub.Controllers
{
    using Azure;
    using Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using StreamingHub.Models;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    [Route("api/setting")]
    [ApiController]
    public class SettingController : Controller
    {
        private readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=zeeaquariumopslag;AccountKey=iWCPvu0JKtSncCJKeugIS02UdfETHFTmM+zqpO+1w7myUfM3kbM97/QX4RiDfE6IqVCW+7RK84w9TU3BSlH33A==;EndpointSuffix=core.windows.net";

        private readonly string containerReference = "zeeaquarium-blob";

        [HttpPost]
        [Authorize(Roles = Role.User + "," + Role.Streamer)]
        public async Task<IActionResult> SetSetting([FromBody] SettingModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerReference);

            var fileName = $"zeeaquarium-setting-{model.Setting}.txt";

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            var modelObj = new JObject();
            modelObj.Add(nameof(model.Setting), model.Setting);
            modelObj.Add(nameof(model.Value), model.Value);

            var content = Encoding.UTF8.GetBytes(modelObj.ToString(Formatting.None));

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(content, 0, content.Length);

                memoryStream.Position = 0;

                var blobResult = await blobClient.UploadAsync(memoryStream, true);

                return Ok(blobResult.ToString());
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSetting(string setting)
        {
            if (setting == null)
            {
                return BadRequest("Invalid client request");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerReference);

            var fileName = $"zeeaquarium-setting-{setting}.txt";

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    var download = await blobClient.DownloadToAsync(memoryStream);

                    return Ok(JObject.Parse(Encoding.UTF8.GetString(memoryStream.ToArray())).ToObject<SettingModel>());
                }
                catch (RequestFailedException e)
                {
                    return NotFound($"Setting {setting} was not found");
                }
            }
        }
    }
}
