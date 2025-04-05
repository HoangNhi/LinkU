using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.MESSAGE.Dtos;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class MessageController : BaseController<MessageController>
    {

        public MessageController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TransToChatScreen(GetByIdRequest request)
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
        
        public ActionResult GetFriendRequestStatus(GetByIdRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_GETFRIENDREQUESTSTATUS, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELFriendStatus>(response.Data.ToString());
                        return PartialView("~/Views/Home/Message/_FriendRequestStatusPartial.cshtml", result);
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
            catch(Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
    }
}
