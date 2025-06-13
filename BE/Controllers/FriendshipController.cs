using BE.Services.FriendShip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.CONVERSATION.Requests;
using MODELS.FRIENDSHIP.Requests;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FriendshipController : BaseController<FriendshipController>
    {
        private readonly IFRIENDSHIPService _service;

        public FriendshipController(IFRIENDSHIPService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetListPaging(POSTFriendshipGetListPagingRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetListPaging(request);
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
