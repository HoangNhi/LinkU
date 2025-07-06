using MODELS.BASE;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;

namespace BE.Services.FriendRequest
{
    public interface IFRIENDREQUESTService
    {
        // Base
        Task<BaseResponse<GetListPagingResponse>> GetListPaging(POSTFriendRequestGetListPagingRequest request);
        BaseResponse<MODELFriendRequest> GetById(GetByIdRequest request);
        Task<BaseResponse<POSTFriendRequest>> GetByPost(GetByIdRequest request);
        BaseResponse<MODELFriendRequest> Insert(POSTFriendRequest request);
        BaseResponse<MODELFriendRequest> Update(POSTFriendRequest request);
        BaseResponse<string> Delete(GetByIdRequest request);

        // Kiểm tra
        BaseResponse<MODELFriendStatus> GetFriendRequestStatus(GetByIdRequest request);

    }
}
