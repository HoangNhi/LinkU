using Microsoft.AspNetCore.Http;

namespace MODELS.MAIL.Dtos
{
    public class MODELMail
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
