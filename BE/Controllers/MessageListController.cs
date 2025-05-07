using BE.Services.MessageList;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MESSAGELIST.Requests;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MessageListController : BaseController<MessageListController>
    {
        private readonly IMESSAGELISTService _service;

        public MessageListController(IMESSAGELISTService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse> Search(MessageList_SearchRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Search(request);
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
    }
}
