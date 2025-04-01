using FE.Services;
using Microsoft.AspNetCore.Mvc;

namespace FE.Controllers
{
    public class ContactController : BaseController<ContactController>
    {
        public ContactController(ICONSUMEAPIService consumeAPI) : base(consumeAPI)
        {
        }
        public IActionResult Index()
        {
            return PartialView("~/Views/Home/Contact/_TabContactPartial.cshtml");
        }
    }
}
