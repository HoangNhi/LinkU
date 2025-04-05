using MODELS.BASE;
using MODELS.FRIENDSHIP.Dtos;
using MODELS.FRIENDSHIP.Requests;

namespace BE.Services.FriendShip
{
    public interface IFRIENDSHIPService
    {
        // Base
        //BaseResponse<GetListPagingResponse> GetListPaging(POSTFriendRequestGetListPagingRequest request);
        //BaseResponse<MODELFriendRequest> GetById(GetByIdRequest request);
        //BaseResponse<POSTFriendRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELFriendship> Insert(POSTFriendshipRequest request);
        //BaseResponse<MODELFriendRequest> Update(POSTFriendRequest request);
        //BaseResponse<string> Delete(GetByIdRequest request);
    }
}
