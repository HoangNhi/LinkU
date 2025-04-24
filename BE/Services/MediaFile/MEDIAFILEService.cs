using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BE.Services.MediaFile
{
    public class MEDIAFILEService : IMEDIAFILEService
    {
        private readonly BlobContainerClient _containerClient;

        public MEDIAFILEService(BlobServiceClient blobServiceClient, IConfiguration config)
        {
            var containerName = config["AzureBlobStorage:ContainerName"];

            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = _containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString(); // Trả về URL file
        }
    }
}
