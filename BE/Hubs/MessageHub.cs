using Azure;
using BE.Services.Conversation;
using BE.Services.FriendRequest;
using BE.Services.Message;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.AspNetCore.SignalR;
using MimeKit;
using MODELS.BASE;
using MODELS.CONVERSATION.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.USER.Dtos;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace BE.Hubs
{
    public class MessageHub : Hub
    {
        private readonly IFRIENDREQUESTService _friendRequestService;
        private readonly IMESSAGEService _messageService;
        private readonly ICONVERSATIONService _conversationService;
        private readonly IUSERService _userService;
        private static ConcurrentDictionary<string, string> _users = new();
        private static Dictionary<Guid, List<Guid>> _conversationUserToUser = new();

        public MessageHub(IMESSAGEService messageService, IFRIENDREQUESTService friendRequestService, ICONVERSATIONService conversationService, IUSERService userService)
        {
            _messageService = messageService;
            _friendRequestService = friendRequestService;
            _conversationService = conversationService;
            _userService = userService;

            var conversationCurrent = conversationService.GetDictionaryConversationUserToUser();
            if (conversationCurrent.Error)
            {
                throw new Exception(conversationCurrent.Message);
            }
            else
            {
                _conversationUserToUser = conversationCurrent.Data;
            }
        }

        // Phương thức gửi tin nhắn tới tất cả client
        public async Task SendPrivateMessage(string sender, string receiver, string message)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Guid? messageId = null; // Lưu ID tin nhắn để rollback nếu cần
            Guid? conversationId = null; // Lưu ID conversation để rollback nếu cần

            try
            {
                // Kiểm tra Id người gửi, người nhận và nội dung tin nhắn có hợp lệ không
                var (senderId, receiverId) = ValidateMessage(sender, receiver, message);

                // Tạo tin nhắn
                var resultMessage = CreateMessage(senderId, receiverId, message, out messageId);

                // Lấy thông tin người dùng
                var (senderInfo, receiverInfo) = GetUserInfos(senderId, receiverId);

                // Kiểm tra cuộc trò chuyện
                EnsureConversation(senderInfo, receiverInfo, resultMessage.Id, out conversationId);

                // Gửi phẩn hồi đến client
                await SendSignalRMessage(senderId, receiverId, message, resultMessage.GetDateTime());
            }
            catch (Exception ex)
            {
                // Không cần rollback thủ công vì TransactionScope sẽ tự động rollback nếu transaction không được Complete
                string error = $"Lỗi hệ thống: {ex.Message}";
                await Clients.Caller.SendAsync("ReceivePrivateMessage", new ApiResponse(false, null, error));

                // (Tùy chọn) Xóa thủ công nếu không dùng TransactionScope
                if (messageId.HasValue)
                {
                    _messageService.RoolbackDelete(new GetByIdRequest { Id = messageId.Value });
                }
                if (conversationId.HasValue)
                {
                    _conversationService.RoolbackDelete(new GetByIdRequest { Id = conversationId.Value });
                }
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Thời gian chạy: {stopwatch.ElapsedMilliseconds} ms");
            }
        }

        // Gửi yêu cầu cập nhật giao diện cho cả người gửi và người nhận
        public async Task UpdateFriendRequest(string RequestId)
        {
            var friendRequestModel = _friendRequestService.GetById(new MODELS.BASE.GetByIdRequest { Id = Guid.Parse(RequestId) });
            if (!friendRequestModel.Error)
            {
                // Kiểm tra xem người nhận có đang trực tuyến không
                if (_users.TryGetValue(friendRequestModel.Data.ReceiverId.ToString(), out string receiverConnectionId))
                {
                    // Gửi yêu cầu cập nhật đến người nhận
                    await Clients.Client(receiverConnectionId).SendAsync("UpdateFriendRequest", new ApiResponse(friendRequestModel.Data));
                }

                // Kiểm tra xem người nhận có đang trực tuyến không
                if (_users.TryGetValue(friendRequestModel.Data.SenderId.ToString(), out string senderConnectionId))
                {
                    // Gửi yêu cầu cập nhật đến người nhận
                    await Clients.Client(senderConnectionId).SendAsync("UpdateFriendRequest", new ApiResponse(friendRequestModel.Data));
                }
            }
        }

        public async Task<BaseResponse> UpdateConversationLatestMessage(WSPrivateMessageUpdateConversation request)
        {
            var response = new BaseResponse();
            try
            {
                var update = _conversationService.UpdateLatestMessage(request.UserId, request.TargetId);

                if (update.Error) 
                {
                    throw new Exception(update.Message);
                }

                SendSignalRRerenderTab(request.TargetId, "GetListPagingConversation");
                SendSignalRRerenderTab(request.UserId, "GetListPagingConversation");
            }
            catch (Exception ex) 
            {
                response.Error = true;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }

        #region Connect/Disconnect
        // Phương thức cho user kết nối vào hub
        public override async Task OnConnectedAsync()
        {
            string UserId = Context.GetHttpContext().Request.Query["userid"];
            _users[UserId] = Context.ConnectionId;

            await base.OnConnectedAsync();
        }

        // Phương thức khi user ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var UserId = _users.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            _users.TryRemove(UserId, out _);
            await base.OnDisconnectedAsync(exception);
        }
        #endregion

        #region Private Function 
        (Guid senderId, Guid receiverId) ValidateMessage(string sender, string receiver, string message)
        {
            // Kiểm tra định dạng ID
            if (!Guid.TryParse(sender, out var senderId) || !Guid.TryParse(receiver, out var receiverId))
            {
                throw new Exception("ID người gửi hoặc người nhận không hợp lệ.");
            }

            // Kiểm tra tin nhắn
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Tin nhắn không được để trống.");
            }

            return (senderId, receiverId);
        }

        MODELMessage CreateMessage(Guid senderId, Guid receiverId, string message, out Guid? messageId)
        {
            var result = _messageService.Insert(new PostMessageRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
            });

            if (result.Error)
            {
                throw new Exception(result.Message);
            }
            messageId = result.Data.Id; // Lưu ID tin nhắn

            return result.Data;
        }

        (MODELUser senderInfo, MODELUser receiverInfo) GetUserInfos(Guid senderId, Guid receiverId)
        {
            var senderInfo = _userService.GetById(new GetByIdRequest { Id = senderId });
            var receiverInfo = _userService.GetById(new GetByIdRequest { Id = receiverId });
            if (senderInfo.Error || receiverInfo.Error)
            {
                throw new Exception("Không tìm thấy thông tin người dùng.");
            }

            return (senderInfo.Data, receiverInfo.Data);
        }

        void EnsureConversation(MODELUser sender, MODELUser receiver, Guid messageId, out Guid? conversationId)
        {
            // Khởi tạo mặc định cho conversationId
            conversationId = null;

            if (_conversationUserToUser.TryGetValue(sender.Id, out List<Guid> existingConversationId))
            {
                if (existingConversationId.Contains(receiver.Id))
                {
                    // Nếu đã có cuộc trò chuyện, update lastread conversation là mới nhất
                    var update = _conversationService.UpdateLatestMessage(sender.Id, receiver.Id);
                    if (update.Error)
                    {
                        throw new Exception(update.Message);
                    }
                }
                else
                {
                    // Nếu chưa có cuộc trò chuyện, kiểm tra trong DB
                    var checkConversation = _conversationService.CheckConversationExist(sender.Id, receiver.Id);
                    if (checkConversation.Error)
                    {
                        throw new Exception(checkConversation.Message);
                    }

                    if (checkConversation.Data)
                    {
                        // Đồng bộ với _conversationCurrent
                        existingConversationId.Add(receiver.Id);
                        _conversationUserToUser[sender.Id] = existingConversationId;

                        // update lastread conversation là mới nhất
                        var update = _conversationService.UpdateLatestMessage(sender.Id, receiver.Id);
                        if (update.Error)
                        {
                            throw new Exception(update.Message);
                        }
                    }
                    else
                    {
                        // Tạo mới conversation
                        var conversation = _conversationService.InsertPrivateConversation(new WSPrivateMessageInsertConversation
                        {
                            MessageId = messageId,
                            Sender = sender,
                            Receiver = receiver,
                        });

                        if (conversation.Error)
                        {
                            throw new Exception(conversation.Message);
                        }


                        existingConversationId.Add(receiver.Id);
                        _conversationUserToUser[sender.Id] = existingConversationId; // Cập nhật sau khi tạo thành công

                        // Lưu ID conversation
                        conversationId = conversation.Data.Id;
                    }
                }
            }
            else
            {
                // Kiểm tra trong DB
                var checkConversation = _conversationService.CheckConversationExist(sender.Id, receiver.Id);
                if (checkConversation.Error)
                {
                    throw new Exception(checkConversation.Message);
                }

                if (checkConversation.Data)
                {
                    // Đồng bộ với _conversationCurrent
                    _conversationUserToUser.Add(sender.Id, new List<Guid> { receiver.Id });

                    // update lastread conversation là mới nhất
                    var update = _conversationService.UpdateLatestMessage(sender.Id, receiver.Id);
                    if (update.Error)
                    {
                        throw new Exception(update.Message);
                    }
                }
                else
                {
                    // Tạo mới conversation
                    var conversation = _conversationService.InsertPrivateConversation(new WSPrivateMessageInsertConversation
                    {
                        MessageId = messageId,
                        Sender = sender,
                        Receiver = receiver,
                    });

                    if (conversation.Error)
                    {
                        throw new Exception(conversation.Message);
                    }

                    // Cập nhật sau khi tạo thành công
                    _conversationUserToUser.Add(sender.Id, new List<Guid> { receiver.Id });

                    conversationId = conversation.Data.Id;
                }
            }
        }

        async Task SendSignalRMessage(Guid senderId, Guid receiverId, string message, string Datetime)
        {
            var response = new MODELPrivateMessageResponse
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                DateTime = Datetime
            };

            // Gửi tin nhắn qua SignalR
            if (_users.TryGetValue(receiverId.ToString(), out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", new ApiResponse(response));
            }
            await Clients.Caller.SendAsync("ReceivePrivateMessage", new ApiResponse(response));
        }

        async Task SendSignalRRerenderTab(Guid UserId, string tab)
        {
            // Gửi tin nhắn qua SignalR
            if (_users.TryGetValue(UserId.ToString(), out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("RerenderTab", new ApiResponse(tab));
            }
        }
        #endregion
    }
}
