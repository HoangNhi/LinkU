using Microsoft.Extensions.Options;
using MODELS.SMS.Dtos;

namespace BE.Services.SMS
{
    public class SMSService : ISMSService
    {
        private readonly SMSoptions _smsSettings;
        public SMSService(IOptions<SMSoptions> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }

        public Task SendSmsAsync(string number, string message)
        {
            return Task.FromResult(0);
        }
    }
}
