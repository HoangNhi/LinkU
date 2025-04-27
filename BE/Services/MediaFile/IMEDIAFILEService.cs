using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;

namespace BE.Services.MediaFile
{
    public interface IMEDIAFILEService
    {
        Task<BaseResponse<FileStreamResult>> DownloadFileAsync(string fileName);
        Task<BaseResponse<string>> UploadFileAsync(IFormFile file);
    }
}
