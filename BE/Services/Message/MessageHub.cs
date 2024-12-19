using Microsoft.AspNetCore.SignalR;
namespace BE.Services.Message
{
    public class MessageHub : Hub
    {
        // Phương thức gửi tin nhắn tới tất cả client
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Phương thức cho user kết nối vào hub
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        // Phương thức khi user ngắt kết nối
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
