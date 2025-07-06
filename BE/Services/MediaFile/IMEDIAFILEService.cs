using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;

namespace BE.Services.MediaFile
{
    public interface IMEDIAFILEService
    {
        Task<BaseResponse<FileStreamResult>> DownloadFileAsync(string fileName);
        Task<BaseResponse<List<MODELMediaFile>>> UploadFileAsync(List<IFormFile> file);
        /// <summary>
        /// Tạo mới hình ảnh cho người dùng hoặc nhóm
        /// Sử dụng cho Profile, Cover Picture và Avartar Group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BaseResponse<MODELMediaFile>> CreatePicture(POSTCreatePictureRequest request);
        Task<BaseResponse<MODELMediaFile>> UpdatePictureUser(POSTMediaFileRequest request);
        BaseResponse<MODELMediaFile> GetById(GetByIdRequest request);
        BaseResponse<POSTMediaFileRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELMediaFile> Insert(POSTMediaFileRequest request);
        BaseResponse<MODELMediaFile> Update(POSTMediaFileRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);
        Task<List<ENTITIES.DbContent.MediaFile>> GetEntitesByMessageIdAsync(List<Guid> messageIds, List<Guid> refIds);
    }
}
