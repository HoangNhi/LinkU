using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
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
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [HttpPost]
        public ActionResult GetListPaging(PostMessageGetListPagingRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGE_GET_LIST_PAGING, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        var DataResult = JsonConvert.DeserializeObject<MODELMessageGetListPaging>(result.Data.ToString());

                        result.Data = DataResult;
                        DataResult.CurrentUser.ProfilePicture = GetProfilePicture(DataResult.CurrentUser.ProfilePicture);
                        DataResult.CurrentUser.CoverPicture = GetCoverPicture(DataResult.CurrentUser.CoverPicture);
                        DataResult.FriendUser.ProfilePicture = GetProfilePicture(DataResult.FriendUser.ProfilePicture);
                        DataResult.FriendUser.CoverPicture = GetCoverPicture(DataResult.FriendUser.CoverPicture);

                        return PartialView("~/Views/Home/Message/_MessageContainerPartial.cshtml", result);
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
        public ActionResult Insert(PostMessageRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGE_INSERT, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELMessage>(response.Data.ToString());
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
    }
}
