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

        public IActionResult GetListPaging(GetListPagingRequest request)
        {
            try
            {
                var result = new GetListPagingResponse();

                GetListPagingRequest param = new GetListPagingRequest();
                param.PageIndex = request.PageIndex - 1;
                param.RowPerPage = request.RowPerPage;
                param.TextSearch = request.TextSearch == null ? string.Empty : request.TextSearch.Trim();

                ApiResponse response = _consumeAPI.ExcuteAPI(URL_API.MESSAGE_GET_LIST_PAGING, request, HttpAction.Post);
                if (response.Success)
                {
                    result = JsonConvert.DeserializeObject<GetListPagingResponse>(response.Data.ToString());
                    result.Data = JsonConvert.DeserializeObject<List<MODELMessage>>(result.Data.ToString());
                    return Json(new { IsSuccess = true, Message = "", Data = result });
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                string message = "Lỗi lấy danh sách: " + ex.Message;
                return Json(new { IsSuccess = false, Message = message, Data = "" });
            }
        }
    }
}
