﻿using BE.Services.FriendRequest;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.FRIENDREQUEST.Requests;
using System.Threading.Tasks;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FriendRequestController : BaseController<FriendRequestController>
    {
        private readonly IFRIENDREQUESTService _service;

        public FriendRequestController(IFRIENDREQUESTService service)
        {
            _service = service;
        }

        // Base
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> GetListPaging(POSTFriendRequestGetListPagingRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = await _service.GetListPaging(request);
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
        public async Task<ActionResult<ApiResponse>> GetByPost(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = await _service.GetByPost(request);
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
        public ActionResult<ApiResponse> Insert(POSTFriendRequest request)
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
        public ActionResult<ApiResponse> Update(POSTFriendRequest request)
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
        public ActionResult<ApiResponse> Delete(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.Delete(request);
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

        // Kiểm tra
        [HttpPost]
        public ActionResult<ApiResponse> GetFriendRequestStatus(GetByIdRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = _service.GetFriendRequestStatus(request);
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
