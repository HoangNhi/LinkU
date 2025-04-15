using BE.Services.FriendRequest;
using BE.Services.Message;
using ENTITIES.DbContent;
using Microsoft.AspNetCore.SignalR;
using MODELS.MESSAGE.Requests;
using System;
using System.Collections.Concurrent;
namespace BE.Hubs
{
    public class MessageHub : Hub
    {
        private readonly IFRIENDREQUESTService _friendRequestService;
        private readonly IMESSAGEService _messageService;
        private static ConcurrentDictionary<string, string> _users = new();

        public MessageHub(IMESSAGEService messageService, IFRIENDREQUESTService friendRequestService)
        {
            _messageService = messageService;
            _friendRequestService = friendRequestService;
        }

        // Phương thức gửi tin nhắn tới tất cả client
        public async Task SendPrivateMessage(string sender, string receiver, string message)
        {
            // Tạo tin nhắn
            var result = _messageService.Insert(new PostMessageRequest
            {
                SenderId = Guid.Parse(sender),
                ReceiverId = Guid.Parse(receiver),
                Content = message,
            });

            // Kiểm tra xem người nhận có đang trực tuyến không
            if (_users.TryGetValue(receiver, out string receiverConnectionId))
            {
                // Gửi tin nhắn đến người nhận
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", sender, message, result.Data.GetDateTime());
            }

            // Gửi lại tin nhắn cho người gửi để hiển thị
            await Clients.Caller.SendAsync("ReceivePrivateMessage", sender, message, result.Data.GetDateTime());
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
                    await Clients.Client(receiverConnectionId).SendAsync("UpdateFriendRequest");
                }

                // Kiểm tra xem người nhận có đang trực tuyến không
                if (_users.TryGetValue(friendRequestModel.Data.SenderId.ToString(), out string senderConnectionId))
                {
                    // Gửi yêu cầu cập nhật đến người nhận
                    await Clients.Client(senderConnectionId).SendAsync("UpdateFriendRequest");
                }
            }
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
    }
}
