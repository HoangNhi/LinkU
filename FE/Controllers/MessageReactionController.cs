using FE.Constant;
using FE.Services.ConsumeAPI;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.MESSAGEREACTION.Dtos;
using MODELS.MESSAGEREACTION.Requests;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class MessageReactionController : BaseController<MessageReactionController>
    {
        public MessageReactionController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HandleRequest(POSTMessageReactionRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGEREACTION_HANDLE_REQUEST, request, HttpAction.Post);

                    // Check response
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELMessageReaction>(response.Data.ToString());
                        return Json(new { IsSuccess = true, Data = result });
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
                return Json(new { IsSuccess = false, Message = message });
            }
        }
        
        public IActionResult ShowPopupDetail(GETMessageReactionShowPopupRequest request)
        {
            try
            {
                if(request.MessageId == null || request.MessageId == Guid.Empty)
                {
                    throw new Exception("Tin nhắn không được để trống");
                }

                if (string.IsNullOrEmpty(request.ReactionTypesJSON))
                {
                    throw new Exception("Phản ứng không được để trống");
                }
                else
                {
                    request.ReactionTypes = JsonConvert.DeserializeObject<List<ReactionTypeShowPopup>>(request.ReactionTypesJSON);
                    if (request.ReactionTypes == null || request.ReactionTypes.Count == 0)
                    {
                        throw new Exception("Phản ứng không được để trống");
                    }
                }

                return PartialView("~/Views/Home/Message/MessageReaction/_PopupDetail.cshtml", request);
            }
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message });
            }
        }

        [HttpPost]
        public IActionResult GetListPaging(POSTMessageReactionGetListPagingRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGEREACTION_GET_LIST_PAGING, request, HttpAction.Post);
                    // Check response
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        result.Data = JsonConvert.DeserializeObject<List<MODELMessageReaction>>(result.Data.ToString());
                        return PartialView("~/Views/Home/Message/MessageReaction/_GetListPagingPartial.cshtml", result);
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
                return Json(new { IsSuccess = false, Message = message });
            }
        }
    }
}
