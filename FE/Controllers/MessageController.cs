using FE.Constant;
using FE.Services;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MESSAGE.Dtos;
using Newtonsoft.Json;

namespace FE.Controllers
{
    public class MessageController : BaseController<MessageController>
    {
        private readonly ICONSUMEAPIService _consumeAPI;

        public MessageController(ICONSUMEAPIService consumeAPI)
        {
            _consumeAPI = consumeAPI;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
