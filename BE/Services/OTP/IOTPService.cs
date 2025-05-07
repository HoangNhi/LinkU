using MODELS.BASE;
using MODELS.OTP.Dtos;
using MODELS.OTP.Requests;

namespace BE.Services.OTP
{
    public interface IOTPService
    {
        BaseResponse<MODELOTP> Insert(string Username);
        BaseResponse<bool> Verify(VerifyOTPRequest request);
    }
}
