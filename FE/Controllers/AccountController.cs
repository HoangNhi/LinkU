using FE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.USER.Requests;
using MODELS.COMMON;
using FE.Constant;
using MODELS.BASE;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Security.Claims;
using MODELS.USER.Dtos;

namespace FE.Controllers
{
    public class AccountController : BaseController<AccountController>
    {
        private readonly ICONSUMEAPIService _consumeAPI;
        public AccountController(ICONSUMEAPIService consumeAPI)
        {
            _consumeAPI = consumeAPI;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewData["Title"] = "Đăng nhập";
            return View("~/Views/Account/Login.cshtml", new UsernameRequest());
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
                        var UserDate = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                        var claims = new List<Claim>();

                        claims.Add(new Claim("Id", UserDate.Id.ToString()));
                        claims.Add(new Claim("Name", UserDate.Username));
                        claims.Add(new Claim("Fullname", UserDate.HoVaTen));
                        claims.Add(new Claim("Email", UserDate.Email));
                        claims.Add(new Claim("Phone", UserDate.SoDienThoai));
                        claims.Add(new Claim("Role", UserDate.RoleId.ToString()));
                        claims.Add(new Claim("Token", UserDate.AccessToken));

                        // Create the identity from the user info
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        // Create the principal
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        // Save token in cookie
                        await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                        {
                            IsPersistent = true, // Giữ cookie sau khi trình duyệt đóng
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(CommonConst.ExpireAccessToken) // Hạn sử dụng là 7 ngày
                        });

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
                    ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_CheckUsernameExist, request, HttpAction.Post);
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
                return View("~/Views/Account/Password.cshtml", new LoginRequest { Username = request.Username});
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml", new RegisterRequest());
        }
    }
}
