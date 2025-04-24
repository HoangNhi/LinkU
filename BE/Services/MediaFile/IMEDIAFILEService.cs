namespace BE.Services.MediaFile
{
    public interface IMEDIAFILEService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
