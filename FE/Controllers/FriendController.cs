using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class FriendController : BaseController<FriendController>
    {
        public FriendController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        #region Tap Friend Request
        [Route("friendrequest")]
        public IActionResult GetTapFriendRequest()
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
                        foreach (var item in resultData)
                        {
                            item.User.ProfilePicture = GetProfilePicture(item.User.ProfilePicture);
                        }
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

        public IActionResult UpdateFriendRequest(POSTFriendRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_UPDATE, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELFriendRequest>(response.Data.ToString());
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
        #endregion

        /// <summary>
        /// Hàm hiển thị popup thêm bạn bè
        /// </summary>
        /// <param name="Case">
        /// 1. Gửi lời mời
        /// 2. Cập nhật lời mời được nhận
        /// 3. Cập nhật lời mời đã gửi
        /// </param>
        /// <returns></returns>
        public IActionResult ShowAddFriendPopup(GetByIdRequest request, int Case = 1)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.Case = Case;
                    ApiResponse response;
                    switch (Case)
                    {
                        case 1:
                            response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, request, HttpAction.Post);
                            if (response.Success)
                            {
                                var User = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                                User.ProfilePicture = GetProfilePicture(User.ProfilePicture);
                                User.CoverPicture = GetCoverPicture(User.CoverPicture);

                                var result = new POSTFriendRequest();
                                result.User = User;
                                return PartialView("~/Views/Home/Friend/PopupAddFriend.cshtml", result);
                            }
                            else
                            {
                                throw new Exception(response.Message);
                            }
                        case 2:
                            response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_GET_BY_POST, request, HttpAction.Post);
                            if (response.Success)
                            {
                                var result = JsonConvert.DeserializeObject<POSTFriendRequest>(response.Data.ToString());
                                result.User.ProfilePicture = GetProfilePicture(result.User.ProfilePicture);
                                result.User.CoverPicture = GetCoverPicture(result.User.CoverPicture);

                                return PartialView("~/Views/Home/Friend/PopupAddFriend.cshtml", result);
                            }
                            else
                            {
                                throw new Exception(response.Message);
                            }
                        case 3:
                            return PartialView("~/Views/Home/Friend/PopupAddFriend.cshtml");
                        default:
                            throw new Exception("Không tìm thấy trường hợp hiển thị popup");
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
        public IActionResult CreateFriendRequest(POSTFriendRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_INSERT, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<MODELFriendRequest>(response.Data.ToString());
                        return Json(new { IsSuccess = true, Message = "Gửi lời mời kết bạn thành công", Data = "" });
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
