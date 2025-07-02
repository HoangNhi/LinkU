using AutoMapper.Execution;
using Azure;
using BE.Services.Conversation;
using BE.Services.FriendRequest;
using BE.Services.GroupMember;
using BE.Services.Message;
using BE.Services.User;
using ENTITIES.DbContent;
using Microsoft.AspNetCore.SignalR;
using MODELS.BASE;
using MODELS.CONVERSATION.Requests;
using MODELS.GROUP.Requests;
using MODELS.GROUPMEMBER.Dtos;
using MODELS.GROUPMEMBER.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.MESSAGEREACTION.Dtos;
using MODELS.MESSAGEREACTION.Requests;
using MODELS.USER.Dtos;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BE.Hubs
{
    public class MessageHub : Hub
    {
        private readonly IFRIENDREQUESTService _friendRequestService;
        private readonly IMESSAGEService _messageService;
        private readonly ICONVERSATIONService _conversationService;
        private readonly IUSERService _userService;
        private readonly IGROUPMEMBERService _groupMemberService;
        private static ConcurrentDictionary<string, string> _users = new();
        private static Dictionary<Guid, List<Guid>> _conversationUserToUser = new();

        public MessageHub(IMESSAGEService messageService, IFRIENDREQUESTService friendRequestService, ICONVERSATIONService conversationService, IUSERService userService, IGROUPMEMBERService groupMemberService)
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

            _groupMemberService = groupMemberService;
        }

        #region Gửi tin nhắn
        // Phương thức gửi tin nhắn tới tất cả client
        public async Task SendPrivateMessage(PostMessageRequest request)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Guid? messageId = null; // Lưu ID tin nhắn để rollback nếu cần
            Guid[]? conversationId = null; // Lưu ID conversation để rollback nếu cần

            try
            {
                // Kiểm tra Id người gửi, người nhận và nội dung tin nhắn có hợp lệ không
                // Kiểm tra tin nhắn
                if (string.IsNullOrWhiteSpace(request.Content) || string.IsNullOrEmpty(request.Content))
                {
                    throw new Exception("Tin nhắn không được để trống");
                }

                // Tạo tin nhắn
                var resultMessage = CreateMessage(request, out messageId);

                // Lấy thông tin người dùng
                var (senderInfo, receiverInfo) = GetUserInfos(request.SenderId, request.TargetId);

                // Kiểm tra cuộc trò chuyện
                EnsureConversation(senderInfo, receiverInfo, resultMessage.Id, out conversationId);

                // Gửi phản hồi đến client
                await SendSignalRMessage(resultMessage);
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
                if (conversationId.Any())
                {
                    foreach (var convId in conversationId)
                    {
                        _conversationService.RoolbackDelete(new GetByIdRequest { Id = convId });
                    }
                }
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Thời gian chạy: {stopwatch.ElapsedMilliseconds} ms");
            }
        }

        /// <summary>
        /// Gửi tin nhắn đến nhóm
        /// </summary>
        public async Task<BaseResponse> SendGroupMessage(PostMessageRequest request)
        {
            var response = new BaseResponse();
            try
            {
                // Kiểm tra Id người gửi, người nhận và nội dung tin nhắn có hợp lệ không
                if (request.SenderId == Guid.Empty || request.TargetId == Guid.Empty || string.IsNullOrWhiteSpace(request.Content))
                {
                    throw new Exception("Nhóm hoặc nội dung tin nhắn không hợp lệ.");
                }

                // Tạo tin nhắn
                var resultMessage = _messageService.Insert(request);

                if (resultMessage.Error)
                {
                    throw new Exception(resultMessage.Message);
                }

                // Lấy thông tin người gửi
                var sender = _userService.GetById(new GetByIdRequest { Id = request.SenderId });
                resultMessage.Data.Sender = sender.Data;

                // Gửi tin nhắn đến tất cả thành viên trong nhóm
                var groupMembers = _groupMemberService.GetListByGroupId(new GetByIdRequest { Id = request.TargetId });
                if (groupMembers.Error)
                {
                    throw new Exception(groupMembers.Message);
                }

                // Xử lý dữ liệu
                

                // Gửi tin nhắn đến tất cả thành viên trong nhóm
                foreach (var member in groupMembers.Data)
                {
                    var Data = _messageService.HanleDataGetListPaging(new List<MODELMessage> { resultMessage.Data }, 1, member.UserId, member.GroupId).Data;

                    var Response = new GetListPagingResponse
                    {
                        PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                        Data = Data,
                        TotalRow = Data.Count
                    };

                    if (_users.TryGetValue(member.UserId.ToString(), out string connectionId))
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", new ApiResponse(Response));
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }
        #endregion


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

                // Nếu đồng ý lời mới kết bạn thì cập nhập lại tab Friendship
                if(friendRequestModel.Data.Status == 1)
                {
                    // Gửi yêu cầu cập nhật đến người nhận
                    await Clients.Client(receiverConnectionId).SendAsync("UpdateFriendshipTab", new ApiResponse(friendRequestModel.Data));

                    // Gửi yêu cầu cập nhật đến người nhận
                    await Clients.Client(senderConnectionId).SendAsync("UpdateFriendshipTab", new ApiResponse(friendRequestModel.Data));
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
                //SendSignalRRerenderTab(request.TargetId, "GetListPagingConversation");
                //SendSignalRRerenderTab(request.UserId, "GetListPagingConversation");
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse> RequestRerenderTab(WSRequestRerenderTab request)
        {
            var response = new BaseResponse();
            try
            {
                if(request.UserIds.Count == 0)
                {
                    throw new Exception("Danh sách UserId không được để trống");
                }
                else
                {
                    foreach (var UserId in request.UserIds)
                    {
                        await SendSignalRRerenderTab(UserId, request.TabName);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Yêu cầu cập nhật lại Tab
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> RequestRerenderTabOfGroupMember(WSRequestRerenderTabOfGroupMember request)
        {
            var response = new BaseResponse();
            try
            {
                // Validate request
                if (request.GroupId == Guid.Empty || request.UserId == Guid.Empty)
                {
                    throw new Exception("Nhóm và người dùng hiện tại không được để trống.");
                }

                var groupMember = _groupMemberService.GetListByGroupId(new GetByIdRequest { Id = request.GroupId });
                if (groupMember.Error)
                {
                    throw new Exception(groupMember.Message);
                }


                List<MODELGroupMember> membersWithoutCurrentUser = groupMember.Data.Where(m => m.UserId != request.UserId).ToList();

                foreach (var member in membersWithoutCurrentUser)
                {
                    if (_users.TryGetValue(member.UserId.ToString(), out string receiverConnectionId))
                    {
                        // Gửi yêu cầu cập nhật đến người nhận
                        await Clients.Client(receiverConnectionId).SendAsync("ReceiveRequestRerenderTabOfGroupMember", new ApiResponse(member.GroupId.ToString()));
                    }
                }

            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse> RequestAppendMessage(WSRequestAppendMessage request)
        {
            var response = new BaseResponse();
            try
            {
                // Validate request
                if (request.SenderId == Guid.Empty || request.TargetId == Guid.Empty)
                {
                    throw new Exception("Dữ liệu không được để trống");
                }

                // Tin nhắn thông thường
                if (request.ConversationType == 0)
                {
                    if (_users.TryGetValue(request.TargetId.ToString(), out string receiverConnectionId))
                    {
                        // Gửi yêu cầu cập nhật đến người nhận
                        await Clients.Client(receiverConnectionId).SendAsync("ReceiveRequestAppendMessage",
                            new ApiResponse(new { Html = request.HtmlMessage, SenderId = request.SenderId, TargetId = request.TargetId })
                        );
                    }
                }
                // Tin nhắn nhóm
                else if(request.ConversationType == 1)
                {
                    var groupMember = _groupMemberService.GetListByGroupId(new GetByIdRequest { Id = request.TargetId });
                    if (groupMember.Error)
                    {
                        throw new Exception(groupMember.Message);
                    }


                    List<MODELGroupMember> membersWithoutCurrentUser = groupMember.Data.Where(m => m.UserId != request.SenderId).ToList();

                    foreach (var member in membersWithoutCurrentUser)
                    {
                        if (_users.TryGetValue(member.UserId.ToString(), out string receiverConnectionId))
                        {
                            // Gửi yêu cầu cập nhật đến người nhận
                            await Clients.Client(receiverConnectionId).SendAsync("ReceiveRequestAppendMessage", 
                                new ApiResponse(new { Html = request.HtmlMessage, SenderId = request.SenderId, TargetId = request.TargetId })
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = "Lỗi hệ thống: " + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse> UpdateMessageReaction(string request)
        {
            var response = new BaseResponse();
            try
            {
                var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<WSUpdateMessageReactionRequest>(request);

                if (requestData == null || requestData.SenderId == Guid.Empty || requestData.TargetId == Guid.Empty || requestData.Reaction == null)
                {
                    throw new Exception("Dữ liệu không được để trống");
                }

                // Lấy thông tin tin nhắn
                var message = _messageService.GetById(new GetByIdRequest { Id = requestData.Reaction.MessageId });
                if (message.Error)
                {
                    throw new Exception(message.Message);
                }

                // Tin nhắn thông thường
                if (requestData.ConversationType == 0)
                {
                    // Xử lý dữ liệu
                    List<MODELMessage> myData = _messageService.HanleDataGetListPaging(new List<MODELMessage> { message.Data }, 0, requestData.SenderId, requestData.TargetId).Data;
                    List<MODELMessage> otherData = _messageService.HanleDataGetListPaging(new List<MODELMessage> { message.Data }, 0, requestData.TargetId, requestData.SenderId).Data;

                    if (_users.TryGetValue(requestData.TargetId.ToString(), out string receiverConnectionId))
                    {
                        // Gửi yêu cầu cập nhật đến người nhận
                        await Clients.Client(receiverConnectionId).SendAsync("ReceiveRequestUpdateMessageReaction",
                            new ApiResponse(
                                new
                                {
                                    SenderId = requestData.SenderId,
                                    TargetId = requestData.TargetId,
                                    Data = new GetListPagingResponse
                                    {
                                        PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                                        Data = otherData,
                                        TotalRow = otherData.Count
                                    }
                                }
                            )
                        );
                    }

                    await Clients.Caller.SendAsync("ReceiveRequestUpdateMessageReaction",
                        new ApiResponse(
                            new
                            {
                                SenderId = requestData.SenderId,
                                TargetId = requestData.TargetId,
                                Data = new GetListPagingResponse
                                {
                                    PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                                    Data = myData,
                                    TotalRow = myData.Count
                                }
                            }
                        )
                    );
                }
                // Tin nhắn nhóm
                else if (requestData.ConversationType == 1)
                {
                    var groupMember = _groupMemberService.GetListByGroupId(new GetByIdRequest { Id = requestData.TargetId });
                    if (groupMember.Error)
                    {
                        throw new Exception(groupMember.Message);
                    }

                    foreach (var member in groupMember.Data)
                    {
                        if (_users.TryGetValue(member.UserId.ToString(), out string receiverConnectionId))
                        {
                            // Xử lý dữ liệu
                            var Data = _messageService.HanleDataGetListPaging(new List<MODELMessage> { message.Data }, 1, member.UserId, member.GroupId).Data;

                            // Gửi yêu cầu cập nhật đến người nhận
                            await Clients.Client(receiverConnectionId).SendAsync("ReceiveRequestUpdateMessageReaction",
                                new ApiResponse(
                                    new
                                    {
                                        SenderId = requestData.SenderId,
                                        TargetId = requestData.TargetId,
                                        Data = new GetListPagingResponse
                                        {
                                            PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                                            Data = Data,
                                            TotalRow = Data.Count
                                        }
                                    }
                                )
                            );
                        }
                    }
                }

            }
            catch(Exception ex)
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

        MODELMessage CreateMessage(PostMessageRequest request, out Guid? messageId)
        {
            var result = _messageService.Insert(request);

            if (result.Error)
            {
                throw new Exception(result.Message);
            }

            //if(!(request.RefId == null || request.RefId == Guid.Empty))
            //{
            //    var refMessage = _messageService.GetById(new GetByIdRequest { Id = request.RefId });
            //    if(refMessage)
            //}

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

        void EnsureConversation(MODELUser sender, MODELUser receiver, Guid messageId, out Guid[]? conversationId)
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
                        conversationId = conversation.Data;
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

                    conversationId = conversation.Data;
                }
            }
        }

        async Task SendSignalRMessage(MODELMessage response)
        {
            // Xử lý dữ liệu
            var myData = _messageService.HanleDataGetListPaging(new List<MODELMessage> { response }, 0, response.SenderId, response.TargetId).Data;
            var otherData = _messageService.HanleDataGetListPaging(new List<MODELMessage> { response }, 0, response.TargetId, response.SenderId).Data;

            var myResponse = new GetListPagingResponse
            {
                PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                Data = myData,
                TotalRow = myData.Count
            };

            var otherResponse = new GetListPagingResponse
            {
                PageIndex = 1, // Chỉ trả về 1 trang vì đây là tin nhắn mới gửi
                Data = otherData,
                TotalRow = otherData.Count
            };


            // Gửi tin nhắn qua SignalR
            if (_users.TryGetValue(response.TargetId.ToString(), out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", new ApiResponse(otherResponse));
            }
            await Clients.Caller.SendAsync("ReceiveMessage", new ApiResponse(myResponse));
        }

        async Task SendSignalRRerenderTab(Guid UserId, string tabname)
        {
            // Gửi tin nhắn qua SignalR
            if (_users.TryGetValue(UserId.ToString(), out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveRequestRerenderTab", new ApiResponse(tabname));
            }
        }
        #endregion
    }
}
