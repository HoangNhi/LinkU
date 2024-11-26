using MODELS.BASE;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.REFRESHTOKEN.Requests;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;

namespace BE.Services.User
{
    public interface IUSERService
    {
        Task<BaseResponse<GetListPagingResponse>> GetListPaging(GetListPagingRequest request);
        Task<BaseResponse<MODELUser>> GetById(GetByIdRequest request);
        Task<BaseResponse<PostUserRequest>> GetByPost(GetByIdRequest request);
        Task<BaseResponse<MODELUser>> Insert(PostUserRequest request);
        Task<BaseResponse<MODELUser>> Update(PostUserRequest request);
        Task<BaseResponse<string>> DeleteList(DeleteListRequest request);

        // Login by UserName and Password
        Task<BaseResponse<MODELUser>> Login(LoginRequest request);
        Task<BaseResponse<MODELRefreshToken>> RefreshToken(PostRefreshTokenRequest request);
        Task<BaseResponse<MODELUser>> Logout(PostLogoutRequest request);
    }
}
