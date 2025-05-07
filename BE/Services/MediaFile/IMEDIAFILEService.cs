using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;

namespace BE.Services.MediaFile
{
    public interface IMEDIAFILEService
    {
        Task<BaseResponse<FileStreamResult>> DownloadFileAsync(string fileName);
        Task<BaseResponse<List<MODELMediaFile>>> UploadFileAsync(List<IFormFile> file);
        Task<BaseResponse<MODELMediaFile>> UpdatePictureUser(IFormFile file, Guid OwnerId, int FileType);
        BaseResponse<MODELMediaFile> UpdatePictureUser(POSTMediaFileRequest request);
        BaseResponse<POSTMediaFileRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELMediaFile> Insert(POSTMediaFileRequest request);
        BaseResponse<MODELMediaFile> Update(POSTMediaFileRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);

    }
}
