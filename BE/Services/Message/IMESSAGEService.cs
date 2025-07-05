using MODELS.BASE;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;

namespace BE.Services.Message
{
    public interface IMESSAGEService
    {
        Task<BaseResponse<GetListPagingResponse>> GetListPaging(PostMessageGetListPagingRequest request);
        BaseResponse<MODELMessage> GetById(GetByIdRequest request);
        BaseResponse<PostMessageRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELMessage> Insert(PostMessageRequest request);
        BaseResponse<MODELMessage> Update(PostMessageRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);
        Task<BaseResponse<List<MODELSendMessageWithFileResponse>>> SendMessageWithFile(POSTSendMessageWithFileRequest request);
        BaseResponse<List<MODELMessage>> HanleDataGetListPaging(List<MODELMessage> result, int conversationType, Guid UserId, Guid TargetId);
        Task<BaseResponse<List<MODELMessage>>> HanleDataGetListPagingAsync(List<MODELMessage> result, int conversationType, Guid UserId, Guid TargetId);
        BaseResponse<MODELMessage> WSInsertPrivateMessage(PostMessageRequest request);
        #region Xử lý request từ Websocket
        BaseResponse<bool> RoolbackDelete(GetByIdRequest request);
        #endregion
    }
}
