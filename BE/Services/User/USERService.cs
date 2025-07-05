using AutoMapper;
using BE.Helpers;
using BE.Services.Mail;
using BE.Services.MediaFile;
using BE.Services.OTP;
using BE.Services.SMS;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MAIL.Dtos;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;
using MODELS.OTP.Requests;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace BE.Services.User
{
    public class USERService : IUSERService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMAILService _mailService;
        private readonly ISMSService _smsService;
        private readonly IOTPService _otpService;
        private readonly IMEDIAFILEService _mediaFileService;

        public USERService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration config, IMAILService mailService, ISMSService smsService, IOTPService otpService, IMEDIAFILEService mediaFileService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _mailService = mailService;
            _smsService = smsService;
            _otpService = otpService;
            _mediaFileService = mediaFileService;
        }

        #region Common Method - CRUD
        public BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request)
        {
            var response = new BaseResponse<GetListPagingResponse>();
            try
            {
                SqlParameter iTotalRow = new SqlParameter()
                {
                    ParameterName = "@oTotalRow",
                    SqlDbType = System.Data.SqlDbType.BigInt,
                    Direction = System.Data.ParameterDirection.Output
                };


                var parameters = new[]
                {
                    new SqlParameter("@iTextSearch", request.TextSearch),
                    new SqlParameter("@iPageIndex", request.PageIndex - 1),
                    new SqlParameter("@iRowsPerPage", request.RowPerPage),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELUser>("sp_USER_GetListPaging", parameters).ToList();
                GetListPagingResponse responseData = new GetListPagingResponse();
                responseData.PageIndex = request.PageIndex;
                responseData.Data = result;
                responseData.TotalRow = Convert.ToInt32(iTotalRow.Value);
                response.Data = responseData;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<List<MODELMediaFile>> GetListMediaFiles(POSTGetListMediaFilesRequest request)
        {
            var response = new BaseResponse<List<MODELMediaFile>>();
            try
            {
                var result = _context.MediaFiles.Where(m => m.OwnerId == request.UserId
                                                            && m.FileType == request.FileType
                                                            && !m.IsDeleted)
                                                .OrderByDescending(m => m.IsActived)
                                                .ThenByDescending(m => m.NgaySua);
                response.Data = _mapper.Map<List<MODELMediaFile>>(result);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }

            return response;
        }

        public BaseResponse<MODELUser> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var user = _context.Users.Find(request.Id);
                if (user == null)
                    throw new Exception("Không tìm thấy dữ liệu");

                var result = _mapper.Map<MODELUser>(user);

                // Lấy cả hai loại hình ảnh cùng lúc
                var mediaFiles = _context.MediaFiles
                    .Where(m => m.OwnerId == result.Id && !m.IsDeleted && m.IsActived)
                    .ToList();

                result.ProfilePicture = mediaFiles
                    .FirstOrDefault(m => m.FileType == (int)MODELS.COMMON.MediaFileType.ProfilePicture)?.Url
                    ?? CommonConst.DefaultUrlNoPicture;

                result.CoverPicture = mediaFiles
                    .FirstOrDefault(m => m.FileType == (int)MODELS.COMMON.MediaFileType.CoverPicture)?.Url
                    ?? CommonConst.DefaultUrlNoCoverPicture;

                // Xóa thông tin nhạy cảm
                result.Password = string.Empty;
                result.PasswordSalt = string.Empty;

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<MODELUser>> GetByIdAsync(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var result = new MODELUser();

                var data = await _context.Users.FindAsync(request.Id);
                if (data == null)
                {
                    return new BaseResponse<MODELUser> { Error = true, Message = "Không tìm thấy người dùng." };
                }

                var profilePicture = await _context.MediaFiles
                    .FirstOrDefaultAsync(m => m.OwnerId == result.Id && m.FileType == (int)MODELS.COMMON.MediaFileType.ProfilePicture && !m.IsDeleted && m.IsActived);
                var coverPicture = await _context.MediaFiles
                    .FirstOrDefaultAsync(m => m.OwnerId == result.Id && m.FileType == (int)MODELS.COMMON.MediaFileType.CoverPicture && !m.IsDeleted && m.IsActived);

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<PostUpdateUserInforRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<PostUpdateUserInforRequest>();
            try
            {
                var result = new PostUpdateUserInforRequest();
                var data = _context.Users.Find(request.Id);
                if (data == null)
                {
                    result.IsEdit = false;
                }
                else
                {
                    result = _mapper.Map<PostUpdateUserInforRequest>(data);
                    result.HoVaTen = data.HoLot + " " + data.Ten;
                    result.IsEdit = true;

                    var ProfilePicture = _context.MediaFiles
                                                 .FirstOrDefault(m => m.OwnerId == result.Id
                                                                && m.FileType == (int)MODELS.COMMON.MediaFileType.ProfilePicture
                                                                && !m.IsDeleted && m.IsActived);
                    if (ProfilePicture != null)
                    {
                        result.ProfilePicture = ProfilePicture.Url;
                    }

                    var CoverPicture = _context.MediaFiles
                                                 .FirstOrDefault(m => m.OwnerId == result.Id
                                                                && m.FileType == (int)MODELS.COMMON.MediaFileType.CoverPicture
                                                               && !m.IsDeleted && m.IsActived);
                    if (CoverPicture != null)
                    {
                        result.CoverPicture = CoverPicture.Url;
                    }
                }
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELUser> Insert(PostUserRequest request)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var checkUsernameExist = _context.Users.Where(x => x.Username == request.Username);
                if (checkUsernameExist.Any())
                {
                    throw new Exception("Tên tài khoản đã tồn tại");
                }

                var checkEmailExist = _context.Users.Where(x => x.Email == request.Email);
                if (checkEmailExist.Any())
                {
                    throw new Exception("Email đã tồn tại");
                }

                var checkPhoneExist = _context.Users.Where(x => x.SoDienThoai == request.SoDienThoai);
                if (checkPhoneExist.Any())
                {
                    throw new Exception("Số điện thoại đã tồn tại");
                }

                if (!CommonFunc.IsValidEmail(request.Email))
                    throw new Exception("Email không đúng định dạng");

                if (!CommonFunc.IsValidPhone(request.SoDienThoai))
                    throw new Exception("Số điện thoại không đúng định dạng");

                var add = _mapper.Map<ENTITIES.DbContent.User>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.PasswordSalt = Encrypt_DecryptHelper.GenerateSalt();
                add.Password = Encrypt_DecryptHelper.EncodePassword(request.Password, add.PasswordSalt);
                //add.ProfilePicture = UploadPicture(request.FolderUpload, "", "Files/User/ProfilePicture");
                //add.CoverPicture = UploadPicture(request.FolderUploadCoverPicture, "", "Files/User/CoverPicture");
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgayTao = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;

                // Lưu dữ liệu
                _context.Users.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELUser>(add);

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        //public BaseResponse<MODELUser> Update(PostUserRequest request)
        //{
        //    var response = new BaseResponse<MODELUser>();
        //    try
        //    {
        //        var checkUsernameExist = _context.Users.Where(x => x.Username == request.Username);
        //        if (checkUsernameExist.Any())
        //        {
        //            throw new Exception("Tên tài khoản đã tồn tại");
        //        }

        //        var checkEmailExist = _context.Users.Where(x => x.Email == request.Email);
        //        if (checkEmailExist.Any())
        //        {
        //            throw new Exception("Email đã tồn tại");
        //        }

        //        var checkPhoneExist = _context.Users.Where(x => x.SoDienThoai == request.SoDienThoai);
        //        if (checkPhoneExist.Any())
        //        {
        //            throw new Exception("Số điện thoại đã tồn tại");
        //        }

        //        if (!CommonFunc.IsValidEmail(request.Email))
        //            throw new Exception("Email không đúng định dạng");

        //        if (!CommonFunc.IsValidPhone(request.SoDienThoai))
        //            throw new Exception("Số điện thoại không đúng định dạng");

        //        var update = _context.Users.Find(request.Id);
        //        if (update == null)
        //        {
        //            throw new Exception("Không tìm thấy dữ liệu");
        //        }
        //        else
        //        {
        //            _mapper.Map(request, update);
        //            if (request.Password != "Abc@123")
        //            {
        //                update.Password = Encrypt_DecryptHelper.EncodePassword(request.Password, update.PasswordSalt);
        //            }

        //            update.ProfilePicture = UploadPicture(request.FolderUpload, update.ProfilePicture, "Files/User/ProfilePicture");
        //            update.CoverPicture = UploadPicture(request.FolderUploadCoverPicture, update.CoverPicture, "Files/User/CoverPicture");
        //            update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
        //            update.NgaySua = DateTime.Now;

        //            // Lưu dữ liệu
        //            _context.Users.Update(update);
        //            _context.SaveChanges();

        //            response.Data = _mapper.Map<MODELUser>(update);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Error = true;
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        public BaseResponse<MODELUser> UpdateInfor(PostUpdateUserInforRequest request)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var update = _context.Users.Find(request.Id);
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    // Họ và tên
                    var nameParts = request.HoVaTen.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    update.HoLot = string.Join(" ", nameParts.Take(nameParts.Length - 1));
                    update.Ten = nameParts.Last();

                    // Ngày sinh và giới tính
                    update.DateOfBirth = request.DateOfBirth;
                    update.Gender = request.Gender;

                    // Thông tin khác
                    update.NgaySua = DateTime.Now;
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                    // Lưu dữ liệu
                    _context.Users.Update(update);
                    _context.SaveChanges();

                    // Trả về dữ liệu
                    response.Data = _mapper.Map<MODELUser>(update);
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Cập nhật hình ảnh đại diện và ảnh bìa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaseResponse<MODELMediaFile> UpdatePicture(POSTMediaFileRequest request)
        {
            var response = new BaseResponse<MODELMediaFile>();
            try
            {
                // Cập nhật tất cả hình ảnh về trạng thái IsActived = False
                var Pictures = _context.MediaFiles.Where(x => x.OwnerId == request.OwnerId
                                                        && !x.IsDeleted && x.IsActived
                                                        && x.FileType == request.FileType);
                foreach (var item in Pictures)
                {
                    item.IsActived = false;
                    item.NgaySua = DateTime.Now;
                    item.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                }

                _context.MediaFiles.UpdateRange(Pictures);

                // Thêm mới hình ảnh hoặc cập nhật trạng thái hình ảnh cũ
                var result = new BaseResponse<MODELMediaFile>();
                if (request.IsEdit)
                {
                    result = _mediaFileService.Update(request);
                }
                else
                {
                    result = _mediaFileService.Insert(request);
                }

                if (result.Error)
                {
                    throw new Exception(result.Message);
                }
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<string> DeleteList(DeleteListRequest request)
        {
            var response = new BaseResponse<string>();
            try
            {
                foreach (var id in request.Ids)
                {
                    var delete = _context.Users.Find(id);
                    if (delete != null)
                    {
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        _context.Users.Remove(delete);
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy dữ liệu");
                    }
                }

                _context.SaveChanges();
                response.Data = String.Join(',', request.Ids);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public List<MODELUser> GetByIds(List<Guid> ids)
        {
            return _context.Users
                .Where(x => ids.Contains(x.Id))
                .Select(x => _mapper.Map<MODELUser>(x))
                .ToList();
        }

        public async Task<List<MODELUser>> GetByIdsAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return new List<MODELUser>();

            var users = await _context.Users
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            return users.Select(x => _mapper.Map<MODELUser>(x)).ToList();
        }

        #endregion

        #region Login/Register/Logout
        public BaseResponse<MODELUser> Login(LoginRequest request, string ipAddress)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var data = new MODELUser();
                var user = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
                if (user == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                else
                {
                    if (!user.IsActived)
                    {
                        throw new Exception("Tài khoản đã bị vô hiệu");
                    }

                    var pass = Encrypt_DecryptHelper.EncodePassword(request.Password, user.PasswordSalt);
                    if (!pass.Equals(user.Password))
                    {
                        throw new Exception("Tài khoản hoặc mật khẩu không đúng");
                    }

                    data = _mapper.Map<MODELUser>(user);
                    var token = JWTHelper.GenerateJwtToken(data, _config);
                    // Generate Refresh Token
                    var refreshToken = GenerateRefreshToken(ipAddress);
                    refreshToken.UserId = user.Id;
                    // Save Refresh Token
                    _context.RefreshTokens.Add(refreshToken);
                    _context.SaveChanges();

                    data.RefreshToken = refreshToken.Token.ToString();
                    data.AccessToken = token;

                    response.Data = data;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // CheckUsername Exist
        public BaseResponse<LoginRequest> CheckUsernameExist(UsernameRequest request)
        {
            var response = new BaseResponse<LoginRequest>();
            try
            {
                var data = new LoginRequest();
                var user = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
                if (user == null)
                {
                    throw new Exception("Email hoặc số điện thoại không đúng");
                }
                else
                {
                    data.Username = user.Username;
                }
                response.Data = data;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELUser> Register(RegisterRequest request, string ipAddress)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                if (!CommonFunc.IsValidEmail(request.Username) && !CommonFunc.IsValidPhone(request.Username))
                {
                    throw new Exception("Email hoặc số điện thoại chưa đúng định dạng");
                }

                var checkUsername = _context.Users.Where(x => x.Username.Equals(request.Username));
                if (checkUsername.Any())
                {
                    throw new Exception("Email hoặc số điện thoại đã được sử dụng");
                }

                if (request.Password != request.ConfirmPassword)
                    throw new Exception("Mật khẩu không khớp");

                request.HoLot = request.HoLot.Trim();
                request.Ten = request.Ten.Trim();

                var add = _mapper.Map<ENTITIES.DbContent.User>(request);
                add.Id = Guid.NewGuid();
                add.PasswordSalt = Encrypt_DecryptHelper.GenerateSalt();
                add.Password = Encrypt_DecryptHelper.EncodePassword(request.Password, add.PasswordSalt);
                // Vai trò
                add.RoleId = Guid.Parse("EFF9DA3C-EF59-4131-B427-2D83518C950D");
                add.IsActived = true;

                if (CommonFunc.IsValidEmail(request.Username))
                {
                    add.Email = request.Username;
                }
                else
                {
                    add.SoDienThoai = request.Username;
                }

                add.NguoiTao = "User";
                add.NgayTao = DateTime.Now;
                add.NguoiSua = "User";
                add.NgaySua = DateTime.Now;

                // Lưu dữ liệu
                _context.Users.Add(add);
                _context.SaveChanges();

                // Nếu là đăng nhập bằng Google thì sẽ tự động đăng nhập
                if (add.IsGoogle)
                {
                    // Khi đã cập nhật dữ liệu thành công thì sẽ tự động đăng nhập cho người dùng
                    response = LoginGoogle(new LoginGoogleRequest() { Username = request.Username }, ipAddress);
                }
                else
                {
                    response.Data = _mapper.Map<MODELUser>(add);
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELToken> RefreshToken(string token, string ipAddress)
        {
            var response = new BaseResponse<MODELToken>();
            try
            {
                var user = getUserByRefreshToken(token);
                var refreshToken = _mapper.Map<MODELRefreshToken>(user.RefreshTokens.Single(x => x.Token == token));

                if (refreshToken.IsRevoked)
                {
                    // Thu hồi tất cả các Refresh Token trong trường hợp Token này đã bị thu hồi
                    revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"RefreshToken bị thu hồi đang được sử dụng lại: {token}");
                    _context.SaveChanges();
                }

                if (!refreshToken.IsActive)
                    throw new Exception("RefreshToken không hợp lệ");

                // replace old refresh token with a new one (rotate token)
                var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
                newRefreshToken.UserId = user.Id;

                // save changes to db
                _context.RefreshTokens.Add(newRefreshToken);
                _context.SaveChanges();

                // generate new jwt
                response.Data = new MODELToken
                {
                    AccessToken = JWTHelper.GenerateJwtToken(_mapper.Map<MODELUser>(user), _config),
                    RefreshToken = newRefreshToken.Token
                };
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELUser> Logout(PostLogoutRequest request)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // Login Google
        public BaseResponse<MODELUser> LoginGoogle(LoginGoogleRequest request, string ipAddress)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var data = new MODELUser();
                var user = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
                if (user == null)
                {
                    throw new Exception("AccountNotExist");
                }
                else
                {
                    if (!user.IsActived)
                    {
                        throw new Exception("Tài khoản đã bị vô hiệu");
                    }

                    data = _mapper.Map<MODELUser>(user);
                    var token = JWTHelper.GenerateJwtToken(data, _config);
                    // Generate Refresh Token
                    var refreshToken = GenerateRefreshToken(ipAddress);
                    refreshToken.UserId = user.Id;
                    // Save Refresh Token
                    _context.RefreshTokens.Add(refreshToken);
                    _context.SaveChanges();

                    data.RefreshToken = refreshToken.Token.ToString();
                    data.AccessToken = token;

                    response.Data = data;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region Quên mật khẩu
        public BaseResponse SendOTP(UsernameRequest request)
        {
            var response = new BaseResponse();
            try
            {
                if (CommonFunc.IsValidEmail(request.Username))
                {
                    var OTP = _otpService.Insert(request.Username);
                    if (OTP.Error)
                    {
                        throw new Exception(OTP.Message);
                    }
                    else
                    {
                        SendEmailConfirm(request.Username, OTP.Data.Code);
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<string> VerifyOTP(VerifyOTPRequest request)
        {
            var response = new BaseResponse<string>();
            try
            {
                // Check User
                var checkUser = _context.Users.FirstOrDefault(x => x.Username == request.Username);
                if (checkUser == null)
                {
                    throw new Exception("Mã OTP không hợp lệ");
                }
                request.UserId = checkUser.Id;

                // Verify OTP
                var checkOTP = _otpService.Verify(request).Data;
                if (checkOTP)
                {
                    string Token = JWTHelper.GenerateForgetPasswordToken(new MODELUser { Id = checkUser.Id, Username = checkUser.Username }, _config);
                    response.Data = Token;
                }
                else
                {
                    throw new Exception("Mã OTP không hợp lệ");
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse ChangePassword(ChangePasswordRequest request)
        {
            var response = new BaseResponse();
            try
            {
                // Validate Token
                var principal = JWTHelper.ValidateForgetPasswordToken(request.Token, _config);
                if (principal == null)
                {
                    throw new Exception("Quá thời hạn đổi mật khẩu");
                }

                var UserId = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                var User = _context.Users.Find(Guid.Parse(UserId));
                if (User == null)
                {
                    throw new Exception("Đổi mật khẩu không thành công");
                }

                // Change Password
                User.PasswordSalt = Encrypt_DecryptHelper.GenerateSalt();
                User.Password = Encrypt_DecryptHelper.EncodePassword(request.Password, User.PasswordSalt);
                User.NguoiSua = "User";
                User.NgaySua = DateTime.Now;
                // Lưu dữ liệu
                _context.Users.Update(User);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        #endregion

        #region Private Method
        // Update Image
        private string UploadPicture(string folderUpload, string oldImage, string fileDirectory)
        {
            string path = "";
            string folderUploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Files/Temp/UploadFile/" + folderUpload);
            if (Directory.Exists(folderUploadPath))
            {
                string[] arrFiles = Directory.GetFiles(folderUploadPath);
                string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg" };
                List<string> imgFiles = new List<string>();
                foreach (var file in arrFiles)
                {
                    string fileExtension = Path.GetExtension(file);

                    if (imageExtensions.Contains(fileExtension.ToLower()))
                    {
                        imgFiles.Add(file);
                    }
                }
                if (imgFiles.Count() > 0) //có đính kèm
                {
                    FileInfo info = new FileInfo(imgFiles[0]);
                    string fileName = Guid.NewGuid().ToString() + info.Extension;
                    //string avataPath = Path.Combine(_webHostEnvironment.WebRootPath, "Files/User/ProfilePicture");
                    string avataPath = Path.Combine(_webHostEnvironment.WebRootPath, fileDirectory);
                    //Kiểm tra nếu thư mục chưa tồn tại thì tạo mới.
                    if (!Directory.Exists(avataPath))
                    {
                        Directory.CreateDirectory(avataPath);
                    }

                    //Xóa ảnh cũ nếu tồn tại
                    if (File.Exists(_webHostEnvironment.WebRootPath + "/" + oldImage))
                    {
                        File.Delete(_webHostEnvironment.WebRootPath + "/" + oldImage);
                    }

                    //Copy ảnh mới
                    File.Move(info.FullName, avataPath + "/" + fileName, true);
                    path = fileDirectory + fileName;
                }
            }
            return path;
        }

        // Send Email Confirm
        private async Task SendEmailConfirm(string Email, string OTPCode)
        {
            try
            {
                // Đường dẫn Template
                string templateFullPath = Path.Combine(_webHostEnvironment.WebRootPath, @"Files/Mail/OTPMail.html");
                string messageBody = await File.ReadAllTextAsync(templateFullPath);
                // Replace các giá trị trong template
                messageBody = messageBody.Replace("{0}", OTPCode);

                Task.Run(() => _mailService.SendEmailAsync(new MODELMail
                {
                    ToEmail = Email,
                    Subject = "Quên mật khẩu",
                    Body = messageBody
                }));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Refresh Token
        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(CommonConst.ExpireRefreshToken),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
            return refreshToken;
        }

        private ENTITIES.DbContent.User? getUserByRefreshToken(string token)
        {
            return _context.Users
                .Include(u => u.RefreshTokens)
                .AsNoTracking()
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        }

        private RefreshToken rotateRefreshToken(MODELRefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Đổi RefreshToken mới", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void revokeRefreshToken(MODELRefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;

            var updateToken = _mapper.Map<ENTITIES.DbContent.RefreshToken>(token);
            _context.RefreshTokens.Update(updateToken);
        }

        // Thu hồi tất cả các Refresh Token cũ
        private void revokeDescendantRefreshTokens(MODELRefreshToken refreshToken, ENTITIES.DbContent.User user, string ipAddress, string reason)
        {
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = _mapper.Map<MODELRefreshToken>(user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken));
                if (childToken.IsActive)
                    revokeRefreshToken(childToken, ipAddress, reason);
                else
                    revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }
        #endregion
        #endregion
    }
}
