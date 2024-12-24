using FE.Constant;
using FE.Models;
using FE.Services;
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
        private readonly ICONSUMEAPIService _consumeAPI;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger, ICONSUMEAPIService consumeAPI, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _consumeAPI = consumeAPI;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            MODELUser model = new MODELUser();

            foreach (var claim in _contextAccessor.HttpContext.User.Claims)
            {
                switch (claim.Type)
                {
                    case "HoLot": { model.HoLot = claim.Value; }; break;
                    case "Ten": { model.Ten = claim.Value; }; break;
                    case "ProfilePicture": { model.ProfilePicture = claim.Value; }; break;
                }
            }

            ViewBag.UserInfo = model;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetListMessage(GetListPagingRequest request)
        {
            try
            {
                var result = new GetListPagingResponse();

                GetListPagingRequest param = new GetListPagingRequest();
                param.PageIndex = request.PageIndex - 1;
                param.RowPerPage = request.RowPerPage;
                param.TextSearch = request.TextSearch == null ? string.Empty : request.TextSearch.Trim();

                ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGE_GET_LIST_PAGING, request, HttpAction.Post);
                if (response.Success)
                {
                    result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                    result.Data = JsonConvert.DeserializeObject<List<MODELMessage>>(result.Data.ToString());
                    return PartialView("~/Views/Home/_MessageListPartial.cshtml", result);
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi lấy danh sách: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
    }
}
