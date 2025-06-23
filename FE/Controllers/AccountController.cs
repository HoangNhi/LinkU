using FE.Constant;
using FE.Services.ConsumeAPI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.OTP.Requests;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FE.Controllers
{
    public class AccountController : BaseController<AccountController>
    {
        public AccountController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }
        #region Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["Title"] = "Đăng ký";

            return View("~/Views/Account/Register.cshtml", new RegisterRequest());
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_REGISTER, request, HttpAction.Post);
                    if (response.Success)
                    {
                        return Json(new { IsSuccess = true, Message = "Đăng ký thành công", Data = "" });
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
                string message = "Lỗi đăng ký: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [AllowAnonymous]
        public IActionResult GoogleRegister(string Username)
        {
            ViewData["Title"] = "Thông tin tài khoản";
            return View("~/Views/Account/GoogleRegister.cshtml", new RegisterRequest { Username = Username, IsGoogle = true });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GoogleRegister(RegisterRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_REGISTER, request, HttpAction.Post);
                    if (response.Success)
                    {
                        SetClaimLogin(response.Data.ToString());
                        return Json(new { IsSuccess = true, Message = "Cập nhật dữ liệu thành công", Data = "" });
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
                string message = "Lỗi cập nhật thông tin: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
        #endregion

        #region Forgot Password
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            ViewData["Title"] = "Quên mật khẩu";
            return View("~/Views/Account/ForgetPassword.cshtml", new UsernameRequest());
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SendOTP(string Username)
        {
            try
            {
                ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_SENDOTP, new UsernameRequest { Username = Username }, HttpAction.Post);
                if (response.Success)
                {
                    return PartialView("~/Views/Account/VerifyOTP.cshtml", new VerifyOTPRequest { Username = Username });
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi cập nhật thông tin: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult VerifyOTP(VerifyOTPRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_VERIFYOTP, request, HttpAction.Post);
                    if (response.Success)
                    {
                        return PartialView("~/Views/Account/ChangePassword.cshtml", new ChangePasswordRequest { Token = response.Data.ToString() });
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
                string message = "Lỗi: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_CHANGEPASSWORD, request, HttpAction.Post);
                    if (response.Success)
                    {
                        return Json(new { IsSuccess = true, Message = "Đổi mật khẩu thành công", Data = "" });
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
                string message = "Lỗi: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
        #endregion

        #region Login/LogOut
        [AllowAnonymous]
        public IActionResult Login(string userName = "")
        {
            ViewData["Title"] = "Đăng nhập";
            if (userName == "")
            {
                return View("~/Views/Account/Login.cshtml", new UsernameRequest());
            }
            else
            {
                return View("~/Views/Account/Login.cshtml", new UsernameRequest { Username = userName });
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_LOGIN, request, HttpAction.Post);
                    if (response.Success)
                    {
                        // Save token in cookie
                        SetClaimLogin(response.Data.ToString());
                        return Json(new { IsSuccess = true, Message = "Đăng nhập thành công", Data = "" });
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
                string message = "Lỗi đăng nhập: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CheckUsername(UsernameRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_CHECKUSERNAMEEXIST, request, HttpAction.Post);
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
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [Route("/Account/Login/Password")]
        [AllowAnonymous]
        public IActionResult Password(UsernameRequest request)
        {
            if (request != null && ModelState.IsValid)
            {
                ViewData["Title"] = "Đăng nhập";
                return View("~/Views/Account/Password.cshtml", new LoginRequest { Username = request.Username });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SigninGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (result == null || !result.Succeeded)
                {
                    // Xóa cookie hiện tại
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Login");
                }

                // Đăng nhập thành công sẽ lấy được các claim như: email, name, ...
                var claimsGoogle = result.Principal.Identities
                    .FirstOrDefault().Claims.Select(claim => new
                    {
                        claim.Issuer,
                        claim.OriginalIssuer,
                        claim.Type,
                        claim.Value
                    });

                var Username = claimsGoogle.FirstOrDefault(x => x.Type.EndsWith("emailaddress")).Value;

                ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_LOGINGOOGLE, new LoginGoogleRequest { Username = Username }, HttpAction.Post);
                if (response.Success)
                {
                    // Save token in cookie
                    SetClaimLogin(response.Data.ToString());
                    return Redirect(Url.Action("Index", "Home"));
                }
                else
                {
                    if (response.Message == "AccountNotExist")
                    {
                        return RedirectToAction("GoogleRegister", new { Username = Username });
                    }
                    else
                    {
                        throw new Exception(response.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi đăng ký: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        #endregion

        #region Private Method
        private async void SetClaimLogin(string ResponseData)
        {
            var UserData = JsonConvert.DeserializeObject<MODELUser>(ResponseData);
            var claims = new List<Claim>();

            claims.Add(new Claim("UserId", UserData.Id.ToString()));
            claims.Add(new Claim("Name", UserData.HoVaTen));
            //claims.Add(new Claim("HoLot", UserData.HoLot));
            //claims.Add(new Claim("Ten", UserData.Ten));
            //claims.Add(new Claim("ProfilePicture", String.IsNullOrEmpty(UserData.ProfilePicture) || String.IsNullOrEmpty(UserData.ProfilePicture) ? _consumeAPI.GetBEUrl() + "/Files/Common/NoPicture.png" : _consumeAPI.GetBEUrl() + "/" + UserData.ProfilePicture));
            //claims.Add(new Claim("Role", UserData.RoleId.ToString()));
            claims.Add(new Claim("AccessToken", UserData.AccessToken));
            claims.Add(new Claim("RefreshToken", UserData.RefreshToken));

            // Create the identity from the user info
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Create the principal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Save token in cookie
            await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true, // Giữ cookie sau khi trình duyệt đóng
            });
        }

        #endregion
    }
}
