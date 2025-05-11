using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MESSAGELIST.Dtos;
using MODELS.MESSAGELIST.Requests;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class MessageListController : BaseController<MessageListController>
    {

        public MessageListController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult Index()
        {
            return PartialView("~/Views/Home/MessageList/_TabMessageListPartial.cshtml");
        }

        [HttpPost]
        public IActionResult Search(MessageList_SearchRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGELIST_SEARCH, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELMessageList_Search>(response.Data.ToString());
                        foreach (var item in result.Users)
                        {
                            item.ProfilePicture = GetProfilePicture(item.ProfilePicture);
                        }
                        return PartialView("~/Views/Home/MessageList/_SearchResultPartial.cshtml", result);
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

        public IActionResult GetListMessageLatest(POSTGetListMessageLatestRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGELIST_GETLISTMESSAGELATEST, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        result.Data = JsonConvert.DeserializeObject<List<MODELMessageList_GetListMessageLatest>>(result.Data.ToString());
                        return PartialView("~/Views/Home/MessageList/_TabListMessageLatestPartial.cshtml", result);
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
