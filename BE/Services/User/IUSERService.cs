using MODELS.BASE;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.REFRESHTOKEN.Requests;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;

namespace BE.Services.User
{
    public interface IUSERService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request);
        BaseResponse<MODELUser> GetById(GetByIdRequest request);
        BaseResponse<PostUserRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELUser> Insert(PostUserRequest request);
        BaseResponse<MODELUser> Update(PostUserRequest request);
        BaseResponse<string> DeleteList(DeleteListRequest request);

        // Login by UserName and Password
        BaseResponse<MODELUser> Login(LoginRequest request);
        BaseResponse<MODELUser> Register(RegisterRequest request);
        BaseResponse<MODELRefreshToken> RefreshToken(PostRefreshTokenRequest request);
        BaseResponse<MODELUser> Logout(PostLogoutRequest request);
        BaseResponse<LoginRequest> CheckUsernameExist(UsernameRequest request);
        BaseResponse<MODELUser> LoginGoogle(LoginGoogleRequest request);    
    }
}
