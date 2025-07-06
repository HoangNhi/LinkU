using AutoMapper;
using BE.Helpers;
using BE.Services.FriendShip;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.FRIENDREQUEST.Dtos;
using MODELS.FRIENDREQUEST.Requests;
using MODELS.FRIENDSHIP.Requests;
using MODELS.USER.Dtos;
using System.Threading.Tasks;

namespace BE.Services.FriendRequest
{
    public class FRIENDREQUESTService : IFRIENDREQUESTService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFRIENDSHIPService _friendshipService;
        private readonly IUSERService _userService;

        public FRIENDREQUESTService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IFRIENDSHIPService friendshipService, IUSERService userService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _friendshipService = friendshipService;
            _userService = userService;
        }

        public async Task<BaseResponse<GetListPagingResponse>> GetListPaging(POSTFriendRequestGetListPagingRequest request)
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
                foreach (var x in result)
                {
                    var user = await _userService.GetByIdAsync(new GetByIdRequest { Id = request.IsSend ? x.ReceiverId : x.SenderId });
                    if (user.Error)
                    {
                        throw new Exception(user.Message);
                    }
                    x.User = user.Data;
                }

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
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<BaseResponse<POSTFriendRequest>> GetByPost(GetByIdRequest request)
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
                    var user = await _userService.GetByIdAsync(new GetByIdRequest { Id = result.ReceiverId });
                    if (user.Error)
                    {
                        throw new Exception(user.Message);
                    }
                    response.Data.User = user.Data;
                }
                else
                {
                    var user = await _userService.GetByIdAsync(new GetByIdRequest { Id = result.SenderId });
                    if (user.Error)
                    {
                        throw new Exception(user.Message);
                    }
                    response.Data.User = user.Data;
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
                var checkExist = _context.FriendRequests.Any(x => ((x.SenderId == request.SenderId
                                                            && x.ReceiverId == request.ReceiverId)
                                                            || (x.SenderId == request.ReceiverId
                                                            && x.ReceiverId == request.SenderId))
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
                    update.Status = request.Status;
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                    update.NgaySua = DateTime.Now;

                    // Cập nhật dữ liệu
                    _context.FriendRequests.Update(update);

                    // Tạo dữ liệu FriendShip
                    if (update.Status == 1)
                    {
                        var result = _friendshipService.Insert(new POSTFriendshipRequest
                        {
                            Id = Guid.NewGuid(),
                            UserId1 = update.SenderId,
                            UserId2 = update.ReceiverId,
                        });

                        if (result.Error)
                        {
                            throw new Exception(result.Message);
                        }
                    }
                    // Lưu dữ liệu
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

        /// <summary>
        /// Kiểm tra trạng thái kết bạn
        /// </summary>
        /// <param name="request">Id người cần kiểm tra</param>
        /// <returns></returns>
        public BaseResponse<MODELFriendStatus> GetFriendRequestStatus(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELFriendStatus>();
            try
            {
                // UserId từ token
                var UserId = Guid.Parse(_contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "name").Value);
                // Kiểm tra đã kết bạn hay chưa
                var checkFriendShip = _context.Friendships
                    .Any(x => ((x.UserId1 == UserId && x.UserId2 == request.Id)
                            || (x.UserId1 == request.Id && x.UserId2 == UserId))
                            && !x.IsDeleted && x.IsActived);
                if (checkFriendShip)
                {
                    response.Data = new MODELFriendStatus()
                    {
                        IsFriend = true,
                    };
                }
                else
                {
                    var result = new MODELFriendStatus()
                    {
                        IsFriend = false,
                    };
                    // Kiểm tra đã gửi yêu cầu kết bạn hay chưa
                    var checkSendRequest = _context.FriendRequests
                                           .FirstOrDefault(x => ((x.SenderId == UserId && x.ReceiverId == request.Id)
                                           || (x.SenderId == request.Id && x.ReceiverId == UserId))
                                           && !x.IsDeleted && x.Status == 0);
                    if (checkSendRequest != null)
                    {
                        result.Id = checkSendRequest.Id;
                        result.IsSentRequest = true;
                        result.IsMyRequest = checkSendRequest.SenderId == UserId;
                    }
                    else
                    {
                        result.IsSentRequest = false;
                    }
                    response.Data = result;
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
