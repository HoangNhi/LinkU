using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.REFRESHTOKEN.Dtos;
using MODELS.REFRESHTOKEN.Requests;
using MODELS.USER.Dtos;
using MODELS.USER.Requests;

namespace BE.Services.User
{
    public class USERService : IUSERService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        private readonly IConfiguration _config;

        public USERService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

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
                GetListPagingResponse resposeData = new GetListPagingResponse();
                resposeData.PageIndex = request.PageIndex;
                resposeData.Data = result;
                resposeData.TotalRow = Convert.ToInt32(iTotalRow.Value);
                response.Data = resposeData;
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
                var result = new MODELUser();
                var data = _context.Users.FindAsync(request.Id);
                if(data == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    result = _mapper.Map<MODELUser>(data);
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
        public BaseResponse<PostUserRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<PostUserRequest>();
            try
            {
                var result = new PostUserRequest();
                var data = _context.Users.FindAsync(request.Id);
                if (data == null)
                {
                    result.IsEdit = false;
                }
                else
                {
                    result = _mapper.Map<PostUserRequest>(data);
                    result.Password = "Abc@123";
                    result.IsEdit = true;
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
                add.ProfilePicture = UploadPicture(request.FolderUpload, "", "Files/User/ProfilePicture");
                add.CoverPicture = UploadPicture(request.FolderUploadCoverPicture, "", "Files/User/CoverPicture");
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

        public BaseResponse<MODELUser> Update(PostUserRequest request)
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

                var update = _context.Users.Find(request.Id);
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    _mapper.Map(request, update);
                    if (request.Password != "Abc@123")
                    {
                        update.Password = Encrypt_DecryptHelper.EncodePassword(request.Password, update.PasswordSalt);
                    }

                    update.ProfilePicture = UploadPicture(request.FolderUpload, update.ProfilePicture, "Files/User/ProfilePicture");
                    update.CoverPicture = UploadPicture(request.FolderUploadCoverPicture, update.CoverPicture, "Files/User/CoverPicture");
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                    update.NgaySua = DateTime.Now;

                    // Lưu dữ liệu
                    _context.Users.Update(update);
                    _context.SaveChanges();

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

        public BaseResponse<MODELUser> Login(LoginRequest request)
        {
            var response = new BaseResponse<MODELUser>();
            try
            {
                var data = new MODELUser();
                var user = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
                if(user == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                else
                {
                    if(!user.IsActived)
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
                    var refreshToken = new RefreshToken()
                    {
                        UserId = user.Id,
                        RefreshToken1 = Guid.NewGuid(),
                        ExpiryDate = DateTime.Now.AddHours(CommonConst.ExpireRefreshToken),
                        ExpiryDateAccessTokenRecent = DateTime.Now.AddHours(CommonConst.ExpireAccessToken),
                        IsActived = true,
                        IsDeleted = false,
                    };

                    // Save Refresh Token
                    _context.RefreshTokens.Add(refreshToken);
                    _context.SaveChanges();

                    data.RefreshToken = refreshToken.RefreshToken1.ToString();
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

        public BaseResponse<MODELUser> Register(RegisterRequest request)
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

                add.NguoiTao = "admin";
                add.NgayTao = DateTime.Now;
                add.NguoiSua = "admin";
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

        public BaseResponse<MODELRefreshToken> RefreshToken(PostRefreshTokenRequest request)
        {
            var response = new BaseResponse<MODELRefreshToken>();
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

        // CheckUsername Exist
        public BaseResponse<LoginRequest> CheckUsernameExist(UsernameRequest request)
        {
            var response = new BaseResponse<LoginRequest>();
            try
            {
                var data = new LoginRequest();
                var user = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
                if(user == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
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
    }
}
