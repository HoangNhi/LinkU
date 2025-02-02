using MODELS.BASE;
using MODELS.OTP.Requests;
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
        BaseResponse<MODELUser> Login(LoginRequest request, string ipAddress);
        BaseResponse<MODELUser> Register(RegisterRequest request, string ipAddress);
        BaseResponse<MODELToken> RefreshToken(string token, string ipAddress);
        BaseResponse<MODELUser> Logout(PostLogoutRequest request);
        BaseResponse<LoginRequest> CheckUsernameExist(UsernameRequest request);
        BaseResponse<MODELUser> LoginGoogle(LoginGoogleRequest request, string ipAddress);
        
        // Forgot Password
        BaseResponse SendOTP(UsernameRequest request);
        BaseResponse<string> VerifyOTP(VerifyOTPRequest request);
        BaseResponse ChangePassword(ChangePasswordRequest request);
    }
}
