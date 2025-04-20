using FE.Constant;
using FE.Models;
using FE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MESSAGE.Dtos;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using System.Diagnostics;

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

        #region ConfigProfile Popover
        public IActionResult ConfigProfile()
        {
            return PartialView("~/Views/Shared/ConfigProfile/_ConfigProfilePopoverPartial.cshtml");
        }

        public IActionResult ShowPersonalInfoModal()
        {
            // Lấy thông tin user
            ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, new GetByIdRequest { Id = Guid.Parse(_consumeAPI.GetUserId()) }, HttpAction.Post);
            if (response.Success)
            {
                var user = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                user.ProfilePicture = GetProfilePicture(user.ProfilePicture);
                user.CoverPicture = GetCoverPicture(user.CoverPicture);
                return PartialView("~/Views/Shared/ConfigProfile/_PeronalInforPartial.cshtml", user);
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { Message = response.Message });
            }
        }
        #endregion
    }
}
