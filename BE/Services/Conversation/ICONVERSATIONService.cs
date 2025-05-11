using MODELS.BASE;
using MODELS.CONVERSATION.Requests;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.MESSAGESTATUS.Requests;

namespace BE.Services.Conversation
{
    public interface ICONVERSATIONService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(POSTConversationGetListPagingRequest request);
        BaseResponse<MODELConversation> Insert(POSTConversationRequest request);
        BaseResponse<MODELConversation> Update(POSTConversationRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);

        basere
    }
}
