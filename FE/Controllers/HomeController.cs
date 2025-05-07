using FE.Constant;
using FE.Models;
using FE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace FE.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger, ICONSUMEAPIService consumeAPI, IHttpContextAccessor contextAccessor) : base(consumeAPI)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            // Lấy thông tin user
            ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, new GetByIdRequest { Id = Guid.Parse(_consumeAPI.GetUserId()) }, HttpAction.Post);
            if (response.Success)
            {
                var user = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                user.ProfilePicture = GetProfilePicture(user.ProfilePicture);
                ViewBag.UserInfo = user;
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { Message = response.Message });
            }
            return View();
        }

        public IActionResult GetConfigProfile()
        {
            // Lấy thông tin user
            ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, new GetByIdRequest { Id = Guid.Parse(_consumeAPI.GetUserId()) }, HttpAction.Post);
            if (response.Success)
            {
                var user = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                user.ProfilePicture = GetProfilePicture(user.ProfilePicture);

                return PartialView("~/Views/Shared/ConfigProfile/_ProfilePicture.cshtml", user);
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { Message = response.Message });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Tên file không hợp lệ.");
            }

            return await _consumeAPI.ExecuteFileDownloadAPI(URL_API.MEDIAFILE_DOWNLOAD, fileName);
        }

        #region ConfigProfile Popover
        public IActionResult ConfigProfile()
        {
            return PartialView("~/Views/Shared/ConfigProfile/_ConfigProfilePopoverPartial.cshtml");
        }

        public IActionResult ShowPersonalInfoModal()
        {
            // Lấy thông tin user
            ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_POST, new GetByIdRequest { Id = Guid.Parse(_consumeAPI.GetUserId()) }, HttpAction.Post);
            if (response.Success)
            {
                var user = JsonConvert.DeserializeObject<PostUpdateUserInforRequest>(response.Data.ToString());
                user.ProfilePicture = GetProfilePicture(user.ProfilePicture);
                user.CoverPicture = GetCoverPicture(user.CoverPicture);
                return PartialView("~/Views/Shared/ConfigProfile/_PeronalInforPartial.cshtml", user);
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { Message = response.Message });
            }
        }

        // Update Profile Tab - Start
        public IActionResult GetUpdateProfile(string request)
        {
            string decodedJson = WebUtility.HtmlDecode(request);
            var user = JsonConvert.DeserializeObject<PostUpdateUserInforRequest>(decodedJson);
            return PartialView("~/Views/Shared/ConfigProfile/_UpdateProfilePartial.cshtml", user);
        }

        public IActionResult UpdatePersonalInfo(PostUpdateUserInforRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_UPDATE_INFOR, request, HttpAction.Post);
                    if (response.Success)
                    {
                        return Json(new { IsSuccess = true, Message = "Cập nhật thành công", Data = "" });
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelState(this.ModelState));
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
        // Update Profile Tab - End

        // Update Picture - Start
        public IActionResult GetUpdatePicture(POSTGetListMediaFilesRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_LIST_MEDIA_FILES, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<List<MODELMediaFile>>(response.Data.ToString());
                        return PartialView("~/Views/Shared/ConfigProfile/_UpdatePicturePartial.cshtml", result);
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelState(this.ModelState));
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        public IActionResult GetConfirmUpdatePicture(Guid fileid, int filetype)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MEDIAFILE_GET_BY_POST, new GetByIdRequest { Id = fileid }, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<POSTMediaFileRequest>(response.Data.ToString());
                        if (!result.IsEdit)
                        {
                            result.FileType = filetype;
                        }
                        return PartialView("~/Views/Shared/ConfigProfile/_ConfirmUpdatePicturePartial.cshtml", result);
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelState(this.ModelState));
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        /// <summary>
        /// Dùng cho việc upload ảnh đại diện và ảnh bìa rồi tạo 1 dữ liệu mới trong table MediaFile, cập nhật lại Image đang sử dụng hiện tại
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePictureUserWithUploadFile(IFormCollection data)
        {
            try
            {
                // Kiểm tra xem có file không
                if (data.Files == null || data.Files.Count == 0)
                    return Json(new { IsSuccess = false, Message = "Không có file được gửi lên", Data = "" });

                // Kiểm tra từng file
                long maxFileSize = 2 * 1024 * 1024; // 2MB

                foreach (var file in data.Files)
                {
                    if (!MODELS.COMMON.CommonConst.AllowedPictureTypes.Contains(file.ContentType))
                        throw new Exception($"File '{file.FileName}' không hợp lệ. Chỉ cho phép .jpg, .jpeg, .jpe, .jfif và .png");

                    if (file.Length > maxFileSize)
                        throw new Exception($"File '{file.FileName}' vượt quá kích thước cho phép (2MB)");
                }

                // Xử lý upload
                var multiForm = new System.Net.Http.MultipartFormDataContent();

                // add API method parameters
                foreach (var file in data.Files)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());

                    // Gán Content-Type cho từng file
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    multiForm.Add(streamContent, "file", file.FileName);
                }

                if (string.IsNullOrEmpty(data["FileType"]))
                {
                    throw new Exception("FileType không được để trống");
                }
                else
                {
                    multiForm.Add(new StringContent(data["FileType"]), "FileType");
                }

                multiForm.Add(new StringContent(_consumeAPI.GetUserId()), "OwnerId");

                ApiResponse response = await _consumeAPI.PostFormDataAPI(URL_API.MEDIAFILE_UPDATE_PICTURE_USER_WITH_UPLOAD_PICTURE, multiForm);

                if (response.Success)
                {
                    var result = JsonConvert.DeserializeObject<MODELMediaFile>(response.Data.ToString());
                    return Json(new { IsSuccess = true, Message = "", Data = "" });
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi upload file: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }


        /// <summary>
        /// Dùng cho việc cập nhật ảnh đại diện và ảnh bìa đã có trong table MediaFile (đã upload trước đó)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdatePictureUserWithoutUploadFile(POSTMediaFileRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MEDIAFILE_UPDATE_PICTURE_USER_WITHOUT_UPLOAD_PICTURE, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELMediaFile>(response.Data.ToString());
                        return Json(new { IsSuccess = true, Message = "", Data = "" });
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelState(this.ModelState));
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [HttpPost]
        public IActionResult DeleteListPicture(DeleteListRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.MEDIAFILE_DELETE_LIST, request, HttpAction.Post);
                    if (response.Success)
                    {
                        return Json(new { IsSuccess = true, Message = "Xóa ảnh thành công", Data = "" });
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelState(this.ModelState));
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
        // Update Picture - End
        #endregion
    }
}
