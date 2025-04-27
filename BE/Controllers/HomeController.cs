using BE.Services.MediaFile;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;

namespace BE.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : BaseController<HomeController>
    {
        private readonly IMEDIAFILEService _mediaFileService;

        public HomeController(IMEDIAFILEService mediaFileService)
        {
            _mediaFileService = mediaFileService;
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
                var response = await _mediaFileService.DownloadFileAsync(fileName);
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
        public async Task<ActionResult<ApiResponse>> Upload(IFormFile file)
        {
            try
            {
                // 1. Kiểm tra null
                if (file == null || file.Length == 0)
                    throw new Exception("File không hợp lệ hoặc không có dữ liệu.");

                // 2. Giới hạn kích thước (ví dụ: 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxFileSize)
                    throw new Exception("File vượt quá dung lượng cho phép (5MB).");

                // 3. Kiểm tra định dạng file (chỉ cho phép ảnh và PDF)
                if (!CommonConst.AllowedContentTypes.Contains(file.ContentType.ToLower()))
                    throw new Exception("Định dạng file không được hỗ trợ. Chỉ cho phép JPEG, PNG, GIF, hoặc PDF.");

                // 4. Upload nếu hợp lệ
                var url = await _mediaFileService.UploadFileAsync(file);
                return Ok(new ApiResponse(url));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));

            }
        }
    }
}
