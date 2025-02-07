using Microsoft.AspNetCore.Mvc;

namespace FE.Controllers
{
    public class MessageListController : BaseController<MessageListController>
    {
        public IActionResult Index()
        {
            return PartialView("~/Views/Home/MessageList/_SearchPartial.cshtml");
        }
    }
}
