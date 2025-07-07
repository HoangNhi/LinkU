using AutoMapper;
using BE.Helpers;
using BE.Services.Conversation;
using BE.Services.MediaFile;
using BE.Services.Message;
using BE.Services.Redis;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.GROUP.Dtos;
using MODELS.GROUP.Requests;
using MODELS.MEDIAFILE.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BE.Services.Group
{
    public class GROUPService : IGROUPService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMEDIAFILEService _mediaService;
        private readonly ICONVERSATIONService _conversationService;
        private readonly IMESSAGEService _messageService;
        private readonly IUSERService _userService;
        private readonly IREDISService _redisService;

        public GROUPService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IMEDIAFILEService mediaService, ICONVERSATIONService conversationService, IMESSAGEService messageService, IUSERService userService, IREDISService redisService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _mediaService = mediaService;
            _conversationService = conversationService;
            _messageService = messageService;
            _userService = userService;
            _redisService = redisService;
        }

        #region Common CRUD
        public BaseResponse<GetListPagingResponse> GetListPaging(POSTGroupGetListPagingRequest request)
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
                    new SqlParameter("@iUserId", request.UserId),
                    iTotalRow
                };

                // Lấy dữ liệu
                var result = _context.ExcuteStoredProcedure<MODELGroup>("sp_GROUP_GetListPaging", parameters).ToList();

                // Xử lý dữ liệu
                foreach (var item in result)
                {
                    // Lấy hình ảnh đại diện của nhóm
                    if (string.IsNullOrEmpty(item.AvartarUrl))
                    {
                        var avatar = _conversationService.GetGroupAvartar(new GetByIdRequest { Id = item.Id });
                        if (avatar.Error)
                        {
                            throw new Exception(avatar.Message);
                        }
                        item.Avartar = avatar.Data;
                    }
                }

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

        public async Task<BaseResponse<MODELGroup>> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                // Tìm kiếm nhóm
                var group = _context.Groups.Where(g => g.Id == request.Id && !g.IsDeleted)
                                           .Include(g => g.GroupMembers.Where(gm => !gm.IsDeleted))
                                           .FirstOrDefault();
                if (group == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                // Gán kết quả
                response.Data = _mapper.Map<MODELGroup>(group);

                // Chèn tên người tạo
                var CreateBy = _context.Users.FirstOrDefault(u => u.Username == group.NguoiTao && !u.IsDeleted);
                response.Data.NguoiTao = CreateBy != null ? string.Concat(CreateBy.HoLot, " ", CreateBy.Ten) : group.NguoiTao;

                // Lấy hình ảnh đại diện của nhóm
                response.Data.AvartarUrl = _context.MediaFiles.FirstOrDefault(m => m.GroupId == group.Id
                                            && m.FileType == (int)MODELS.COMMON.MediaFileType.GroupAvatar
                                            && !m.IsDeleted && m.IsActived)?.Url;

                // Chuyển đổi danh sách thành viên nhóm sang DTO
                foreach (var member in group.GroupMembers)
                {
                    var user = await _userService.GetByIdAsync(new GetByIdRequest { Id = member.UserId });
                    if (user.Error)
                    {
                        throw new Exception(user.Message);
                    }

                    response.Data.GroupMembers.FirstOrDefault(gm => gm.Id == member.Id).User = user.Data;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<POSTGroupRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<POSTGroupRequest>();
            try
            {
                var group = _context.Groups.FirstOrDefault(g => g.Id == request.Id && !g.IsDeleted);
                if (group == null)
                {
                    var newGroup = new POSTGroupRequest
                    {
                        Id = Guid.NewGuid(),
                        IsEdit = false
                    };
                    response.Data = newGroup;
                }
                else
                {
                    response.Data = _mapper.Map<POSTGroupRequest>(group);
                    response.Data.IsEdit = true;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroup> Insert(POSTGroupRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                var add = _mapper.Map<ENTITIES.DbContent.Group>(request);

                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.Groups.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroup>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroup> Update(POSTGroupRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                var update = _context.Groups.FirstOrDefault(g => g.Id == request.Id);

                if (update == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                _mapper.Map(request, update);

                update.NgaySua = DateTime.Now;
                update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.Groups.Update(update);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroup>(update);
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
                    var delete = _context.Groups.Find(id);
                    if (delete != null)
                    {
                        delete.IsDeleted = true;
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        // Lưu dữ liệu
                        _context.Groups.Update(delete);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"Không tìm thấy dữ liệu: {id.ToString()}");
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
        #endregion

        #region Advanced
        public async Task<BaseResponse<MODELGroup>> CreateGroupWithMember(POSTCreateGroupRequest request)
        {
            var response = new BaseResponse<MODELGroup>();
            try
            {
                // Validate request
                var validate = ValidateRequestCreateGroupWithMember(request);
                if (validate.Error)
                {
                    throw new Exception(validate.Message);
                }

                // Tạo nhóm
                var group = new ENTITIES.DbContent.Group
                {
                    Id = Guid.NewGuid(),
                    GroupName = request.GroupName,
                    GroupType = request.GroupType,
                    NgayTao = DateTime.Now,
                    NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                    NgaySua = DateTime.Now,
                    NguoiSua = _contextAccessor.HttpContext.User.Identity.Name
                };
                _context.Groups.Add(group);

                // Thêm members vào nhóm
                var GroupMembers = request.Members.Select(m => new ENTITIES.DbContent.GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    UserId = m.UserId,
                    Role = m.Role,
                    NgayTao = DateTime.Now,
                    NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                    NgaySua = DateTime.Now,
                    NguoiSua = _contextAccessor.HttpContext.User.Identity.Name
                });
                _context.GroupMembers.AddRange(GroupMembers);

                // Nếu có ảnh đại diện thì thêm vào nhóm
                if (request.Avatar != null)
                {
                    var createMedia = await _mediaService.CreatePicture(
                        new POSTCreatePictureRequest
                        {
                            File = request.Avatar,
                            GroupId = group.Id,
                            FileType = MediaFileType.GroupAvatar,
                            IsSaveChange = false,
                        }
                    );

                    if (createMedia.Error)
                    {
                        throw new Exception(createMedia.Message);
                    }
                }

                // Tạo tin nhắn chào mừng
                var UserId = _contextAccessor.GetClaim("name");
                var messagecontent = new MODELMessageContent
                {
                    UserId = Guid.Parse(UserId),
                    TargetId = request.Members.Select(m => m.UserId)
                                      .Where(id => id != Guid.Parse(UserId)).ToList(),
                };

                if (string.IsNullOrEmpty(UserId))
                {
                    throw new Exception("Lỗi trong lúc tạo nhóm");
                }

                var message = await _messageService.Insert(new PostMessageRequest
                {
                    Content = JsonConvert.SerializeObject(messagecontent),
                    SenderId = Guid.Parse(UserId),
                    TargetId = group.Id,
                    MessageType = (int)MessageType.Welcome,
                    IsSaveChange = false
                });

                if (message.Error)
                {
                    throw new Exception(message.Message);
                }

                foreach (var member in request.Members)
                {
                    // Tạo cuộc trò chuyện cho nhóm
                    var conversation = _conversationService.Insert(new MODELS.MESSAGESTATUS.Requests.POSTConversationRequest
                    {
                        UserId = member.UserId,
                        TargetId = group.Id,
                        TypeOfConversation = 1,
                        LastReadMessageId = member.Role == 1 ? null : message.Data.Id,
                        IsSaveChange = false
                    });

                    if (conversation.Error)
                    {
                        throw new Exception(conversation.Message);
                    }
                }

                // Lưu dữ liệu
                _context.SaveChanges();
                response.Data = _mapper.Map<MODELGroup>(group);

                // Lưu vào redis
                await Task.WhenAll(
                    _redisService.SetAsync(
                        RedisKeyHelper.GroupMemberByGroupId(group.Id),
                        JsonConvert.SerializeObject(GroupMembers),
                        TimeSpan.FromMinutes(CommonConst.ExpireRedisGroupMember)
                    ),
                    _redisService.SetAsync(
                        RedisKeyHelper.MessageById(group.Id),
                        JsonConvert.SerializeObject(request.Members.Select(m => m.UserId).ToList()),
                        TimeSpan.FromMinutes(CommonConst.ExpireRedisMessage)
                    )
                );
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<List<MODELMemberCreateGroup>> GetListMemberCreateGroup()
        {
            var response = new BaseResponse<List<MODELMemberCreateGroup>>();
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@iUserId", _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "name").Value)
                };

                // Lấy dữ liệu
                var Data = _context.ExcuteStoredProcedure<MODELMemberCreateGroup>("sp_GROUP_GetListMemberCreateGroup", parameters).ToList();

                response.Data = Data;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<GetListPagingResponse> GetListSuggestMember(POSTGetListSuggestMemberRequest request)
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
                    new SqlParameter("@iUserId", request.UserId),
                    new SqlParameter("@iGroupId", request.GroupId),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELUser>("sp_GROUP_GetListSuggestMember", parameters).ToList();

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
        #endregion

        #region Private Methods
        BaseResponse ValidateRequestCreateGroupWithMember(POSTCreateGroupRequest request)
        {
            var response = new BaseResponse();
            try
            {
                if (request.Members.Count > MODELS.COMMON.CommonConst.MaxGroupMember + 1)
                {
                    throw new Exception("Số lượng thành viên phải nhỏ hơn 11");
                }

                if (request.Members.Count < 3)
                {
                    throw new Exception("Số lượng thành viên phải lớn hơn hoặc bằng 3");
                }

                // Kiểm tra User có tồn tại không
                var UserId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                // Kiểm tra từng User
                foreach (var member in request.Members)
                {
                    var userExist = _context.Users.FirstOrDefault(u => u.Id == member.UserId && !u.IsDeleted);
                    if (userExist == null)
                    {
                        throw new Exception($"Người dùng không tồn tại: {member.UserId}");
                    }
                    else
                    {
                        if (!member.UserId.ToString().Equals(UserId))
                        {
                            // Kiểm tra xem người dùng có phải là bạn bè không
                            var friendExist = _context.Friendships.FirstOrDefault(
                                f => (
                                        (f.UserId1 == Guid.Parse(UserId) && f.UserId2 == member.UserId)
                                        || (f.UserId1 == member.UserId && f.UserId2 == Guid.Parse(UserId))
                                    )
                                && !f.IsDeleted);

                            if (friendExist == null)
                            {
                                throw new Exception($"Người dùng {userExist.Ten} không phải là bạn bè của bạn");
                            }
                        }
                    }
                }

                // Nhóm phải có ít nhất 1 Admin
                var IsOneAdmin = request.Members.Count(m => m.Role == 2);
                switch (IsOneAdmin)
                {
                    case 0:
                        throw new Exception("Nhóm phải có ít nhất 1 Admin");
                    case 1:
                        break;
                    default:
                        throw new Exception("Nhóm chỉ được có 1 Admin");
                }

                // Kiểm tra hình ảnh
                if (request.Avatar != null)
                {
                    // 1. Giới hạn kích thước (ví dụ: 2MB)
                    const long maxFileSize = 2 * 1024 * 1024; // 2MB
                    if (request.Avatar.Length > maxFileSize)
                        throw new Exception("File vượt quá dung lượng cho phép (2MB).");

                    // 2. Kiểm tra định dạng file (chỉ cho phép ảnh và PDF)
                    if (!CommonConst.AllowedPictureTypes.Contains(request.Avatar.ContentType.ToLower()))
                        throw new Exception("Định dạng file không được hỗ trợ. Chỉ cho phép .jpg, .jpeg, .jpe, .jfif và .png");
                }

                response.Error = false;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion
    }
}
