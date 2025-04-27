using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using HeyRed.Mime;

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

        public async Task<BaseResponse<FileStreamResult>> DownloadFileAsync(string fileName)
        {
            var response = new BaseResponse<FileStreamResult>();
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                if (await blobClient.ExistsAsync())
                {
                    var downloadInfo = await blobClient.DownloadAsync();
                    var contentType = downloadInfo.Value.ContentType;
                    var fileStream = downloadInfo.Value.Content;

                    response.Data = new FileStreamResult(fileStream, contentType)
                    {
                        FileDownloadName = fileName + "." + MimeTypesMap.GetExtension(contentType)
                    };

                }
                else
                {
                    throw new Exception("File không tồn tại");
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<string>> UploadFileAsync(IFormFile file)
        {
            var response = new BaseResponse<string>();
            try
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var blobClient = _containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                response.Data = blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
