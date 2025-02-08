using MODELS.BASE;
using MODELS.MESSAGELIST.Dtos;
using MODELS.MESSAGELIST.Requests;

namespace BE.Services.MessageList
{
    public interface IMESSAGELISTService
    {
        BaseResponse<MODELMessageList_Search> Search(MessageList_SearchRequest request);
    }
}
