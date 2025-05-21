using MODELS.BASE;
using MODELS.MESSAGELIST.Dtos;
using MODELS.MESSAGELIST.Requests;

namespace BE.Services.MessageList
{
    public interface IMESSAGELISTService
    {
        /// <summary>
        /// Tìm kiếm danh sách tin nhắn: Người dùng, tin nhắn, file đính kèm,...
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseResponse<MODELMessageList_Search> Search(MessageList_SearchRequest request);

        /// <summary>
        /// Lấy danh sách tin nhắn mới nhất
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseResponse<GetListPagingResponse> GetListMessageLatest(POSTGetListMessageLatestRequest request);
    }
}
