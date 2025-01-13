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
using Microsoft.AspNetCore.Authentication.Google;

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
                        var UserData = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                        var claims = new List<Claim>();

                        claims.Add(new Claim("Id", UserData.Id.ToString()));
                        claims.Add(new Claim("Name", UserData.Username));
                        claims.Add(new Claim("HoLot", UserData.HoLot));
                        claims.Add(new Claim("Ten", UserData.Ten));
                        claims.Add(new Claim("ProfilePicture", String.IsNullOrEmpty(UserData.ProfilePicture) || String.IsNullOrEmpty(UserData.ProfilePicture) ? _consumeAPI.GetBEUrl() + "/Files/Common/NoPicture.png" : _consumeAPI.GetBEUrl() + "/" + UserData.ProfilePicture));
                        claims.Add(new Claim("Role", UserData.RoleId.ToString()));
                        claims.Add(new Claim("Token", UserData.AccessToken));

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
                var HoLot = claimsGoogle.FirstOrDefault(x => x.Type.EndsWith("surname") || x.Type.EndsWith("name")).Value;
                var Ten = claimsGoogle.FirstOrDefault(x => x.Type.EndsWith("givenname")).Value;

                ApiResponse response = _consumeAPI.ExcuteAPIWithoutToken(URL_API.USER_LOGINGOOGLE, new LoginGoogleRequest { Username = Username , HoVaTen = HoLot + " " + Ten}, HttpAction.Post);
                if (response.Success)
                {
                    var UserData = JsonConvert.DeserializeObject<MODELUser>(response.Data.ToString());
                    var claims = new List<Claim>();

                    claims.Add(new Claim("Id", UserData.Id.ToString()));
                    claims.Add(new Claim("Name", UserData.Username));
                    claims.Add(new Claim("HoLot", UserData.HoLot));
                    claims.Add(new Claim("Ten", UserData.Ten));
                    claims.Add(new Claim("ProfilePicture", String.IsNullOrEmpty(UserData.ProfilePicture) || String.IsNullOrEmpty(UserData.ProfilePicture) ? _consumeAPI.GetBEUrl() + "/Files/Common/NoPicture.png" : _consumeAPI.GetBEUrl() + "/" + UserData.ProfilePicture));
                    claims.Add(new Claim("Role", UserData.RoleId.ToString()));
                    claims.Add(new Claim("Token", UserData.AccessToken));

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

                    return Redirect(Url.Action("Index", "Home"));
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
