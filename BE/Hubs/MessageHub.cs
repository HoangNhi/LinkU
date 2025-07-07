using BE.Services.Conversation;
using BE.Services.FriendRequest;
using BE.Services.GroupMember;
using BE.Services.Message;
using BE.Services.User;
using Microsoft.AspNetCore.SignalR;
using MODELS.BASE;
using MODELS.CONVERSATION.Requests;
using MODELS.GROUP.Requests;
using MODELS.GROUPMEMBER.Dtos;
using MODELS.GROUPMEMBER.Requests;
using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGE.Requests;
using MODELS.MESSAGEREACTION.Requests;
using System.Collections.Concurrent;
using System.Net.WebSockets;

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

        public MessageHub(IMESSAGEService messageService, IFRIENDREQUESTService friendRequestService, ICONVERSATIONService conversationService, IUSERService userService, IGROUPMEMBERService groupMemberService)
        {
            _messageService = messageService;
            _friendRequestService = friendRequestService;
            _conversationService = conversationService;
            _userService = userService;
            _groupMemberService = groupMemberService;
        }

        #region Gửi tin nhắn
        // Phương thức gửi tin nhắn tới tất cả client
        public async Task SendPrivateMessage(PostMessageRequest request)
        {
            try
            {
                // Create Message
                var newMessage = await _messageService.WSInsertPrivateMessage(request);
                if (newMessage.Error)
                {
                    throw new Exception(newMessage.Message);
                }

                // SignalR message
                await SendSignalRMessage(newMessage.Data);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceivePrivateMessage", new ApiResponse(false, null, $"Lỗi hệ thống: {ex.Message}"));
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
                var resultMessage = await _messageService.Insert(request);

                if (resultMessage.Error)
                {
                    throw new Exception(resultMessage.Message);
                }

                // Lấy thông tin người gửi
                var sender = await _userService.GetByIdAsync(new GetByIdRequest { Id = request.SenderId });
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
                    var Data = (await _messageService.HanleDataGetListPagingAsync(new List<MODELMessage> { resultMessage.Data }, 1, member.UserId, member.GroupId)).Data;

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
                if (friendRequestModel.Data.Status == 1)
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
                var update = await _conversationService.UpdateLatestMessage(request.UserId, request.TargetId);

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
                if (request.UserIds.Count == 0)
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
                else if (request.ConversationType == 1)
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
                    List<MODELMessage> myData = (await _messageService.HanleDataGetListPagingAsync(new List<MODELMessage> { message.Data }, 0, requestData.SenderId, requestData.TargetId)).Data;
                    List<MODELMessage> otherData = (await _messageService.HanleDataGetListPagingAsync(new List<MODELMessage> { message.Data }, 0, requestData.TargetId, requestData.SenderId)).Data;

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
                            var Data = (await _messageService.HanleDataGetListPagingAsync(new List<MODELMessage> { message.Data }, 1, member.UserId, member.GroupId)).Data;

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
        async Task SendSignalRMessage(MODELMessage response)
        {
            // Hàm xử lý tạo response và gửi đi
            async Task SendToClient(Guid userId, Guid targetId, Func<GetListPagingResponse, Task> sendFunc)
            {
                var handleMessage = await _messageService.HanleDataGetListPagingAsync(
                    new List<MODELMessage> { response }, 0, userId, targetId);

                var data = handleMessage.Data;

                var pagingResponse = new GetListPagingResponse
                {
                    PageIndex = 1,
                    Data = data,
                    TotalRow = data.Count
                };

                await sendFunc.Invoke(pagingResponse);
            }

            var tasks = new List<Task>();

            // Gửi cho người nhận nếu họ đang online
            if (_users.TryGetValue(response.TargetId.ToString(), out string receiverConnectionId))
            {
                tasks.Add(Task.Run(async () =>
                {
                    await SendToClient(response.TargetId, response.SenderId, async (res) =>
                    {
                        Clients.Client(receiverConnectionId)
                            .SendAsync("ReceiveMessage", new ApiResponse(res));
                    });
                }));
            }

            // Gửi cho người gửi (caller)
            tasks.Add(Task.Run(async () =>
            {
                await SendToClient(response.SenderId, response.TargetId, async (res) =>
                {
                    Clients.Caller.SendAsync("ReceiveMessage", new ApiResponse(res));
                });
            }));

            await Task.WhenAll(tasks);
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
