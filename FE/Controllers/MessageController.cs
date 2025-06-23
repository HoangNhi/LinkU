using FE.Constant;
using FE.Services.ConsumeAPI;
using FE.Services.ViewRender;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.GROUP.Dtos;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class MessageController : BaseController<MessageController>
    {
        private readonly IVIEWRENDERService _viewRenderService;
        public MessageController(ICONSUMEAPIService consumeAPI, IVIEWRENDERService viewRenderService) : base(consumeAPI)
        {
            _viewRenderService = viewRenderService;
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
                    if (request.MessageType == 0)
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
                    else if (request.MessageType == 1)
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
                        ViewBag.ConversationType = request.ConversationType;
                        ViewBag.CurrentUserId = Guid.Parse(_consumeAPI.GetUserId());

                        // Nếu chưa có tin nhắn thì lấy thông tin của người nhắn tin để hiển thị
                        if (DataResult.Count == 0 && request.ConversationType == 0 && result.PageIndex == 1)
                        {
                            ApiResponse targetUser = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, new GetByIdRequest { Id = request.TargetId }, HttpAction.Post);
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

        [HttpPost]
        public async Task<ActionResult> SendMessageWithFile(IFormCollection data)
        {
            try
            {
                // Kiểm tra xem có file không
                if (data.Files == null || data.Files.Count == 0)
                    return Json(new { IsSuccess = false, Message = "Không có file được gửi lên", Data = "" });

                // Kiểm tra từng file
                long maxFileSize = 20 * 1024 * 1024; // 20MB

                foreach (var file in data.Files)
                {
                    if (file.Length > maxFileSize)
                        throw new Exception($"File '{file.FileName}' vượt quá kích thước cho phép (20MB)");
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

                // add other parameters
                multiForm.Add(new StringContent(_consumeAPI.GetUserId()), "SenderId");
                multiForm.Add(new StringContent(data["Content"]), "Content");
                multiForm.Add(new StringContent(data["RefId"]), "RefId");

                if (string.IsNullOrEmpty(data["TargetId"]))
                {
                    throw new Exception("Người nhận không được để trống");
                }
                else
                {
                    multiForm.Add(new StringContent(data["TargetId"]), "TargetId");
                }

                if (string.IsNullOrEmpty(data["ConversationType"]))
                {
                    throw new Exception("Kiểu hội thoại không được để trống");
                }
                else
                {
                    multiForm.Add(new StringContent(data["ConversationType"]), "ConversationType");
                }

                ApiResponse response = await _consumeAPI.PostFormDataAPI(URL_API.MESSAGE_SEND_MESSAGE_WITH_FILE, multiForm);

                if (response.Success)
                {
                    var result = JsonConvert.DeserializeObject<List<MODELSendMessageWithFileResponse>>(response.Data.ToString());
                    List<object> Htmls = new List<object>();
                    foreach (var item in result)
                    {
                        // Deserialize the message data
                        var DataResult = JsonConvert.DeserializeObject<List<MODELMessage>>(item.Messages.Data.ToString());
                        item.Messages.Data = DataResult;

                        var viewBag = new Dictionary<string, object>
                        {
                            { "RowPerPage", DataResult.Count },
                            { "ConversationType", int.Parse(data["ConversationType"]) },
                            { "CurrentUserId", item.IsMyResponse ? Guid.Parse(_consumeAPI.GetUserId()) : Guid.Empty },
                        };

                        // Render HTML for the message container
                        var html = await _viewRenderService.RenderToStringAsync("/Views/Home/Message/_MessageContainerPartial.cshtml", item.Messages, viewBag);

                        // Add the rendered HTML to the list
                        Htmls.Add(new { Html = html, IsMyResponse = item.IsMyResponse });
                    }

                    return Json(new { IsSuccess = true, Message = "", Data = Htmls });
                }
                else
                {
                    throw new Exception(response.Message);
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
