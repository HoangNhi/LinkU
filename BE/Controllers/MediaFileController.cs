using BE.Services.MediaFile;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Requests;

namespace BE.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MediaFileController : BaseController<MediaFileController>
    {
        private readonly IMEDIAFILEService _service;

        public MediaFileController(IMEDIAFILEService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            try
            {
                // 1. Kiểm tra null
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("File không hợp lệ hoặc không có dữ liệu.");
                // 2. Tải file
                var response = await _service.DownloadFileAsync(fileName);
                if (response.Error)
                    throw new Exception(response.Message);
                return response.Data;
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> UploadFile(List<IFormFile> files)
        {
            try
            {
                foreach (var file in files)
                {
                    // 1. Kiểm tra null
                    if (file == null || file.Length == 0)
                        throw new Exception("File không hợp lệ hoặc không có dữ liệu.");

                    // 2. Giới hạn kích thước (ví dụ: 2MB)
                    const long maxFileSize = 2 * 1024 * 1024; // 2MB
                    if (file.Length > maxFileSize)
                        throw new Exception("File vượt quá dung lượng cho phép (2MB).");

                    //// 3. Kiểm tra định dạng file (chỉ cho phép ảnh và PDF)
                    //if (!CommonConst.AllowedContentTypes.Contains(file.ContentType.ToLower()))
                    //    throw new Exception("Định dạng file không được hỗ trợ. Chỉ cho phép JPEG, PNG, GIF, hoặc PDF.");
                }

                // 4. Upload nếu hợp lệ
                var response = await _service.UploadFileAsync(files);
                if (response.Error)
                {
                    throw new Exception(response.Message);
                }
                return Ok(new ApiResponse(response.Data));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));

            }
        }

        /// <summary>
        /// Dùng cho việc upload ảnh đại diện và ảnh bìa rồi tạo 1 dữ liệu mới trong table MediaFile, cập nhật lại Image đang sử dụng hiện tại
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> UpdatePictureUserWithUploadPicture(IFormFile file)
        {
            try
            {
                // 1. Kiểm tra null
                if (file == null || file.Length == 0)
                    throw new Exception("File không hợp lệ hoặc không có dữ liệu.");

                // 2. Giới hạn kích thước (ví dụ: 2MB)
                const long maxFileSize = 2 * 1024 * 1024; // 2MB
                if (file.Length > maxFileSize)
                    throw new Exception("File vượt quá dung lượng cho phép (2MB).");

                // 3. Kiểm tra định dạng file (chỉ cho phép ảnh và PDF)
                if (!CommonConst.AllowedPictureTypes.Contains(file.ContentType.ToLower()))
                    throw new Exception("Định dạng file không được hỗ trợ. Chỉ cho phép .jpg, .jpeg, .jpe, .jfif và .png");

                // 4. Kiểm tra OwnerId và FileType
                var ownerId = Request.Form["OwnerId"];
                if (string.IsNullOrEmpty(ownerId))
                    throw new Exception("OwnerId không được để trống.");
                var fileType = Request.Form["FileType"];
                if (string.IsNullOrEmpty(fileType))
                    throw new Exception("FileType không được để trống.");

                // 5. Upload nếu hợp lệ
                var response = await _service.CreatePicture(new POSTCreatePictureRequest
                {
                    File = file, 
                    OwnerId = Guid.Parse(ownerId), 
                    FileType = (MediaFileType)int.Parse(fileType)
                });

                if (response.Error)
                {
                    throw new Exception(response.Message);
                }
                return Ok(new ApiResponse(response.Data));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));

            }
        }

        /// <summary>
        /// Dùng cho việc cập nhật ảnh đại diện và ảnh bìa đã có trong table MediaFile (đã upload trước đó)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ApiResponse> UpdatePictureUserWithoutUploadPicture(POSTMediaFileRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.UpdatePictureUser(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetByPost(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetByPost(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> Insert(POSTMediaFileRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Insert(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> Update(POSTMediaFileRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Insert(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> DeleteList(DeleteListRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.DeleteList(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }
    }
}
