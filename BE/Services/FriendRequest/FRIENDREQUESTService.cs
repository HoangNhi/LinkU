using AutoMapper;
using BE.Helpers;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.USER.Dtos;

namespace BE.Services.FriendRequest
{
    public class FRIENDREQUESTService : IFRIENDREQUESTService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public FRIENDREQUESTService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(POSTFriendRequestGetListPagingRequest request)
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

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@iUserId", request.UserId),
                    new SqlParameter("@iIsSend", request.IsSend),
                    new SqlParameter("@iTextSearch", request.TextSearch),
                    new SqlParameter("@iPageIndex", request.PageIndex - 1),
                    new SqlParameter("@iRowsPerPage", request.RowPerPage),
                    iTotalRow
                };

                // Lấy dữ liệu
                var result = _context.ExcuteStoredProcedure<MODELFriendRequest>("sp_FRIENDREQUEST_GetListPaging", parameters).ToList();
                // Lấy thông tin user
                result.ForEach(x =>
                {
                    x.User = _mapper.Map<MODELUser>(_context.Users.Find(request.IsSend ? x.ReceiverId : x.SenderId));
                    x.User.Password = null;
                    x.User.PasswordSalt = null;
                });

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

        public BaseResponse<MODELFriendRequest> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELFriendRequest>();
            try
            {
                var result = _context.FriendRequests.Find(request.Id);
                if (result == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    response.Data = _mapper.Map<MODELFriendRequest>(result);
                    //var test = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "name").Value;
                    //if (result.SenderId == Guid.Parse(test))
                    //{
                    //    response.Data.User = _mapper.Map<MODELUser>(_context.Users.Find(result.ReceiverId));
                    //}
                    //else
                    //{
                    //    response.Data.User = _mapper.Map<MODELUser>(_context.Users.Find(result.SenderId));
                    //}
                    // Ẩn mật khẩu
                    response.Data.User.Password = null;
                    response.Data.User.PasswordSalt = null;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public BaseResponse<POSTFriendRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<POSTFriendRequest>();
            try
            {
                var result = new POSTFriendRequest();
                var data = _context.FriendRequests.Find(request.Id);
                if (data == null)
                {
                    result.Id = Guid.NewGuid();
                    result.IsEdit = false;
                }
                else
                {
                    result = _mapper.Map<POSTFriendRequest>(data);
                    result.IsEdit = true;
                }

                response.Data = result;

                // Lấy thông tin userid từ token
                var UserId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "name").Value;
                if (result.SenderId == Guid.Parse(UserId))
                {
                    response.Data.User = _mapper.Map<MODELUser>(_context.Users.Find(result.ReceiverId));
                }
                else
                {
                    var Sender = _context.Users.Find(result.SenderId);
                    response.Data.User = _mapper.Map<MODELUser>(Sender);
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELFriendRequest> Insert(POSTFriendRequest request)
        {
            var response = new BaseResponse<MODELFriendRequest>();
            try
            {
                var checkExist = _context.FriendRequests.Any(x => (x.SenderId == request.SenderId
                                                            && x.ReceiverId == request.ReceiverId)
                                                            || (x.SenderId == request.ReceiverId
                                                            && x.ReceiverId == request.SenderId)
                                                            && x.Status == 0
                                                            && !x.IsDeleted);
                if (checkExist)
                {
                    throw new Exception("Dữ liệu đã tồn tại: " + String.Join(',', checkExist));
                }

                var add = _mapper.Map<ENTITIES.DbContent.FriendRequest>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                // Mặc định trạng thái là chưa xác nhận
                add.Status = 0;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                // Lưu dữ liệu
                _context.FriendRequests.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELFriendRequest>(add);

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // Chấp nhận hoặc từ chối yêu cầu kết bạn
        public BaseResponse<MODELFriendRequest> Update(POSTFriendRequest request)
        {
            var response = new BaseResponse<MODELFriendRequest>();
            try
            {
                var checkExist = _context.FriendRequests.Any(x => (x.SenderId == request.SenderId
                                                            && x.ReceiverId == request.ReceiverId)
                                                            || (x.SenderId == request.ReceiverId
                                                            && x.ReceiverId == request.SenderId)
                                                            && x.Status == 0
                                                            && !x.IsDeleted
                                                            && x.Id != request.Id);
                if (checkExist)
                {
                    throw new Exception("Dữ liệu đã tồn tại");
                }

                var update = _context.FriendRequests.Find(request.Id);
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    _mapper.Map(request, update);
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                    update.NgaySua = DateTime.Now;

                    // Cập nhật dữ liệu
                    _context.FriendRequests.Update(update);
                    _context.SaveChanges();

                    response.Data = _mapper.Map<MODELFriendRequest>(update);
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        // Thu hồi yêu cầu kết bạn
        public BaseResponse<string> Delete(GetByIdRequest request)
        {
            var response = new BaseResponse<string>();
            try
            {
                var delete = _context.FriendRequests.Find(request.Id);
                if (delete == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    delete.IsDeleted = true;
                    delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                    delete.NgayXoa = DateTime.Now;
                    // Xóa dữ liệu
                    _context.FriendRequests.Update(delete);
                    _context.SaveChanges();
                    response.Data = "Xóa dữ liệu thành công";
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
