using BE.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Requests;
using MODELS.OTP.Requests;
using MODELS.REFRESHTOKEN.Requests;
using MODELS.USER.Requests;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        private readonly IUSERService _service;

        public UserController(IUSERService userService)
        {
            _service = userService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetListPaging(GetListPagingRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetListPaging(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetById(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetById(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetByPost(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetByPost(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> Insert(PostUserRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Insert(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> UpdateInfor(PostUpdateUserInforRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.UpdateInfor(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> UpdatePicture(POSTMediaFileRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.UpdatePicture(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> DeleteList(DeleteListRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.DeleteList(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> Login(LoginRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Login(request, ipAddress());
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> Register(RegisterRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Register(request, ipAddress());
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.RefreshToken(request.Token, ipAddress());
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> Logout(PostLogoutRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Logout(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> CheckUsernameExist(UsernameRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.CheckUsernameExist(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> LoginGoogle(LoginGoogleRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.LoginGoogle(request, ipAddress());
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> SendOTP(UsernameRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.SendOTP(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(true, 200));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> VerifyOTP(VerifyOTPRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.VerifyOTP(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ApiResponse> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.ChangePassword(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(true, 200));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetListMediaFiles(POSTGetListMediaFilesRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetListMediaFiles(request);
                    if (response.Error)
                    {
                        throw new Exception(response.Message);
                    }
                    return Ok(new ApiResponse(response.Data));
                }
                else
                {
                    throw new Exception(CommonFunc.GetModelStateAPI(ModelState));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }
        private string ipAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
