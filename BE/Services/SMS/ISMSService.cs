namespace BE.Services.SMS
{
    public interface ISMSService
    {
        Task SendSmsAsync(string number, string message);
    }
}
