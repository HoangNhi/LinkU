using MODELS.BASE;
using MODELS.FRIENDSHIP.Dtos;
using MODELS.FRIENDSHIP.Requests;

namespace BE.Services.FriendShip
{
    public interface IFRIENDSHIPService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(POSTFriendshipGetListPagingRequest request);
        BaseResponse<MODELFriendship> Insert(POSTFriendshipRequest request);
    }
}
