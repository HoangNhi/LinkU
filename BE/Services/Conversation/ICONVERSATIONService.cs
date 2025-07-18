﻿using MODELS.BASE;
using MODELS.CONVERSATION.Requests;
using MODELS.GROUP.Dtos;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.MESSAGESTATUS.Requests;

namespace BE.Services.Conversation
{
    public interface ICONVERSATIONService
    {
        Task<BaseResponse<GetListPagingResponse>> GetListPaging(POSTConversationGetListPagingRequest request);
        BaseResponse<GetListPagingResponse> SearchUserByEmailOrPhone(POSTSearchInConversationRequest request);
        BaseResponse<MODELConversation> Insert(POSTConversationRequest request);
        BaseResponse<MODELConversation> Update(POSTConversationRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);

        #region Xử lý request từ Websocket
        /// <summary>
        /// Tạo conversation qua websocket SendPrivateMessage
        /// Hàm sẽ tạo conversation với cả 2 người dùng
        //// </summary>
        //BaseResponse<Guid[]> InsertPrivateConversation(WSPrivateMessageInsertConversation request);
        //BaseResponse<bool> CheckConversationExist(Guid userId, Guid targetId);
        //BaseResponse<Dictionary<Guid, List<Guid>>> GetDictionaryConversationUserToUser();
        /// <summary>
        /// Delete conversation trong trường hợp roolback
        /// </summary> 
        //BaseResponse<bool> RoolbackDelete(GetByIdRequest request);

        Task<BaseResponse> UpdateLatestMessage(Guid UserId, Guid TargetId);

        BaseResponse<MODELGroupAvartar> GetGroupAvartar(GetByIdRequest request);

        #endregion
    }
}
