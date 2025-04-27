using FE.Constant;
using FE.Models;
using FE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MESSAGE.Dtos;
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
            ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, new GetByIdRequest { Id = Guid.Parse(_consumeAPI.GetUserId())}, HttpAction.Post);
            if(response.Success)
            {
                var user = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                user.ProfilePicture = GetProfilePicture(user.ProfilePicture);
                ViewBag.UserInfo = user;
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { Message = response.Message});
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

            return await _consumeAPI.ExecuteFileDownloadAPI(URL_API.HOME_DOWNLOAD, fileName);
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

        // Update Profile Picture - Start
        public IActionResult GetUpdateProfilePicture(GetByIdRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                        result.ProfilePicture = GetProfilePicture(result.ProfilePicture);
                        return PartialView("~/Views/Home/Message/_MessagePartial.cshtml", result);
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

        // Update Profile Picture - End
        #endregion
    }
}
