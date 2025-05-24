using BE.Services.Group;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.GROUP.Requests;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class GroupController : BaseController<GroupController>
    {
        private readonly IGROUPService _service;

        public GroupController(IGROUPService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse> GetListPaging(GetListPagingRequest request)
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
        public ActionResult<ApiResponse> GetById(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetById(request);
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
        public ActionResult<ApiResponse> GetByPost(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetByPost(request);
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
        public ActionResult<ApiResponse> Insert(POSTGroupRequest request)
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
        public ActionResult<ApiResponse> Update(POSTGroupRequest request)
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
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateGroupWithMember([FromForm] POSTCreateGroupRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = await _service.CreateGroupWithMember(request);
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

        [HttpGet]
        public ActionResult<ApiResponse> GetListMemberCreateGroup()
        {
            try
            {
                var response = _service.GetListMemberCreateGroup();
                if (response.Error)
                {
                    throw new Exception(response.Message);
                }
                return Ok(new ApiResponse(response.Data));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(false, 500, ex.Message));
            }
        }
    }
}
