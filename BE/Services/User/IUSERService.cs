using MODELS.BASE;
using MODELS.CONVERSATION.Requests;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;
using MODELS.OTP.Requests;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;

namespace BE.Services.User
{
    public interface IUSERService
    {
        BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request);
        /// <summary>
        /// Lấy danh sách CoverPicture hoặc ProfilePicture của người dùng
        /// </summary>
        BaseResponse<List<MODELMediaFile>> GetListMediaFiles(POSTGetListMediaFilesRequest request);
        BaseResponse<MODELUser> GetById(GetByIdRequest request);
        BaseResponse<PostUpdateUserInforRequest> GetByPost(GetByIdRequest request);
        BaseResponse<MODELUser> Insert(PostUserRequest request);
        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseResponse<MODELUser> UpdateInfor(PostUpdateUserInforRequest request);
        /// <summary>
        /// Cập nhật hình ảnh đại diện và ảnh bìa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BaseResponse<MODELMediaFile> UpdatePicture(POSTMediaFileRequest request);
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
