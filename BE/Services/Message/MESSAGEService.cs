using AutoMapper;
using BE.Helpers;
using BE.Services.MediaFile;
using BE.Services.ReactionType;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.MESSAGEREACTION.Dtos;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Net.WebSockets;

namespace BE.Services.Message
{
    public class MESSAGEService : IMESSAGEService
    {
        private readonly LINKUContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUSERService _userService;
        private readonly IMEDIAFILEService _mediaFileService;
        private readonly IREACTIONTYPEService _reactionTypeService;


        public MESSAGEService(LINKUContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IUSERService userService, IMEDIAFILEService mediaFileService, IREACTIONTYPEService reactionTypeService)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _userService = userService;
            _mediaFileService = mediaFileService;
            _reactionTypeService = reactionTypeService;
        }

        public BaseResponse<GetListPagingResponse> GetListPaging(PostMessageGetListPagingRequest request)
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
                    new SqlParameter("@iTargetId", request.TargetId),
                    new SqlParameter("@iConversationType", request.ConversationType),
                    iTotalRow
                };

                var result = _context.ExcuteStoredProcedure<MODELMessage>("sp_MESSAGE_GetListPaging", parameters).ToList();
                result = HanleDataGetListPaging(result, request.ConversationType, request.UserId, request.TargetId).Data;

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
        public BaseResponse<MODELMessage> GetById(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                var result = new MODELMessage();
                var data = _context.Messages.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (data == null)
                {
                    throw new Exception("Không tìm thấy dữ liệu");
                }
                else
                {
                    result = _mapper.Map<MODELMessage>(data);
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
        public BaseResponse<PostMessageRequest> GetByPost(GetByIdRequest request)
        {
            var response = new BaseResponse<PostMessageRequest>();
            try
            {
                var result = new PostMessageRequest();
                var data = _context.Messages.FindAsync(request.Id);
                if (data == null)
                {
                    result.IsEdit = false;
                }
                else
                {
                    result = _mapper.Map<PostMessageRequest>(data);
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
        public BaseResponse<MODELMessage> Insert(PostMessageRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                if (request.Content == "")
                {
                    throw new Exception("Nội dung không được để trống");
                }

                var add = _mapper.Map<ENTITIES.DbContent.Message>(request);
                add.Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

                var Sender = _userService.GetById(new GetByIdRequest { Id = request.SenderId}).Data;
                add.NguoiTao = Sender.Username;
                add.NgayTao = DateTime.Now;
                add.NguoiSua = Sender.Username;
                add.NgaySua = DateTime.Now;

                // Lưu dữ liệu
                _context.Messages.Add(add);
                if (request.IsSaveChange)
                {
                    _context.SaveChanges();
                }

                response.Data = _mapper.Map<MODELMessage>(add);
                response.Data.Sender = _mapper.Map<MODELUser>(Sender);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<MODELMessage> Update(PostMessageRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                var update = _context.Messages.Find(request.Id);
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
                    _context.Messages.Update(update);
                    _context.SaveChanges();

                    response.Data = _mapper.Map<MODELMessage>(update);
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
                    var delete = _context.Messages.Find(id);
                    if (delete != null)
                    {
                        delete.NguoiXoa = _contextAccessor.HttpContext.User.Identity.Name;
                        delete.NgayXoa = DateTime.Now;
                        _context.Messages.Remove(delete);
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

        public BaseResponse<bool> RoolbackDelete(GetByIdRequest request)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var delete = _context.Messages.Find(request.Id);
                if (delete != null)
                {
                    delete.IsDeleted = true;
                    delete.NguoiXoa = "roolback";
                    delete.NgayXoa = DateTime.Now;
                    // Lưu dữ liệu
                    _context.Messages.Update(delete);
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

        public async Task<BaseResponse<List<MODELSendMessageWithFileResponse>>> SendMessageWithFile(POSTSendMessageWithFileRequest request)
        {
            var response = new BaseResponse<List<MODELSendMessageWithFileResponse>>();
            try
            {
                var ListMessage = new List<MODELMessage>();

                // Gửi tin nhắn thông thường (nếu có)
                if (!string.IsNullOrEmpty(request.Content) && !string.IsNullOrWhiteSpace(request.Content))
                {
                    var message = new PostMessageRequest
                    {
                        Id = Guid.NewGuid(),
                        SenderId = request.SenderId,
                        TargetId = request.TargetId,
                        Content = request.Content,
                        MessageType = (int)MODELS.COMMON.MessageType.Text, // Tin nhắn bình thường
                        RefId = request.RefId,
                        IsSaveChange = false
                    };

                    var insertResponse = Insert(message);

                    if (insertResponse.Error)
                    {
                        throw new Exception(insertResponse.Message);
                    }

                    ListMessage.Add(insertResponse.Data);
                }

                // Xử lý Upload File
                var uploadFileResult = await _mediaFileService.UploadFileAsync(request.Files);
                if (uploadFileResult.Error)
                {
                    throw new Exception(uploadFileResult.Message);
                }
                else
                {
                    foreach (var file in uploadFileResult.Data)
                    {
                        // Tạo Message
                        var message = Insert(new PostMessageRequest
                        {
                            Id = Guid.NewGuid(),
                            SenderId = request.SenderId,
                            TargetId = request.TargetId,
                            Content = null, // Lưu tên file vào nội dung tin nhắn
                            MessageType = (int)MODELS.COMMON.MessageType.File, // Tin nhắn là File
                            RefId = request.RefId,
                            IsSaveChange = false
                        });

                        if (message.Error)
                        {
                            throw new Exception(message.Message);
                        }

                        // Tạo File
                        var mediafile = _mediaFileService.Insert(new POSTMediaFileRequest
                        {
                            FileName = file.FileName,
                            FileType = file.FileType,
                            Url = file.Url,
                            MessageId = message.Data.Id,
                            IsSaveChange = false,
                            Shape = file.Shape,
                            FileLength = file.FileLength
                        });

                        if (mediafile.Error)
                        {
                            throw new Exception(mediafile.Message);
                        }

                        ListMessage.Add(message.Data);
                    }
                }

                // Cập nhật Conversation với tin nhắn mới nhất cho người gửi
                var conversation = _context.Conversations.FirstOrDefault(x => x.UserId == request.SenderId 
                                                                        && x.TargetId == request.TargetId 
                                                                        && !x.IsDeleted);

                if (conversation != null)
                {
                    conversation.LastReadMessageId = ListMessage.LastOrDefault()?.Id ?? Guid.Empty;
                    conversation.NgaySua = DateTime.Now;
                    conversation.NguoiSua = _contextAccessor.HttpContext.User.Identity.Name;

                    _context.Conversations.Update(conversation);
                }

                // Lưu tất cả các thay đổi trong context
                _context.SaveChanges();

                // Reverse Message
                ListMessage.Reverse();
                // Xử lý dữ liệu để trả về
                var MyResponse = HanleDataGetListPaging(ListMessage, request.ConversationType, request.SenderId, request.TargetId).Data;
                var TargetResponse = HanleDataGetListPaging(ListMessage, request.ConversationType, request.TargetId, request.SenderId).Data;

                response.Data = new List<MODELSendMessageWithFileResponse> {
                    new MODELSendMessageWithFileResponse
                    {
                        Messages = new GetListPagingResponse
                        {
                            PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                            Data = MyResponse,
                            TotalRow = MyResponse.Count
                        },
                        IsMyResponse = true
                    },
                    new MODELSendMessageWithFileResponse
                    {
                        Messages = new GetListPagingResponse
                        {
                            PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                            Data = TargetResponse,
                            TotalRow = TargetResponse.Count
                        },
                        IsMyResponse = false
                    },
                };
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<List<MODELMessage>> HanleDataGetListPaging(List<MODELMessage> result, int conversationType, Guid UserId, Guid TargetId)
        {
            var response = new BaseResponse<List<MODELMessage>>();
            try
            {
                if (conversationType == 0)
                {
                    var CurrentUser = _userService.GetById(new GetByIdRequest() { Id = UserId }).Data;
                    var Target = _userService.GetById(new GetByIdRequest() { Id = TargetId }).Data;

                    foreach (var item in result)
                    {
                        item.Sender = item.SenderId == UserId ? CurrentUser : Target;

                        // Tin nhắn là File
                        if (item.MessageType == 3)
                        {
                            var data = _context.MediaFiles.FirstOrDefault(x => x.MessageId == item.Id);
                            if (data != null)
                            {
                                item.MediaFile = _mapper.Map<MODELMediaFile>(data);
                            }
                            else
                            {
                                item.MediaFile = new MODELMediaFile
                                {
                                    Id = Guid.Empty,
                                    FileName = "File không tồn tại",
                                    FileType = (int)MediaFileType.ChatFile,
                                    Url = string.Empty,
                                };
                            }
                        }

                        // Xử lý RefMessage
                        if (item.RefId != null && item.RefId != Guid.Empty)
                        {
                            var refMessage = GetById(new GetByIdRequest { Id = item.RefId.Value });
                            if (refMessage.Error)
                            {
                                item.RefMessage = new MODELMessage
                                {
                                    Id = Guid.Empty,
                                    Content = "Đã gỡ tin nhắn",
                                };
                            }
                            else
                            {
                                item.RefMessage = refMessage.Data;
                                item.RefMessage.Sender = item.RefMessage.SenderId == UserId ? CurrentUser : Target;

                                if (item.RefMessage.MessageType == 3)
                                {
                                    var data = _context.MediaFiles.FirstOrDefault(x => x.MessageId == item.RefMessage.Id);
                                    if (data != null)
                                    {
                                        item.RefMessage.MediaFile = _mapper.Map<MODELMediaFile>(data);
                                    }
                                    else
                                    {
                                        item.RefMessage.MediaFile = new MODELMediaFile
                                        {
                                            Id = Guid.Empty,
                                            FileName = "File không tồn tại",
                                            FileType = (int)MediaFileType.ChatFile,
                                            Url = string.Empty,
                                        };
                                    }
                                }
                            }
                        }

                        // Xử lý MessageReaction
                        var messageReaction = _context.MessageReactions.Where(x => x.MessageId == item.Id && !x.IsDeleted);
                        if (messageReaction.Count() > 0)
                        {
                            // Khởi tạo MessageReaction với ReactionCount
                            item.MessageReaction = new MODELMessageReaction
                            {
                                ReactionCount = messageReaction.Count(),
                            };

                            // Lấy danh sách các ReactionType
                            var reactionType = messageReaction.Select(x => x.ReactionTypeId).Distinct().ToList();
                            foreach (var reactionId in reactionType)
                            {
                                var reaction = _reactionTypeService.GetById(new GetByIdRequest { Id = reactionId });
                                if (!reaction.Error)
                                {
                                    item.MessageReaction.ReactionTypes.Add(reaction.Data);
                                }
                            }

                            // Lấy danh sách người đã phản ứng với tin nhắn
                            var userReaction = messageReaction.Select(x => x.UserId).Take(3).ToList();
                            foreach (var userId in userReaction)
                            {
                                var user = _userService.GetById(new GetByIdRequest { Id = userId });
                                if (!user.Error)
                                {
                                    item.MessageReaction.ReactedUsers.Add(user.Data);
                                }
                            }

                            // Lấy ra Reaction của người dùng hiện tại (nếu có)
                            var exitstMyReaction = messageReaction.FirstOrDefault(x => x.UserId == UserId);
                            if(exitstMyReaction != null)
                            {
                                item.MessageReaction.MyReactionTypeId = exitstMyReaction.ReactionTypeId;
                            }
                        }
                    }
                }
                else
                {
                    var currentUser = _context.Users.Find(UserId);

                    // Duyệt qua từng tin nhắn
                    foreach (var item in result)
                    {
                        item.Sender = _userService.GetById(new GetByIdRequest() { Id = item.SenderId }).Data;

                        // Tin nhắn là Welcome hoặc Notification
                        if (item.MessageType == 1 || item.MessageType == 2)
                        {
                            MODELMessageContent content = JsonConvert.DeserializeObject<MODELMessageContent>(item.Content);

                            // Thay đổi nội dung của tin nhắn theo người dùng hiện tại
                            // Case 1: User hiện tại là người tạo ra message này
                            if (currentUser.Id == content.UserId)
                            {
                                // Nếu ta thêm ai đó vào nhóm
                                if (content.TargetId.Count > 0)
                                {
                                    List<string> usernames = new List<string>();
                                    foreach (var userid in content.TargetId)
                                    {
                                        var user = _context.Users.Find(userid);
                                        if (user != null)
                                        {
                                            usernames.Add(string.Concat(user.HoLot, " ", user.Ten));
                                        }
                                    }

                                    // Cập nhật nội dung tin nhắn
                                    item.Content = $"Bạn đã thêm {string.Join(", ", usernames)} vào nhóm";
                                }
                                // Nếu ta tham gia bằng Lời mời (GroupRequest)
                                else
                                {
                                    // Cập nhật nội dung tin nhắn
                                    item.Content = $"Bạn đã tham gia nhóm";
                                }
                            }
                            // Case 2: Bạn là 1 trong những người nhận tin nhắn
                            else if (content.TargetId.Contains(currentUser.Id))
                            {
                                var user = _context.Users.Find(content.UserId);
                                if (user != null)
                                {
                                    // Cập nhật nội dung tin nhắn
                                    item.Content = $"{string.Concat(user.HoLot, " ", user.Ten)} đã thêm bạn vào nhóm";
                                }
                            }
                            // Case 3: Bạn không phải là người tạo và cũng không phải là người nhận tin nhắn
                            else
                            {
                                if (content.TargetId.Count > 0)
                                {
                                    var user = _context.Users.Find(content.UserId);
                                    var usernames = new List<string>();
                                    foreach (var userid in content.TargetId)
                                    {
                                        var targetUser = _context.Users.Find(userid);
                                        if (targetUser != null)
                                        {
                                            usernames.Add(string.Concat(targetUser.HoLot, " ", targetUser.Ten));
                                        }
                                    }

                                    if (user != null)
                                    {
                                        // Cập nhật nội dung tin nhắn
                                        item.Content = $"{string.Concat(user.HoLot, " ", user.Ten)} đã thêm {string.Join(", ", usernames)} vào nhóm";
                                    }
                                }
                                else
                                {
                                    var user = _context.Users.Find(content.UserId);
                                    if (user != null)
                                    {
                                        // Cập nhật nội dung tin nhắn
                                        item.Content = $"{string.Concat(user.HoLot, " ", user.Ten)} đã tham gia nhóm";
                                    }
                                }
                            }
                        }
                        else if (item.MessageType == 3) // Tin nhắn là File
                        {
                            var data = _context.MediaFiles.FirstOrDefault(x => x.MessageId == item.Id);
                            if (data != null)
                            {
                                item.MediaFile = _mapper.Map<MODELMediaFile>(data);
                            }
                            else
                            {
                                item.MediaFile = new MODELMediaFile
                                {
                                    Id = Guid.Empty,
                                    FileName = "File không tồn tại",
                                    FileType = (int)MediaFileType.ChatFile,
                                    Url = string.Empty,
                                };
                            }
                        }

                        // Xử lý RefMessage
                        if (item.RefId != null && item.RefId != Guid.Empty)
                        {
                            var refMessage = GetById(new GetByIdRequest { Id = item.RefId.Value });
                            if (refMessage.Error)
                            {
                                item.RefMessage = new MODELMessage
                                {
                                    Id = Guid.Empty,
                                    Content = "Đã gỡ tin nhắn",
                                };
                            }
                            else
                            {
                                item.RefMessage = refMessage.Data;
                                item.RefMessage.Sender = _userService.GetById(new GetByIdRequest() { Id = item.RefMessage.SenderId }).Data; ;

                                if (item.RefMessage.MessageType == 3)
                                {
                                    var data = _context.MediaFiles.FirstOrDefault(x => x.MessageId == item.RefMessage.Id);
                                    if (data != null)
                                    {
                                        item.RefMessage.MediaFile = _mapper.Map<MODELMediaFile>(data);
                                    }
                                    else
                                    {
                                        item.RefMessage.MediaFile = new MODELMediaFile
                                        {
                                            Id = Guid.Empty,
                                            FileName = "File không tồn tại",
                                            FileType = (int)MediaFileType.ChatFile,
                                            Url = string.Empty,
                                        };
                                    }
                                }
                            }
                        }
                        
                        // Xử lý MessageReaction
                        var messageReaction = _context.MessageReactions.Where(x => x.MessageId == item.Id && !x.IsDeleted);
                        if(messageReaction.Count() > 0)
                        {
                            // Khởi tạo MessageReaction với ReactionCount
                            item.MessageReaction = new MODELMessageReaction
                            {
                                ReactionCount = messageReaction.Count(),
                            };

                            // Lấy danh sách các ReactionType
                            var reactionType = messageReaction.Select(x => x.ReactionTypeId).Distinct().ToList();
                            foreach (var reactionId in reactionType)
                            {
                                var reaction = _reactionTypeService.GetById(new GetByIdRequest { Id = reactionId });
                                if (!reaction.Error)
                                {
                                    item.MessageReaction.ReactionTypes.Add(reaction.Data);
                                }
                            }

                            // Lấy danh sách người đã phản ứng với tin nhắn
                            var userReaction = messageReaction.Select(x => x.UserId).Take(3).ToList();
                            foreach (var userId in userReaction)
                            {
                                var user = _userService.GetById(new GetByIdRequest { Id = userId });
                                if (!user.Error)
                                {
                                    item.MessageReaction.ReactedUsers.Add(user.Data);
                                }
                            }

                            // Lấy ra Reaction của người dùng hiện tại (nếu có)
                            var exitstMyReaction = messageReaction.FirstOrDefault(x => x.UserId == UserId);
                            if (exitstMyReaction != null)
                            {
                                item.MessageReaction.MyReactionTypeId = exitstMyReaction.ReactionTypeId;
                            }
                        }
                    }
                }
                
                response.Data = _mapper.Map<List<MODELMessage>>(result);
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
