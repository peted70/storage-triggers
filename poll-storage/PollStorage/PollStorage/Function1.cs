using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PollStorage
{
    public static class Function1
    {
        private const string Key = "CustomStatus";

        //        {
        //              "Id": "JTJmcG9sbC1zdG9yYWdlJTJmYWRtaW4tZGFzaGJvYXJkMS5wbmc=",
        //              "Name": "admin-dashboard1.png",
        //              "DisplayName": "admin-dashboard1.png",
        //              "Path": "/poll-storage/admin-dashboard1.png",
        //              "LastModified": "2021-03-22T18:23:44Z",
        //              "Size": 67070,
        //              "MediaType": "image/png",
        //              "IsFolder": false,
        //              "ETag": "\"0x8D8ED5FA0888553\"",
        //              "FileLocator": "JTJmcG9sbC1zdG9yYWdlJTJmYWRtaW4tZGFzaGJvYXJkMS5wbmc=",
        //              "LastModifiedBy": null
        //        }
        [FunctionName("CheckStorageMetadata")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string path = data.Path;
            var sections = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            string connectionString = Environment.GetEnvironmentVariable("STORAGE_CONN_STRING");
            string containerName = sections[0];
            string blobName = data.Name;

            BlobServiceClient client = new BlobServiceClient(connectionString);
            var bcb = client.GetBlobContainerClient(containerName);
            var bc = bcb.GetBlobClient(blobName);
            var props = await bc.GetPropertiesAsync();
            var hasCustomValue = props.Value.Metadata.TryGetValue(Key, out string status);

            if (!hasCustomValue)
            {
                await bc.SetMetadataAsync(new Dictionary<string, string> { { Key, "processing" } });
            }

            return new OkObjectResult(hasCustomValue ? status : "none");
        }
    }
}

