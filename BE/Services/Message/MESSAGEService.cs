using AutoMapper;
using BE.Helpers;
using BE.Services.MediaFile;
using BE.Services.ReactionType;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MODELS.BASE;
using MODELS.COMMON;
using MODELS.MEDIAFILE.Dtos;
using MODELS.MEDIAFILE.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.MESSAGEREACTION.Dtos;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public async Task<BaseResponse<GetListPagingResponse>> GetListPaging(PostMessageGetListPagingRequest request)
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
                var data = _context.Messages.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                if (data == null)
                    throw new Exception("Không tìm thấy dữ liệu");

                response.Data = _mapper.Map<MODELMessage>(data);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<MODELMessage>> GetByIdAsync(GetByIdRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                var data = await _context.Messages
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

                if (data == null)
                    throw new Exception("Không tìm thấy dữ liệu");

                response.Data = _mapper.Map<MODELMessage>(data);
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
        
        public async Task<BaseResponse<MODELMessage>> Insert(PostMessageRequest request)
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

                var Sender = (await _userService.GetByIdAsync(new GetByIdRequest { Id = request.SenderId })).Data;
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

                    var insertResponse = await Insert(message);

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
                        var message = await Insert(new PostMessageRequest
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
                else
                {
                    if (request.ConversationType == 0)
                    {
                        // Nếu không tìm thấy Conversation, tạo mới
                        var newConversation = new List<ENTITIES.DbContent.Conversation>
                        {
                            // Sender
                            new ENTITIES.DbContent.Conversation
                            {
                                Id = Guid.NewGuid(),
                                UserId = request.SenderId,
                                TargetId = request.TargetId,
                                LastReadMessageId = ListMessage.LastOrDefault()?.Id ?? Guid.Empty,
                                NgayTao = DateTime.Now,
                                NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                                NgaySua = DateTime.Now,
                                NguoiSua = _contextAccessor.HttpContext.User.Identity.Name
                            },
                            // Target
                            new ENTITIES.DbContent.Conversation
                            {
                                Id = Guid.NewGuid(),
                                UserId = request.TargetId,
                                TargetId = request.SenderId,
                                LastReadMessageId = Guid.Empty,
                                NgayTao = DateTime.Now,
                                NguoiTao = _contextAccessor.HttpContext.User.Identity.Name,
                                NgaySua = DateTime.Now,
                                NguoiSua = _contextAccessor.HttpContext.User.Identity.Name
                            }
                        };

                        _context.Conversations.AddRange(newConversation);
                    }
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

        // 203ms
        public async Task<BaseResponse<List<MODELMessage>>> HanleDataGetListPagingAsync(List<MODELMessage> result, int conversationType, Guid UserId, Guid TargetId)
        {
            var sw = Stopwatch.StartNew();
            var response = new BaseResponse<List<MODELMessage>>();
            try
            {
                if (conversationType == 0)
                {
                    var CurrentUser = (await _userService.GetByIdAsync(new GetByIdRequest() { Id = UserId })).Data;
                    var Target = (await _userService.GetByIdAsync(new GetByIdRequest() { Id = TargetId })).Data;

                    // Tập hợp các ID cần preload
                    var messageIds = result.Select(x => x.Id).ToList();
                    var refIds = result.Where(x => x.RefId.HasValue && x.RefId != Guid.Empty)
                                       .Select(x => x.RefId.Value)
                                       .Distinct()
                                       .ToList();

                    // Preload MediaFiles cho MessageId và RefId
                    var mediaFiles = _context.MediaFiles
                        .Where(x => (messageIds.Contains(x.MessageId.Value) || refIds.Contains(x.MessageId.Value)) && !x.IsDeleted)
                        .AsNoTracking()
                        .ToList()
                        .ToLookup(x => x.MessageId);

                    // Preload RefMessages
                    var refMessages = GetByIds(refIds) // bạn cần viết GetByIds để trả về Dictionary<Guid, MODELMessage>
                                        .ToDictionary(x => x.Id, x => x);

                    // Preload Reactions
                    var allReactions = _context.MessageReactions
                        .Where(x => messageIds.Contains(x.MessageId) && !x.IsDeleted)
                        .AsNoTracking()
                        .ToList()
                        .GroupBy(x => x.MessageId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // Lấy tất cả ReactionTypeId & UserId duy nhất để preload batch
                    var allReactionTypeIds = allReactions.SelectMany(x => x.Value.Select(r => r.ReactionTypeId)).Distinct().ToList();
                    var allUserIds = allReactions.SelectMany(x => x.Value.Select(r => r.UserId)).Distinct()
                                      .Concat(refMessages.Values.Select(x => x.SenderId))
                                      .Distinct()
                                      .ToList();

                    // Preload ReactionTypes và Users
                    var reactionTypesDict = _reactionTypeService.GetByIds(allReactionTypeIds).ToDictionary(x => x.Id);
                    var usersDict = _userService.GetByIds(allUserIds).ToDictionary(x => x.Id);

                    // Lặp và gán dữ liệu đã preload
                    foreach (var item in result)
                    {
                        item.Sender = item.SenderId == UserId ? CurrentUser : Target;

                        // MediaFile cho item
                        var file = mediaFiles[item.Id].FirstOrDefault();
                        item.MediaFile = file != null
                            ? _mapper.Map<MODELMediaFile>(file)
                            : new MODELMediaFile
                            {
                                Id = Guid.Empty,
                                FileName = "File không tồn tại",
                                FileType = (int)MediaFileType.ChatFile,
                                Url = string.Empty
                            };

                        // RefMessage
                        if (item.RefId.HasValue && refMessages.TryGetValue(item.RefId.Value, out var refMsg))
                        {
                            item.RefMessage = refMsg;
                            item.RefMessage.Sender = usersDict.TryGetValue(refMsg.SenderId, out var sender) ? sender : null;

                            if (refMsg.MessageType == 3)
                            {
                                var refFile = mediaFiles[refMsg.Id].FirstOrDefault();
                                item.RefMessage.MediaFile = refFile != null
                                    ? _mapper.Map<MODELMediaFile>(refFile)
                                    : new MODELMediaFile
                                    {
                                        Id = Guid.Empty,
                                        FileName = "File không tồn tại",
                                        FileType = (int)MediaFileType.ChatFile,
                                        Url = string.Empty
                                    };
                            }
                        }
                        else
                        {
                            item.RefMessage = new MODELMessage
                            {
                                Id = Guid.Empty,
                                Content = "Đã gỡ tin nhắn"
                            };
                        }

                        // MessageReactions
                        if (allReactions.TryGetValue(item.Id, out var reactions) && reactions.Any())
                        {
                            var reactionTypeIds = reactions.Select(r => r.ReactionTypeId).Distinct().ToList();
                            var userIds = reactions.Select(r => r.UserId).Distinct().Take(3).ToList();

                            item.MessageReaction = new MODELMessageReaction
                            {
                                ReactionCount = reactions.Count,
                                ReactionTypes = reactionTypeIds
                                    .Where(reactionTypesDict.ContainsKey)
                                    .Select(id => reactionTypesDict[id])
                                    .ToList(),
                                ReactedUsers = userIds
                                    .Where(usersDict.ContainsKey)
                                    .Select(id => usersDict[id])
                                    .ToList(),
                                MyReactionTypeId = reactions.FirstOrDefault(r => r.UserId == UserId)?.ReactionTypeId
                            };
                        }
                    }
                }
                else if (conversationType == 1)
                {
                    var currentUser = _context.Users.Find(UserId);

                    // Tập hợp các ID cần preload
                    var senderIds = result.Select(x => x.SenderId).Distinct().ToList();
                    var refIds = result.Where(x => x.RefId.HasValue && x.RefId != Guid.Empty)
                                       .Select(x => x.RefId.Value)
                                       .Distinct()
                                       .ToList();
                    var messageIds = result.Select(x => x.Id).ToList();

                    // Preload tất cả người dùng liên quan
                    var allTargetUserIds = result
                        .Where(x => x.MessageType == 1 || x.MessageType == 2)
                        .SelectMany(x => JsonConvert.DeserializeObject<MODELMessageContent>(x.Content).TargetId)
                        .Distinct()
                        .ToList();

                    var userIdsToPreload = senderIds
                        .Concat(allTargetUserIds)
                        .Concat(refIds.Select(id => GetById(new GetByIdRequest { Id = id }).Data?.SenderId ?? Guid.Empty))
                        .Distinct()
                        .Where(id => id != Guid.Empty)
                        .ToList();

                    var usersDict = userIdsToPreload.Select(x => _userService.GetById(new GetByIdRequest { Id = x }).Data).ToDictionary(x => x.Id);

                    // Preload MediaFiles
                    var mediaFiles = _context.MediaFiles
                        .Where(x => messageIds.Contains(x.MessageId.Value) || refIds.Contains(x.MessageId.Value))
                        .AsNoTracking()
                        .ToList()
                        .ToLookup(x => x.MessageId);

                    // Preload RefMessages
                    var refMessages = GetByIds(refIds).ToDictionary(x => x.Id, x => x);

                    // Preload Reactions
                    var allReactions = _context.MessageReactions
                        .Where(x => messageIds.Contains(x.MessageId) && !x.IsDeleted)
                        .AsNoTracking()
                        .ToList()
                        .GroupBy(x => x.MessageId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    var allReactionTypeIds = allReactions.SelectMany(x => x.Value.Select(r => r.ReactionTypeId)).Distinct().ToList();
                    var reactionTypesDict = _reactionTypeService.GetByIds(allReactionTypeIds).ToDictionary(x => x.Id);

                    // Xử lý từng item
                    foreach (var item in result)
                    {
                        // Gán Sender
                        usersDict.TryGetValue(item.SenderId, out var sender);
                        item.Sender = sender;

                        if (item.MessageType == 1 || item.MessageType == 2)
                        {
                            var content = JsonConvert.DeserializeObject<MODELMessageContent>(item.Content);

                            // Case 1: User hiện tại là người tạo message
                            if (currentUser.Id == content.UserId)
                            {
                                if (content.TargetId.Count > 0)
                                {
                                    var usernames = content.TargetId
                                        .Where(usersDict.ContainsKey)
                                        .Select(id => usersDict[id])
                                        .Select(u => $"{u.HoLot} {u.Ten}")
                                        .ToList();
                                    item.Content = $"Bạn đã thêm {string.Join(", ", usernames)} vào nhóm";
                                }
                                else
                                {
                                    item.Content = "Bạn đã tham gia nhóm";
                                }
                            }
                            else if (content.TargetId.Contains(currentUser.Id))
                            {
                                if (usersDict.TryGetValue(content.UserId, out var user))
                                {
                                    item.Content = $"{user.HoLot} {user.Ten} đã thêm bạn vào nhóm";
                                }
                            }
                            else
                            {
                                if (usersDict.TryGetValue(content.UserId, out var user))
                                {
                                    var usernames = content.TargetId
                                        .Where(usersDict.ContainsKey)
                                        .Select(id => usersDict[id])
                                        .Select(u => $"{u.HoLot} {u.Ten}")
                                        .ToList();
                                    item.Content = usernames.Count > 0
                                        ? $"{user.HoLot} {user.Ten} đã thêm {string.Join(", ", usernames)} vào nhóm"
                                        : $"{user.HoLot} {user.Ten} đã tham gia nhóm";
                                }
                            }
                        }

                        // Gán MediaFile nếu là file
                        if (item.MessageType == 3)
                        {
                            var file = mediaFiles[item.Id].FirstOrDefault();
                            item.MediaFile = file != null
                                ? _mapper.Map<MODELMediaFile>(file)
                                : new MODELMediaFile
                                {
                                    Id = Guid.Empty,
                                    FileName = "File không tồn tại",
                                    FileType = (int)MediaFileType.ChatFile,
                                    Url = string.Empty
                                };
                        }

                        // RefMessage
                        if (item.RefId.HasValue && refMessages.TryGetValue(item.RefId.Value, out var refMsg))
                        {
                            item.RefMessage = refMsg;
                            usersDict.TryGetValue(refMsg.SenderId, out var refSender);
                            item.RefMessage.Sender = refSender;

                            if (refMsg.MessageType == 3)
                            {
                                var refFile = mediaFiles[refMsg.Id].FirstOrDefault();
                                item.RefMessage.MediaFile = refFile != null
                                    ? _mapper.Map<MODELMediaFile>(refFile)
                                    : new MODELMediaFile
                                    {
                                        Id = Guid.Empty,
                                        FileName = "File không tồn tại",
                                        FileType = (int)MediaFileType.ChatFile,
                                        Url = string.Empty
                                    };
                            }
                        }
                        else if (item.RefId != null)
                        {
                            item.RefMessage = new MODELMessage
                            {
                                Id = Guid.Empty,
                                Content = "Đã gỡ tin nhắn"
                            };
                        }

                        // MessageReaction
                        if (allReactions.TryGetValue(item.Id, out var reactions) && reactions.Any())
                        {
                            var reactionTypeIds = reactions.Select(r => r.ReactionTypeId).Distinct();
                            var userIds = reactions.Select(r => r.UserId).Distinct().Take(3);

                            item.MessageReaction = new MODELMessageReaction
                            {
                                ReactionCount = reactions.Count,
                                ReactionTypes = reactionTypeIds
                                    .Where(reactionTypesDict.ContainsKey)
                                    .Select(id => reactionTypesDict[id])
                                    .ToList(),
                                ReactedUsers = userIds
                                    .Where(usersDict.ContainsKey)
                                    .Select(id => usersDict[id])
                                    .ToList(),
                                MyReactionTypeId = reactions.FirstOrDefault(r => r.UserId == UserId)?.ReactionTypeId
                            };
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
            sw.Stop();
            Console.WriteLine($"HanleDataGetListPagingAsync took {sw.ElapsedMilliseconds} ms");
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

                    // Tập hợp các ID cần preload
                    var messageIds = result.Select(x => x.Id).ToList();
                    var refIds = result.Where(x => x.RefId.HasValue && x.RefId != Guid.Empty)
                                       .Select(x => x.RefId.Value)
                                       .Distinct()
                                       .ToList();

                    // Preload MediaFiles cho MessageId và RefId
                    var mediaFiles = _context.MediaFiles
                        .Where(x => (messageIds.Contains(x.MessageId.Value) || refIds.Contains(x.MessageId.Value)) && !x.IsDeleted)
                        .AsNoTracking()
                        .ToList()
                        .ToLookup(x => x.MessageId);

                    // Preload RefMessages
                    var refMessages = GetByIds(refIds) // bạn cần viết GetByIds để trả về Dictionary<Guid, MODELMessage>
                                        .ToDictionary(x => x.Id, x => x);

                    // Preload Reactions
                    var allReactions = _context.MessageReactions
                        .Where(x => messageIds.Contains(x.MessageId) && !x.IsDeleted)
                        .AsNoTracking()
                        .ToList()
                        .GroupBy(x => x.MessageId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // Lấy tất cả ReactionTypeId & UserId duy nhất để preload batch
                    var allReactionTypeIds = allReactions.SelectMany(x => x.Value.Select(r => r.ReactionTypeId)).Distinct().ToList();
                    var allUserIds = allReactions.SelectMany(x => x.Value.Select(r => r.UserId)).Distinct()
                                      .Concat(refMessages.Values.Select(x => x.SenderId))
                                      .Distinct()
                                      .ToList();

                    // Preload ReactionTypes và Users
                    var reactionTypesDict = _reactionTypeService.GetByIds(allReactionTypeIds).ToDictionary(x => x.Id);
                    var usersDict = _userService.GetByIds(allUserIds).ToDictionary(x => x.Id);

                    // Lặp và gán dữ liệu đã preload
                    foreach (var item in result)
                    {
                        item.Sender = item.SenderId == UserId ? CurrentUser : Target;

                        // MediaFile cho item
                        var file = mediaFiles[item.Id].FirstOrDefault();
                        item.MediaFile = file != null
                            ? _mapper.Map<MODELMediaFile>(file)
                            : new MODELMediaFile
                            {
                                Id = Guid.Empty,
                                FileName = "File không tồn tại",
                                FileType = (int)MediaFileType.ChatFile,
                                Url = string.Empty
                            };

                        // RefMessage
                        if (item.RefId.HasValue && refMessages.TryGetValue(item.RefId.Value, out var refMsg))
                        {
                            item.RefMessage = refMsg;
                            item.RefMessage.Sender = usersDict.TryGetValue(refMsg.SenderId, out var sender) ? sender : null;

                            if (refMsg.MessageType == 3)
                            {
                                var refFile = mediaFiles[refMsg.Id].FirstOrDefault();
                                item.RefMessage.MediaFile = refFile != null
                                    ? _mapper.Map<MODELMediaFile>(refFile)
                                    : new MODELMediaFile
                                    {
                                        Id = Guid.Empty,
                                        FileName = "File không tồn tại",
                                        FileType = (int)MediaFileType.ChatFile,
                                        Url = string.Empty
                                    };
                            }
                        }
                        else
                        {
                            item.RefMessage = new MODELMessage
                            {
                                Id = Guid.Empty,
                                Content = "Đã gỡ tin nhắn"
                            };
                        }

                        // MessageReactions
                        if (allReactions.TryGetValue(item.Id, out var reactions) && reactions.Any())
                        {
                            var reactionTypeIds = reactions.Select(r => r.ReactionTypeId).Distinct().ToList();
                            var userIds = reactions.Select(r => r.UserId).Distinct().Take(3).ToList();

                            item.MessageReaction = new MODELMessageReaction
                            {
                                ReactionCount = reactions.Count,
                                ReactionTypes = reactionTypeIds
                                    .Where(reactionTypesDict.ContainsKey)
                                    .Select(id => reactionTypesDict[id])
                                    .ToList(),
                                ReactedUsers = userIds
                                    .Where(usersDict.ContainsKey)
                                    .Select(id => usersDict[id])
                                    .ToList(),
                                MyReactionTypeId = reactions.FirstOrDefault(r => r.UserId == UserId)?.ReactionTypeId
                            };
                        }
                    }
                }
                else if (conversationType == 1)
                {
                    var currentUser = _context.Users.Find(UserId);

                    // Tập hợp các ID cần preload
                    var senderIds = result.Select(x => x.SenderId).Distinct().ToList();
                    var refIds = result.Where(x => x.RefId.HasValue && x.RefId != Guid.Empty)
                                       .Select(x => x.RefId.Value)
                                       .Distinct()
                                       .ToList();
                    var messageIds = result.Select(x => x.Id).ToList();

                    // Preload tất cả người dùng liên quan
                    var allTargetUserIds = result
                        .Where(x => x.MessageType == 1 || x.MessageType == 2)
                        .SelectMany(x => JsonConvert.DeserializeObject<MODELMessageContent>(x.Content).TargetId)
                        .Distinct()
                        .ToList();

                    var userIdsToPreload = senderIds
                        .Concat(allTargetUserIds)
                        .Concat(refIds.Select(id => GetById(new GetByIdRequest { Id = id }).Data?.SenderId ?? Guid.Empty))
                        .Distinct()
                        .Where(id => id != Guid.Empty)
                        .ToList();

                    var usersDict = userIdsToPreload.Select(x => _userService.GetById(new GetByIdRequest { Id = x }).Data).ToDictionary(x => x.Id);

                    // Preload MediaFiles
                    var mediaFiles = _context.MediaFiles
                        .Where(x => messageIds.Contains(x.MessageId.Value) || refIds.Contains(x.MessageId.Value))
                        .AsNoTracking()
                        .ToList()
                        .ToLookup(x => x.MessageId);

                    // Preload RefMessages
                    var refMessages = GetByIds(refIds).ToDictionary(x => x.Id, x => x);

                    // Preload Reactions
                    var allReactions = _context.MessageReactions
                        .Where(x => messageIds.Contains(x.MessageId) && !x.IsDeleted)
                        .AsNoTracking()
                        .ToList()
                        .GroupBy(x => x.MessageId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    var allReactionTypeIds = allReactions.SelectMany(x => x.Value.Select(r => r.ReactionTypeId)).Distinct().ToList();
                    var reactionTypesDict = _reactionTypeService.GetByIds(allReactionTypeIds).ToDictionary(x => x.Id);

                    // Xử lý từng item
                    foreach (var item in result)
                    {
                        // Gán Sender
                        usersDict.TryGetValue(item.SenderId, out var sender);
                        item.Sender = sender;

                        if (item.MessageType == 1 || item.MessageType == 2)
                        {
                            var content = JsonConvert.DeserializeObject<MODELMessageContent>(item.Content);

                            // Case 1: User hiện tại là người tạo message
                            if (currentUser.Id == content.UserId)
                            {
                                if (content.TargetId.Count > 0)
                                {
                                    var usernames = content.TargetId
                                        .Where(usersDict.ContainsKey)
                                        .Select(id => usersDict[id])
                                        .Select(u => $"{u.HoLot} {u.Ten}")
                                        .ToList();
                                    item.Content = $"Bạn đã thêm {string.Join(", ", usernames)} vào nhóm";
                                }
                                else
                                {
                                    item.Content = "Bạn đã tham gia nhóm";
                                }
                            }
                            else if (content.TargetId.Contains(currentUser.Id))
                            {
                                if (usersDict.TryGetValue(content.UserId, out var user))
                                {
                                    item.Content = $"{user.HoLot} {user.Ten} đã thêm bạn vào nhóm";
                                }
                            }
                            else
                            {
                                if (usersDict.TryGetValue(content.UserId, out var user))
                                {
                                    var usernames = content.TargetId
                                        .Where(usersDict.ContainsKey)
                                        .Select(id => usersDict[id])
                                        .Select(u => $"{u.HoLot} {u.Ten}")
                                        .ToList();
                                    item.Content = usernames.Count > 0
                                        ? $"{user.HoLot} {user.Ten} đã thêm {string.Join(", ", usernames)} vào nhóm"
                                        : $"{user.HoLot} {user.Ten} đã tham gia nhóm";
                                }
                            }
                        }

                        // Gán MediaFile nếu là file
                        if (item.MessageType == 3)
                        {
                            var file = mediaFiles[item.Id].FirstOrDefault();
                            item.MediaFile = file != null
                                ? _mapper.Map<MODELMediaFile>(file)
                                : new MODELMediaFile
                                {
                                    Id = Guid.Empty,
                                    FileName = "File không tồn tại",
                                    FileType = (int)MediaFileType.ChatFile,
                                    Url = string.Empty
                                };
                        }

                        // RefMessage
                        if (item.RefId.HasValue && refMessages.TryGetValue(item.RefId.Value, out var refMsg))
                        {
                            item.RefMessage = refMsg;
                            usersDict.TryGetValue(refMsg.SenderId, out var refSender);
                            item.RefMessage.Sender = refSender;

                            if (refMsg.MessageType == 3)
                            {
                                var refFile = mediaFiles[refMsg.Id].FirstOrDefault();
                                item.RefMessage.MediaFile = refFile != null
                                    ? _mapper.Map<MODELMediaFile>(refFile)
                                    : new MODELMediaFile
                                    {
                                        Id = Guid.Empty,
                                        FileName = "File không tồn tại",
                                        FileType = (int)MediaFileType.ChatFile,
                                        Url = string.Empty
                                    };
                            }
                        }
                        else if (item.RefId != null)
                        {
                            item.RefMessage = new MODELMessage
                            {
                                Id = Guid.Empty,
                                Content = "Đã gỡ tin nhắn"
                            };
                        }

                        // MessageReaction
                        if (allReactions.TryGetValue(item.Id, out var reactions) && reactions.Any())
                        {
                            var reactionTypeIds = reactions.Select(r => r.ReactionTypeId).Distinct();
                            var userIds = reactions.Select(r => r.UserId).Distinct().Take(3);

                            item.MessageReaction = new MODELMessageReaction
                            {
                                ReactionCount = reactions.Count,
                                ReactionTypes = reactionTypeIds
                                    .Where(reactionTypesDict.ContainsKey)
                                    .Select(id => reactionTypesDict[id])
                                    .ToList(),
                                ReactedUsers = userIds
                                    .Where(usersDict.ContainsKey)
                                    .Select(id => usersDict[id])
                                    .ToList(),
                                MyReactionTypeId = reactions.FirstOrDefault(r => r.UserId == UserId)?.ReactionTypeId
                            };
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

        List<MODELMessage> GetByIds(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return new List<MODELMessage>();

            var messages = _context.Messages
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .ToList();

            var result = _mapper.Map<List<MODELMessage>>(messages);

            return result;
        }

        public async Task<List<MODELMessage>> GetByIdsAsync(List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return new List<MODELMessage>();

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync();

            var result = _mapper.Map<List<MODELMessage>>(messages);

            return result;
        }

        public BaseResponse<MODELMessage> WSInsertPrivateMessage(PostMessageRequest request)
        {
            var response = new BaseResponse<MODELMessage>();
            try
            {
                // 1. Validate nhanh
                if (string.IsNullOrWhiteSpace(request.Content))
                    throw new Exception("Tin nhắn không được để trống");

                // 2. Truy xuất username trực tiếp nếu đã biết hoặc preload ở ngoài
                var sender = _context.Users
                    .AsNoTracking()
                    .Where(x => x.Id == request.SenderId)
                    .Select(x => new { x.Username }) // chỉ lấy Username
                    .FirstOrDefault();

                if (sender == null)
                    throw new Exception("Người gửi không tồn tại");

                // 3. Tạo message
                var now = DateTime.Now;
                var newMessage = new ENTITIES.DbContent.Message
                {
                    Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
                    Content = request.Content,
                    SenderId = request.SenderId,
                    TargetId = request.TargetId,
                    MessageType = request.MessageType,
                    RefId = request.RefId,
                    NgayTao = now,
                    NguoiTao = sender.Username,
                    NgaySua = now,
                    NguoiSua = sender.Username
                };

                _context.Messages.Add(newMessage);

                // 4. Kiểm tra xem đã có conversation chưa
                var conversation = _context.Conversations
                    .FirstOrDefault(x => x.UserId == request.SenderId
                                      && x.TargetId == request.TargetId
                                      && !x.IsDeleted);

                if (conversation != null)
                {
                    conversation.LastReadMessageId = newMessage.Id;
                    conversation.NgaySua = now;
                    conversation.NguoiSua = sender.Username;

                    _context.Conversations.Update(conversation);
                }
                else
                {
                    var newConversations = new[]
                    {
                        new ENTITIES.DbContent.Conversation
                        {
                            Id = Guid.NewGuid(),
                            UserId = request.SenderId,
                            TargetId = request.TargetId,
                            LastReadMessageId = newMessage.Id,
                            NgayTao = now,
                            NguoiTao = sender.Username,
                            NgaySua = now,
                            NguoiSua = sender.Username
                        },
                        new ENTITIES.DbContent.Conversation
                        {
                            Id = Guid.NewGuid(),
                            UserId = request.TargetId,
                            TargetId = request.SenderId,
                            LastReadMessageId = null,
                            NgayTao = now,
                            NguoiTao = sender.Username,
                            NgaySua = now,
                            NguoiSua = sender.Username
                        }
                    };

                    _context.Conversations.AddRange(newConversations);
                }

                _context.SaveChanges();

                // 5. Trả về kết quả sau khi lưu
                response.Data = _mapper.Map<MODELMessage>(newMessage);
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
