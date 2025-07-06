using AutoMapper;
using BE.Helpers;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.CONVERSATION.Dtos;
using MODELS.CONVERSATION.Requests;
using MODELS.GROUP.Dtos;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.MESSAGESTATUS.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BE.Services.Conversation
{
    public class CONVERSATIONService : ICONVERSATIONService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUSERService _userService;

        public CONVERSATIONService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IUSERService userService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public async Task<BaseResponse<GetListPagingResponse>> GetListPaging(POSTConversationGetListPagingRequest request)
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
                    new SqlParameter("@iCurrentUserId", request.CurrentUserId),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELConversationGetListPaging>("sp_CONVERSATION_GetListPaging", parameters).ToList();

                foreach (var item in result)
                {
                    // Nếu chưa có hình ảnh đại diện thì lấy hình ảnh của các thành viên trong nhóm
                    if (item.TypeOfConversation == 1 && string.IsNullOrEmpty(item.TargetPicture))
                    {
                        var avatar = GetGroupAvartar(new GetByIdRequest { Id = item.TargetId });
                        if (avatar.Error)
                        {
                            throw new Exception(avatar.Message);
                        }
                        item.Avartar = avatar.Data;
                    }

                    // Lấy ra người dùng hiện tại từ HttpContext
                    var currentUser = (await _userService.GetByIdAsync(new GetByIdRequest { Id = request.CurrentUserId })).Data;

                    // Xử lý nội dung tin nhắn nếu tin nhắn là Welcome hoặc Notification
                    if (item.LatestMessageType == 1 || item.LatestMessageType == 2)
                    {
                        MODELMessageContent content = JsonConvert.DeserializeObject<MODELMessageContent>(item.LatestMessage);

                        // Thay đổi nội dung của tin nhắn theo người dùng hiện tại
                        // Case 1: User hiện tại là người tạo ra message này
                        if (currentUser.Id == content.UserId)
                        {
                            // Nếu ta thêm ai đó vào nhóm
                            if(content.TargetId.Count > 0)
                            {
                                List<string> usernames = new List<string>();
                                foreach (var userid in content.TargetId)
                                {
                                    //var user = _context.Users.Find(userid);
                                    var user = (await _userService.GetByIdAsync(new GetByIdRequest { Id = userid })).Data;
                                    if (user != null)
                                    {
                                        usernames.Add(string.Concat(user.HoLot, " ", user.Ten));
                                    }
                                }

                                // Cập nhật nội dung tin nhắn
                                item.LatestMessage = $"Bạn đã thêm {string.Join(", ", usernames)} vào nhóm";
                            }
                            // Nếu ta tham gia bằng Lời mời (GroupRequest)
                            else
                            {
                                // Cập nhật nội dung tin nhắn
                                item.LatestMessage = $"Bạn đã tham gia nhóm";
                            }
                        }
                        // Case 2: Bạn là 1 trong những người nhận tin nhắn
                        else if (content.TargetId.Contains(currentUser.Id))
                        {
                            //var user = _context.Users.Find(content.UserId);
                            var user = (await _userService.GetByIdAsync(new GetByIdRequest { Id = content.UserId })).Data;
                            if (user != null)
                            {
                                // Cập nhật nội dung tin nhắn
                                item.LatestMessage = $"{string.Concat(user.HoLot, " ", user.Ten)} đã thêm bạn vào nhóm";
                            }
                        }
                        // Case 3: Bạn không phải là người tạo và cũng không phải là người nhận tin nhắn
                        else
                        {
                            if(content.TargetId.Count > 0)
                            {
                                //var user = _context.Users.Find(content.UserId);
                                var user = (await _userService.GetByIdAsync(new GetByIdRequest { Id = content.UserId })).Data;
                                var usernames = new List<string>();
                                foreach (var userid in content.TargetId)
                                {
                                    //var targetUser = _context.Users.Find(userid);
                                    var targetUser = (await _userService.GetByIdAsync(new GetByIdRequest { Id = userid })).Data;
                                    if (targetUser != null)
                                    {
                                        usernames.Add(string.Concat(targetUser.HoLot, " ", targetUser.Ten));
                                    }
                                }

                                if (user != null)
                                {
                                    // Cập nhật nội dung tin nhắn
                                    item.LatestMessage = $"{string.Concat(user.HoLot, " ", user.Ten)} đã thêm {string.Join(", ", usernames)} vào nhóm";
                                }
                            }
                            else
                            {
                                //var user = _context.Users.Find(content.UserId);
                                var user = (await _userService.GetByIdAsync(new GetByIdRequest { Id = content.UserId })).Data;
                                if (user != null)
                                {
                                    // Cập nhật nội dung tin nhắn
                                    item.LatestMessage = $"{string.Concat(user.HoLot, " ", user.Ten)} đã tham gia nhóm";
                                }
                            }
                        }
                    }else if(item.LatestMessageType == 3)
                    {
                        var mediaFile = _context.MediaFiles.FirstOrDefault(x => x.MessageId == item.LatestMessageId);
                        if(mediaFile != null)
                        {
                            item.LatestMessage = $"{item.UserSendLastestMessage} đã gửi một {mediaFile.FileType switch
                            {
                                2 => "hình ảnh",
                                3 => "tệp tin",
                                5 => "video",
                                _ => "tệp tin"
                            }}";
                        }
                        else
                        {
                            item.LatestMessage = $"{item.UserSendLastestMessage} đã gửi một tệp tin";
                        }
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

        public BaseResponse<GetListPagingResponse> SearchUserByEmailOrPhone(POSTSearchInConversationRequest request)
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

                var UserId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "name").Value;

                var parameters = new[]
                {
                    new SqlParameter("@iCurrentId", UserId),
                    new SqlParameter("@iTextSearch", request.TextSearch),
                    new SqlParameter("@iPageIndex", request.PageIndex - 1),
                    new SqlParameter("@iRowsPerPage", request.RowPerPage),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELUser>("sp_USER_UserSearchByEmailOrPhone", parameters).ToList();
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

        public BaseResponse<MODELConversation> Insert(POSTConversationRequest request)
        {
            var response = new BaseResponse<MODELConversation>();
            try
            {
                var check = _context.Conversations.Any(x => x.UserId == request.UserId
                                                                  && x.TargetId == request.TargetId
                                                                  && !x.IsDeleted);

                if (check)
                {
                    throw new Exception("Cuộc trò chuyện đã tồn tại trong hệ thống");
                }

                var add = _mapper.Map<ENTITIES.DbContent.Conversation>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;
                add.NgayTao = DateTime.Now;
                add.NguoiTao = _contextAccessor.HttpContext.User.Identity.Name;
                add.NgaySua = DateTime.Now;
                add.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                _context.Conversations.Add(add);
                if (request.IsSaveChange)
                {
                    _context.SaveChanges();
                }

                response.Data = _mapper.Map<MODELConversation>(add);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELConversation> Update(POSTConversationRequest request)
        {
            var response = new BaseResponse<MODELConversation>();
            try
            {
                var check = _context.Conversations.Any(x => x.UserId == request.UserId
                                                       && x.TargetId == request.TargetId
                                                       && !x.IsDeleted && x.Id != request.Id);

                if (check)
                {
                    throw new Exception("Cuộc trò chuyện đã tồn tại trong hệ thống");
                }

                var update = _context.Conversations.Find(request.Id);
                if (update == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    _mapper.Map(request, update);
                    update.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;
                    update.NgaySua = DateTime.Now;

                    // Lưu dữ liệu
                    _context.Conversations.Update(update);
                    _context.SaveChanges();

                    response.Data = _mapper.Map<MODELConversation>(update);
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
                    var delete = _context.Conversations.Find(id);
                    if (delete != null)
                    {
                        delete.IsDeleted = true;
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        // Lưu dữ liệu
                        _context.Conversations.Update(delete);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"Không tìm thấy dữ liệu: {id.ToString()}");
                    }
                }
                response.Data = string.Join(", ", request.Ids);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        #region Xử lý request từ Websocket
        public BaseResponse<Guid[]> InsertPrivateConversation(WSPrivateMessageInsertConversation request)
        {
            var response = new BaseResponse<Guid[]>();
            try
            {
                var Sender = new ENTITIES.DbContent.Conversation
                {
                    Id = Guid.NewGuid(),
                    UserId = request.Sender.Id,
                    TypeOfConversation = 0,
                    TargetId = request.Receiver.Id,
                    LastReadMessageId = request.MessageId,
                    NgayTao = DateTime.Now,
                    NguoiTao = request.Sender.Username,
                    NgaySua = DateTime.Now,
                    NguoiSua = request.Sender.Username,
                    IsActived = true,
                    IsDeleted = false,
                };

                var Receiver = new ENTITIES.DbContent.Conversation
                {
                    Id = Guid.NewGuid(),
                    UserId = request.Receiver.Id,
                    TypeOfConversation = 0,
                    TargetId = request.Sender.Id,
                    LastReadMessageId = null,
                    NgayTao = DateTime.Now,
                    NguoiTao = request.Receiver.Username,
                    NgaySua = DateTime.Now,
                    NguoiSua = request.Receiver.Username,
                    IsActived = true,
                    IsDeleted = false,
                };

                _context.Conversations.AddRange(Sender, Receiver);
                _context.SaveChanges();

                response.Data = new Guid[] { Sender.Id, Receiver.Id };
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<bool> CheckConversationExist(Guid userId, Guid targetId)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var result = _context.Conversations.Any(x => x.UserId == userId && x.TargetId == targetId && !x.IsDeleted && x.IsActived);
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<Dictionary<Guid, List<Guid>>> GetDictionaryConversationUserToUser()
        {
            var response = new BaseResponse<Dictionary<Guid, List<Guid>>>();
            try
            {
                var result = _context.Conversations
                    .Where(x => !x.IsDeleted && x.IsActived && x.TypeOfConversation == 0)
                    .Select(x => new { x.UserId, x.TargetId })
                    .ToList();

                var dictionary = result
                                .GroupBy(x => x.UserId)
                                .ToDictionary(
                                    group => group.Key,
                                    group => group.Select(x => x.TargetId).ToList()
                                );
                response.Data = dictionary;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<bool> RoolbackDelete(GetByIdRequest request)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var delete = _context.Conversations.Find(request.Id);
                if (delete != null)
                {
                    delete.IsDeleted = true;
                    delete.NguoiXoa = "roolback";
                    delete.NgayXoa = DateTime.Now;
                    // Lưu dữ liệu
                    _context.Conversations.Update(delete);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse> UpdateLatestMessage(Guid UserId, Guid TargetId)
        {
            var response = new BaseResponse();
            try
            {
                var update = _context.Conversations.FirstOrDefault(c => c.UserId == UserId
                                                                   && c.TargetId == TargetId
                                                                   && !c.IsDeleted);
                if (update == null)
                {
                    throw new Exception("Dữ liệu không tồn tại");
                }

                var message = GetLatestMessage(UserId, TargetId);

                if (message == null)
                {
                    throw new Exception("Tin nhắn không tồn tại");
                }

                var User = await _userService.GetByIdAsync(new GetByIdRequest { Id = UserId });


                update.LastReadMessageId = message.Id;
                update.NguoiSua = User.Data.Username;
                update.NgaySua = DateTime.Now;

                _context.Conversations.Update(update);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELGroupAvartar> GetGroupAvartar(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELGroupAvartar>();
            try
            {
                var Data = new MODELGroupAvartar();

                // Lấy ra thông tin thành viên trong nhóm
                var member = _context.GroupMembers.Where(gm => gm.GroupId == request.Id
                                                        && !gm.IsDeleted).ToList();
                if (!member.Any())
                {
                    throw new Exception("Không tìm thấy thông tin nhóm");
                }

                Data.UrlsAvartar = member.Select(mem => _context.MediaFiles
                                                      .FirstOrDefault(
                                                            med => med.OwnerId == mem.UserId
                                                            && med.FileType == (int)MODELS.COMMON.MediaFileType.ProfilePicture
                                                            && !med.IsDeleted && med.IsActived
                                                      ))
                                 .Where(med => med != null)
                                 .Select(med => med.Url)
                                 .Take(4).ToList();

                // Nếu số lượng hình ảnh ít hơn 4 thì cần thêm hình ảnh mặc định
                while (Data.UrlsAvartar.Count < 4 && Data.UrlsAvartar.Count < member.Count)
                {
                    Data.UrlsAvartar.Add(CommonConst.DefaultUrlNoPicture);
                }

                // Lấy ra thông tin số lượng thành viên nhóm
                Data.CountMember = member.Count;

                // Trả về dữ liệu
                response.Data = Data;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        #endregion

        #region Private Function
        MODELMessage GetLatestMessage(Guid UserId, Guid TargetId)
        {
            var conversation = _context.Conversations.FirstOrDefault(c => c.UserId == UserId && c.TargetId == TargetId && !c.IsDeleted);
            ENTITIES.DbContent.Message message;

            if (conversation != null && conversation.TypeOfConversation == 1)
            {
                message = _context.Messages.Where(m => m.TargetId == TargetId && !m.IsDeleted)
                                           .OrderByDescending(m => m.NgayTao)
                                           .FirstOrDefault();
            }
            else
            {
                message = _context.Messages.Where(m => (m.SenderId == UserId || m.SenderId == TargetId)
                                                    && (m.TargetId == UserId || m.TargetId == TargetId)
                                                    && !m.IsDeleted)
                                            .OrderByDescending(m => m.NgayTao)
                                            .FirstOrDefault();
            }
            return _mapper.Map<MODELMessage>(message);
        }
        #endregion
    }
}
