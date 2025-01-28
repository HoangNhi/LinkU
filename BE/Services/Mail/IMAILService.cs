using MODELS.MAIL.Dtos;

namespace BE.Services.Mail
{
    public interface IMAILService
    {
        Task SendEmailAsync(MODELMail mailRequest);
    }
}
