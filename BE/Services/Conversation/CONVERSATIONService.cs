using AutoMapper;
using BE.Helpers;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.CONVERSATION.Dtos;
using MODELS.CONVERSATION.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGELIST.Dtos;
using MODELS.MESSAGESTATUS.Dtos;
using MODELS.MESSAGESTATUS.Requests;
using MODELS.USER.Dtos;

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

        public BaseResponse<GetListPagingResponse> GetListPaging(POSTConversationGetListPagingRequest request)
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
                _context.SaveChanges();

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
        public BaseResponse<MODELConversation> InsertPrivateConversation(WSPrivateMessageInsertConversation request)
        {
            var response = new BaseResponse<MODELConversation>();
            try
            {
                var add = new ENTITIES.DbContent.Conversation
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

                _context.Conversations.Add(add);
                _context.SaveChanges();

                response.Data = _mapper.Map<MODELConversation>(add);
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
        
        public BaseResponse UpdateLatestMessage(Guid UserId, Guid TargetId)
        {
            var response = new BaseResponse();
            try
            {
                var update = _context.Conversations.FirstOrDefault(c => c.UserId == UserId
                                                                   && c.TargetId == TargetId
                                                                   && !c.IsDeleted);
                if(update == null)
                {
                    throw new Exception("Dữ liệu không tồn tại");
                }

                var message = GetLatestMessage(UserId, TargetId);

                if(message == null)
                {
                    throw new Exception("Tin nhắn không tồn tại");
                }

                var User = _userService.GetById(new GetByIdRequest { Id = UserId});


                update.LastReadMessageId = message.Id;
                update.NguoiSua = User.Data.Username;
                update.NgayTao = DateTime.Now;

                _context.Conversations.Update(update);
                _context.SaveChanges();
            }
            catch(Exception ex)
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
            var message = _context.Messages.Where(m => (m.SenderId == UserId || m.SenderId == TargetId)
                                                 && (m.ReceiverId == UserId || m.ReceiverId == TargetId)
                                                 && !m.IsDeleted)
                                           .OrderByDescending(m => m.NgayTao)
                                           .FirstOrDefault();

            return _mapper.Map<MODELMessage>(message);
        }
        #endregion
    }
}
