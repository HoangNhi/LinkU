using MODELS.BASE;

namespace BE.Services.MessageList
{
    public interface IMESSAGELISTService
    {
        BaseResponse<GetListPagingResponse> Search(GetListPagingRequest request);
    }
}
