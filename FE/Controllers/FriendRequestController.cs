using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.MESSAGELIST.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class FriendRequestController : BaseController<FriendRequestController>
    {
        private readonly ICONSUMEAPIService _consumeAPI;

        public FriendRequestController(ICONSUMEAPIService consumeAPI)
        {
            _consumeAPI = consumeAPI;
        }

        public IActionResult Index()
        {
            return PartialView("~/Views/Home/Contact/FriendRequest/_TabFriendRequestPartial.cshtml");
        }

        public IActionResult GetReceiveListPaging(POSTFriendRequestGetListPagingRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_GET_LIST_PAGING, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        var resultData = JsonConvert.DeserializeObject<List<MODELFriendRequest>>(result.Data.ToString());
                        ViewBag.BeURL = _consumeAPI.GetBEUrl();
                        return PartialView("~/Views/Home/Contact/FriendRequest/_ReceiveRequestPartial.cshtml", resultData);
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


    }
}
