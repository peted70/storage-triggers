using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SendMessage.Data
{
    public class BlobStorageService
    {
        BlobContainerClient _bcc;
        const string CustomKey = "customstatus";
        const string ContainerName = "send-message";

        public BlobStorageService(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("Storage_Connection_String");
            _bcc = new BlobContainerClient(connectionString, ContainerName);
            _bcc.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }

        public async Task<IEnumerable<BlobFile>> GetFilesAsync()
        {
            List<BlobFile> blobs = new List<BlobFile>();
            try
            {
                await foreach (BlobItem blob in _bcc.GetBlobsAsync())
                {
                    var bc = _bcc.GetBlobClient(blob.Name);
                    var bp = await bc.GetPropertiesAsync();

                    var bf = new BlobFile();
                    if (bp.Value.Metadata.TryGetValue(CustomKey, out string value))
                    {
                        bf.Status = value;
                    }
                    bf.Name = blob.Name;
                    blobs.Add(bf);
                }
            }
            catch (RequestFailedException)
            {
                // Swallow this as maybe the container doesn't exist yet
            }
            return blobs;
        }

        public async Task SetProcessedAsync(BlobFile file)
        {
            var bc = _bcc.GetBlobClient(file.Name);
            await bc.SetMetadataAsync( new Dictionary<string, string>() { { CustomKey, "processed" } });
        }
    }
}
