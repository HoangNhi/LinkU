using Microsoft.AspNetCore.Mvc;

namespace FE.Controllers
{
    public class ContactController : BaseController<ContactController>
    {
        public IActionResult Index()
        {
            return PartialView("~/Views/Home/Contact/_TabContactPartial.cshtml");
        }
    }
}
