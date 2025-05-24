using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.GROUP.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class GroupController : BaseController<GroupController>
    {
        public GroupController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult ShowModalCreateGroup()
        {
            return PartialView("~/Views/Home/Contact/Group/_PopupCreateGroupPartial.cshtml");
        }

        public IActionResult GetListMemberCreateGroup()
        {
            try
            {
                ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.GROUP_GET_LIST_MEMBER_CREATE_GROUP, null, HttpAction.Get);
                if (response.Success)
                {
                    var result = JsonConvert.DeserializeObject<List<MODELMemberCreateGroup>>(response.Data.ToString());
                    return Json(new { IsSuccess = true, Message = "", Data = result });
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
