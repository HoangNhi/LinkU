using FE.Constant;
using FE.Services.ConsumeAPI;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.CONVERSATION.Dtos;
using MODELS.CONVERSATION.Requests;
using MODELS.MESSAGELIST.Dtos;
using MODELS.MESSAGELIST.Requests;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class ConversationController : BaseController<ConversationController>
    {

        public ConversationController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult Index()
        {
            return PartialView("~/Views/Home/Conversation/_ConversationPartial.cshtml");
        }

        // Tab: Search
        public IActionResult SearchUserByEmailOrPhone(POSTSearchInConversationRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.CONVERSATION_SEARCH_USER_BY_EMAIL_OR_PHONE, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        result.Data = JsonConvert.DeserializeObject<List<MODELUser>>(result.Data.ToString());
                        return PartialView("~/Views/Home/Conversation/_TabSearchResultPartial.cshtml", result);
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

        // Tab: GetListPaging
        public IActionResult GetListPaging(POSTConversationGetListPagingRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.CONVERSATION_GET_LIST_PAGING, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        result.Data = JsonConvert.DeserializeObject<List<MODELConversationGetListPaging>>(result.Data.ToString());
                        return PartialView("~/Views/Home/Conversation/_TabGetListPagingPartial.cshtml", result);
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
        
        public IActionResult NoMessage()
        {
            return PartialView("~/Views/Shared/Empty/_NoMessagePartial.cshtml");
        }
    }
}
