using FE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.USER.Requests;

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
            return View("~/Views/Account/Login.cshtml", new UsernameRequest());
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml", new RegisterRequest());
        }
    }
}
