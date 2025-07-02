using MODELS.BASE;
using MODELS.MESSAGEREACTION.Dtos;
using MODELS.MESSAGEREACTION.Requests;

namespace BE.Services.MessageReaction
{
    public interface IMESSAGEREACTIONService
    {
        BaseResponse<MODELMessageReaction> Insert(POSTMessageReactionRequest request);
        BaseResponse<MODELMessageReaction> Update(POSTMessageReactionRequest request);
        BaseResponse<MODELMessageReaction> Delete(GetByIdRequest request);
        BaseResponse<MODELMessageReaction> HandleRequest(POSTMessageReactionRequest request);
        BaseResponse<GetListPagingResponse> GetListPaging(POSTMessageReactionGetListPagingRequest request);
    }
}
