using AutoMapper;
using BE.Helpers;
using BE.Services.Conversation;
using BE.Services.Group;
using BE.Services.Message;
using BE.Services.Redis;
using BE.Services.User;
using ENTITIES.DbContent;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.GROUPMEMBER.Dtos;
using MODELS.GROUPMEMBER.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using Newtonsoft.Json;

namespace BE.Services.GroupMember
{
    public class GROUPMEMBERService : IGROUPMEMBERService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGROUPService _groupService;
        private readonly IMESSAGEService _messageService;
        private readonly ICONVERSATIONService _conversationService;
        private readonly IUSERService _userService;
        private readonly IREDISService _redisService;

        public GROUPMEMBERService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IGROUPService groupService, IMESSAGEService messageService, ICONVERSATIONService conversationService, IUSERService userService, IREDISService redisService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _groupService = groupService;
            _messageService = messageService;
            _conversationService = conversationService;
            _userService = userService;
            _redisService = redisService;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(GetListPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public BaseResponse<MODELGroupMember> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELGroupMember>();
            try
            {
                var group = _context.GroupMembers.FirstOrDefault(gm => gm.Id == request.Id && !gm.IsDeleted);
                if (group == null)
                {
                    throw new Exception("Thành viên không tồn tại");
                }
                response.Data = _mapper.Map<MODELGroupMember>(group);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<POSTGroupMemberRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<POSTGroupMemberRequest>();
            try
            {
                var group = _context.GroupMembers.FirstOrDefault(gm => gm.Id == request.Id && !gm.IsDeleted);
                if (group == null)
                {
                    var newGroupMember = new POSTGroupMemberRequest
                    {
                        Id = Guid.NewGuid(),
                        IsEdit = false
                    };
                    response.Data = newGroupMember;
                }
                else
                {
                    response.Data = _mapper.Map<POSTGroupMemberRequest>(group);
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

        public BaseResponse<MODELGroupMember> Insert(POSTGroupMemberRequest request)
        {
            var response = new BaseResponse<MODELGroupMember>();
            try
            {
                // Kiểm tra nhóm có tồn tại không
                var group = _context.Groups.Find(request.GroupId);
                if (group == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                // Kiểm tra người dùng có tồn tại trong nhóm không
                var checkExistUserInGroup = _context.GroupMembers.Any(
                    gm => gm.GroupId == request.GroupId
                    && gm.UserId == request.UserId
                    && !gm.IsDeleted
                );

                if (checkExistUserInGroup)
                {
                    throw new Exception("Người dùng đã tồn tại trong nhóm này");
                }

                // Thêm dữ liệu vào bảng GroupMembers
                var add = _mapper.Map<ENTITIES.DbContent.GroupMember>(request);

                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.GroupMembers.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroupMember>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroupMember> Update(POSTGroupMemberRequest request)
        {
            var response = new BaseResponse<MODELGroupMember>();
            try
            {
                var update = _context.GroupMembers.FirstOrDefault(gm => gm.Id == request.Id);

                if (update == null)
                {
                    throw new Exception("Nhóm không tồn tại");
                }

                _mapper.Map(request, update);

                update.NgaySua = DateTime.Now;
                update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.GroupMembers.Update(update);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELGroupMember>(update);
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
                    var delete = _context.GroupMembers.Find(id);
                    if (delete != null)
                    {
                        delete.IsDeleted = true;
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        // Lưu dữ liệu
                        _context.GroupMembers.Update(delete);
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

        public async Task<BaseResponse<MODELResponseAddMemberToGroup>> AddMemberToGroup(POSTAddMemberToGroupRequest request)
        {
            var response = new BaseResponse<MODELResponseAddMemberToGroup>();
            try
            {
                var result = new MODELResponseAddMemberToGroup();
                var UserId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var AddedGroupMember = new List<MODELGroupMember>();
                MODELMessageContent content = new MODELMessageContent();
                content.UserId = Guid.Parse(UserId);

                var checkGroupExist = await _groupService.GetById(new GetByIdRequest { Id = request.GroupId });
                if (checkGroupExist.Error)
                {
                    throw new Exception(checkGroupExist.Message);
                }

                if (request.UserIds.Count == 0)
                {
                    throw new Exception("Vui lòng chọn ít nhất 1 người dùng để thêm vào nhóm");
                }

                // Lấy ra tin nhắn mới nhất của nhóm
                var latestMessage = _context.Messages.Where(x => x.TargetId == request.GroupId)
                                                             .OrderByDescending(x => x.NgayTao)
                                                             .FirstOrDefault();

                foreach (var item in request.UserIds)
                {
                    var checkExistUserInGroup = _context.GroupMembers.Any(
                        gm => gm.GroupId == request.GroupId
                        && gm.UserId == item
                        && !gm.IsDeleted
                    );

                    if (checkExistUserInGroup)
                    {
                        throw new Exception($"Người dùng đã tồn tại trong nhóm này");
                    }

                    var user = (await _userService.GetByIdAsync(new GetByIdRequest { Id = item })).Data;
                    if (user != null)
                    {
                        // Kiểm tra xem người dùng có phải là bạn bè không
                        // Nếu là bạn bè thì thêm vào nhóm, nếu không thì gửi lời mời tham gia nhóm
                        var friendExist = _context.Friendships.FirstOrDefault(
                            f => (
                                    (f.UserId1 == Guid.Parse(UserId) && f.UserId2 == item)
                                    || (f.UserId1 == item && f.UserId2 == Guid.Parse(UserId))
                                )
                            && !f.IsDeleted);

                        // Không phải là bạn thì gửi lời mời vào nhóm
                        if (friendExist == null)
                        {
                            // Kiểm tra người dùng này đã gửi lời mời trước đó chưa
                            var checkGroupRequest = _context.GroupRequests.FirstOrDefault(x => x.SenderId == Guid.Parse(UserId)
                                                                                         && x.ReceiverId == item
                                                                                         && x.GroupId == request.GroupId
                                                                                         && !x.IsDeleted
                                                                                         && x.State == 0);
                            // Case 1: Chưa có lời mời thì sẽ tạo lời mời
                            if (checkGroupRequest == null)
                            {
                                var invitation = new ENTITIES.DbContent.GroupRequest
                                {
                                    Id = Guid.NewGuid(),
                                    GroupId = request.GroupId,
                                    SenderId = Guid.Parse(UserId),
                                    ReceiverId = item,
                                    State = 0, // 0 - Đang chờ
                                    NgayTao = DateTime.Now,
                                    NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                                    NgaySua = DateTime.Now,
                                    NguoiSua = _contextAccessor.HttpContext.User.Identity.Name,
                                };

                                _context.GroupRequests.Add(invitation);
                            }
                            // Case 2: Nếu có lời mời thì sẽ update lại thời gian gửi lời mời
                            else
                            {
                                checkGroupRequest.NgaySua = DateTime.Now;
                                checkGroupRequest.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                                _context.GroupRequests.Update(checkGroupRequest);
                            }

                            result.InvitedUserIds.Add(item);
                        }
                        else
                        {
                            content.TargetId.Add(user.Id);
                            // Thêm dữ liệu vào bảng GroupMembers
                            var add = new ENTITIES.DbContent.GroupMember
                            {
                                Id = Guid.NewGuid(),
                                GroupId = request.GroupId,
                                UserId = item,
                                NgayTao = DateTime.Now,
                                NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                                NgaySua = DateTime.Now,
                                NguoiSua = _contextAccessor.HttpContext.User.Identity.Name
                            };

                            _context.GroupMembers.Add(add);

                            // Tạo Conversation
                            var conversation = _conversationService.Insert(new MODELS.MESSAGESTATUS.Requests.POSTConversationRequest
                            {
                                UserId = item,
                                TargetId = request.GroupId,
                                TypeOfConversation = 1,
                                LastReadMessageId = latestMessage == null ? null : latestMessage.Id,
                                IsSaveChange = false
                            });

                            AddedGroupMember.Add(_mapper.Map<MODELGroupMember>(add));
                            result.NewMemberIds.Add(item);
                        }
                    }
                    else
                    {
                        throw new Exception("Người dùng không tồn tại");
                    }
                }

                // Tạo tin nhắn thông báo
                if (content.TargetId.Count > 0)
                {
                    string textMessage = JsonConvert.SerializeObject(content);

                    var message = await _messageService.Insert(new PostMessageRequest
                    {
                        Content = textMessage,
                        SenderId = Guid.Parse(_contextAccessor.GetClaim("name")),
                        TargetId = request.GroupId,
                        MessageType = (int)MessageType.Notification,
                        IsSaveChange = false
                    });

                    if (message.Error)
                    {
                        throw new Exception(message.Message);
                    }
                }

                _context.SaveChanges();
                if(AddedGroupMember.Count > 0)
                {
                    var value = await _redisService.GetAsync<List<MODELGroupMember>>(RedisKeyHelper.GroupMemberByGroupId(request.GroupId));
                    if (value != null)
                    {
                        await _redisService.SetAsync(
                            RedisKeyHelper.GroupMemberByGroupId(request.GroupId),
                            JsonConvert.SerializeObject(value.Concat(AddedGroupMember).ToList()),
                            TimeSpan.FromMinutes(CommonConst.ExpireRedisGroupMember)
                        );
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

        public async Task<BaseResponse<List<MODELGroupMember>>> GetListByGroupId(GetByIdRequest request)
        {
            var response = new BaseResponse<List<MODELGroupMember>>();
            try
            {
                // Kiểm tra redis có dữ liệu không
                var value = await _redisService.GetAsync<List<MODELGroupMember>>(RedisKeyHelper.GroupMemberByGroupId(request.Id.Value));
                if (value == null)
                {
                    var groupMembers = _context.GroupMembers
                    .Where(gm => gm.GroupId == request.Id && !gm.IsDeleted)
                    .ToList();

                    if (groupMembers.Count == 0)
                    {
                        throw new Exception("Không có thành viên nào trong nhóm này");
                    }
                    response.Data = _mapper.Map<List<MODELGroupMember>>(groupMembers);

                    // Lưu vào redis
                    _redisService.SetAsync(
                        RedisKeyHelper.GroupMemberByGroupId(request.Id.Value),
                        JsonConvert.SerializeObject(response.Data),
                        TimeSpan.FromMinutes(CommonConst.ExpireRedisGroupMember)
                    );
                }
                else
                {
                    response.Data = value;
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
