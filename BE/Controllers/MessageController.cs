using BE.Services.Message;
using Microsoft.AspNetCore.Mvc;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Requests;
using MODELS.MESSAGE.Requests;
using System.Threading.Tasks;

namespace BE.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MessageController : BaseController<MessageController>
    {
        private readonly IMESSAGEService _service;

        public MessageController(IMESSAGEService MessageService)
        {
            _service = MessageService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> GetListPaging(PostMessageGetListPagingRequest request)
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
        public async Task<ActionResult<ApiResponse>> Insert(PostMessageRequest request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    var response = await _service.Insert(request);
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
        public ActionResult<ApiResponse> Update(PostMessageRequest request)
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
        public async Task<ActionResult<ApiResponse>> SendMessageWithFile(IFormCollection form)
        {
            try
            {
                var files = form.Files.ToList();
                // 1. Kiểm tra null
                if (files == null || files.Count == 0)
                    throw new Exception("Không có file được gửi lên");


                foreach (var file in files)
                {
                    // 2. Giới hạn kích thước (ví dụ: 20MB)
                    const long maxFileSize = 20 * 1024 * 1024; // 20MB
                    if (file.Length > maxFileSize)
                        throw new Exception("File vượt quá dung lượng cho phép (20MB).");
                }

                // 3. Kiểm tra SenderId, TargetId, RefId, Content
                var senderId = form["SenderId"];
                if (string.IsNullOrEmpty(senderId))
                    throw new Exception("SenderId không được để trống.");

                var targetId = form["TargetId"];
                if (string.IsNullOrEmpty(targetId))
                    throw new Exception("TargetId không được để trống.");

                var conversationType = form["ConversationType"];
                if (string.IsNullOrEmpty(conversationType))
                    throw new Exception("ConversationType không được để trống.");

                Guid? refId = null;
                if (Guid.TryParse(form["RefId"], out Guid parsedGuid))
                {
                    refId = parsedGuid;
                }
                var content = form["Content"];

                // 4. Upload nếu hợp lệ
                var response = await _service.SendMessageWithFile(new POSTSendMessageWithFileRequest
                {
                    Files = files,
                    SenderId = Guid.Parse(senderId),
                    TargetId = Guid.Parse(targetId),
                    RefId = refId,
                    Content = content,
                    ConversationType = string.IsNullOrEmpty(conversationType) ? 0 : int.Parse(conversationType)
                });

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
