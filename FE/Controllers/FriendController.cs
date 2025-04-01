using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class FriendController : BaseController<FriendController>
    {
        public FriendController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowAddFriendPopup(GetByIdRequest request)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_GET_BY_ID, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                        result.ProfilePicture = GetProfilePicture(result.ProfilePicture);
                        result.CoverPicture = GetCoverPicture(result.CoverPicture);
                        return PartialView("~/Views/Home/Friend/PopupAddFriend.cshtml", result);
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
