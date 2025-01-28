using AutoMapper;
using BE.Services.SMS;
using ENTITIES.DbContent;
using MODELS.BASE;
using MODELS.OTP.Dtos;
using MODELS.OTP.Requests;
using MODELS.USER.Dtos;

namespace BE.Services.OTP
{
    public class OTPService : IOTPService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;

        public OTPService(LINKUContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public BaseResponse<MODELOTP> Insert(string Username)
        {
            var response = new BaseResponse<MODELOTP>();
            try
            {
                // Check if user exists
                var user = _context.Users.FirstOrDefault(x => x.Username == Username);
                if (user == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                else
                {
                    // Check if OTP exists
                    var PreviousOTP = _context.OTPs.Where(x => x.UserId == user.Id);
                    if (PreviousOTP != null)
                    {
                        foreach (var item in PreviousOTP)
                        {
                            item.NgaySua = DateTime.Now;
                            item.NguoiSua = "system";
                            item.IsActived = false;
                        }
                    }

                    // Generate OTP
                    var otp = new Random().Next(100000, 999999).ToString();

                    // Save OTP to database
                    var add = new ENTITIES.DbContent.OTP
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Code = otp,
                        Expires = DateTime.Now.AddMinutes(5),
                        NgayTao = DateTime.Now,
                        NguoiTao = "system",
                        NgaySua = DateTime.Now,
                        NguoiSua = "system",
                        IsActived = true,
                        IsDeleted = false
                    };

                    _context.OTPs.Add(add);
                    _context.SaveChanges();

                    response.Data = _mapper.Map<MODELOTP>(add);
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<bool> Verify(VerifyOTPRequest request)
        {
            var response = new BaseResponse<bool>();
            try
            {
                // Check if OTP exists
                var otp = _context.OTPs.OrderByDescending(x => x.NgayTao)
                                       .FirstOrDefault(x => x.UserId == request.UserId);
                if (otp == null)
                {
                    throw new Exception("Mã OTP không hợp lệ");
                }
                else
                {
                    if(otp.Code != request.Code)
                    {
                        throw new Exception("Mã OTP không hợp lệ");
                    }
                    else
                    {
                        if (otp.Expires < DateTime.Now)
                        {
                            throw new Exception("Mã OTP đã hết hạn");
                        }
                        else
                        {
                            if (otp.IsActived)
                            {
                                otp.NgaySua = DateTime.Now;
                                otp.NguoiSua = "system";
                                otp.IsActived = false;
                                _context.SaveChanges();
                                response.Data = true;
                            }
                            else
                            {
                                response.Data = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
