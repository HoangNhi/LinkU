using BE.Services.Conversation;
using BE.Services.MessageList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.CONVERSATION.Requests;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.MESSAGESTATUS.Requests;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ConversationController : BaseController<ConversationController>
    {
        private readonly ICONVERSATIONService _service;

        public ConversationController(ICONVERSATIONService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetListPaging(POSTConversationGetListPagingRequest request)
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

        [HttpPost]
        public ActionResult<ApiResponse> SearchUserByEmailOrPhone(POSTSearchInConversationRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.SearchUserByEmailOrPhone(request);
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
        [HttpPost]
        public ActionResult<ApiResponse> Insert(POSTConversationRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Insert(request);
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

        [HttpPost]
        public ActionResult<ApiResponse> Update(POSTConversationRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Update(request);
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

        [HttpPost]
        public ActionResult<ApiResponse> DeleteList(DeleteListRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.DeleteList(request);
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
