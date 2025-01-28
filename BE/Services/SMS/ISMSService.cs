using MODELS.MAIL.Dtos;
using MODELS.SMS.Dtos;

namespace BE.Services.SMS
{
    public interface ISMSService
    {
        Task SendSmsAsync(string number, string message);
    }
}
