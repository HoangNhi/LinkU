using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.GROUP.Dtos;
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
        public ActionResult TransToChatScreen(GetMessageByIdAndTypeRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tin nhắn thông thường - User to User
                    if(request.MessageType == 0)
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
                    // Tin nhắn nhóm - User to Group
                    else if(request.MessageType == 1)
                    {
                        ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.GROUP_GET_BY_ID, request, HttpAction.Post);
                        if (response.Success)
                        {
                            var result = JsonConvert.DeserializeObject<MODELGroup>(response.Data.ToString());
                            return PartialView("~/Views/Home/Message/Group/_MessageUserToGroupPartial.cshtml", result);
                        }
                        else
                        {
                            throw new Exception(response.Message);
                        }
                    }
                    else
                    {
                        throw new Exception("Loại tin nhắn không hợp lệ.");
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
                        var DataResult = JsonConvert.DeserializeObject<List<MODELMessage>>(result.Data.ToString());

                        result.Data = DataResult;
                        ViewBag.RowPerPage = request.RowPerPage;

                        // Nếu chưa có tin nhắn thì lấy thông tin của người nhắn tin để hiển thị
                        if (DataResult.Count == 0 && request.ConversationType == 0 && result.PageIndex == 1)
                        {
                            ApiResponse targetUser = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, new GetByIdRequest { Id = request.TargetId}, HttpAction.Post);
                            if (targetUser.Success)
                            {
                                var targetUserData = JsonConvert.DeserializeObject<MODELUser>(targetUser.Data.ToString());
                                ViewBag.TargetUser = targetUserData;
                            }
                            else
                            {
                                throw new Exception(targetUser.Message);
                            }
                        }

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
