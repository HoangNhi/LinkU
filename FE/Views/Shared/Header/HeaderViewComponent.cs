using Microsoft.AspNetCore.Mvc;
using MODELS.USER.Dtos;

namespace FE.Views.Shared.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        IHttpContextAccessor _contextAccessor;

        public HeaderViewComponent(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            MODELUser model = new MODELUser();

            foreach (var claim in _contextAccessor.HttpContext.User.Claims)
            {
                switch (claim.Type)
                {
                    case "HoLot": { model.HoLot = claim.Value; }; break;
                    case "Ten": { model.Ten = claim.Value; }; break;
                    case "ProfilePicture": { model.ProfilePicture = claim.Value; }; break;
                }
            }

            ViewBag.UserInfo = model;
            return View("~/Views/Shared/Header/Default.cshtml");
        }
    }
}
