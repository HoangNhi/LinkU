using FE.Constant;
using FE.Services.ConsumeAPI;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.FRIENDSHIP.Requests;
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

        public IActionResult FriendRequestGetListPaging(POSTFriendRequestGetListPagingRequest request)
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
                        return PartialView($"~/Views/Home/Contact/FriendRequest/{(!request.IsSend ? "_ReceiveRequestPartial" : "_SentRequestPartial")}.cshtml", resultData);
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

        #region Tab Friendship
        [Route("friendship")]
        public IActionResult GetTapFriendship()
        {
            return PartialView("~/Views/Home/Contact/Friendship/_TabFriendshipPartial.cshtml");
        }

        public IActionResult FriendshipGetListPaging(POSTFriendshipGetListPagingRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.FRIENDSHIP_GET_LIST_PAGING, request, HttpAction.Post);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                        result.Data = JsonConvert.DeserializeObject<List<MODELUser>>(result.Data.ToString());
                        return PartialView($"~/Views/Home/Contact/Friendship/_FriendshipGetListPagingPartial.cshtml", result);
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
                    if (Case == 1)
                    {
                        response = _consumeAPI.ExcuteAPI(URL_API.USER_GET_BY_ID, request, HttpAction.Post);
                        if (response.Success)
                        {
                            var User = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());

                            var result = new POSTFriendRequest();
                            result.Id = Guid.NewGuid();
                            result.User = User;
                            return PartialView("~/Views/Home/Friend/PopupAddFriend.cshtml", result);
                        }
                        else
                        {
                            throw new Exception(response.Message);
                        }
                    }
                    else // Case 2, 3
                    {
                        response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_GET_BY_POST, request, HttpAction.Post);
                        if (response.Success)
                        {
                            var result = JsonConvert.DeserializeObject<POSTFriendRequest>(response.Data.ToString());

                            return PartialView("~/Views/Home/Friend/PopupAddFriend.cshtml", result);
                        }
                        else
                        {
                            throw new Exception(response.Message);
                        }
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
            catch (Exception ex)
            {
                string message = "Lỗi hệ thống: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [HttpPost]
        public IActionResult DeleteFriendRequest(GetByIdRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.FRIENDREQUEST_DELETE, request, HttpAction.Post);
                    if (response.Success)
                    {
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
