using MODELS.BASE;

namespace MODELS.MESSAGE.Dtos
{
    public class MODELSendMessageWithFileResponse
    {
        public GetListPagingResponse Messages { get; set; } = new GetListPagingResponse();
        public bool IsMyResponse { get; set; }
    }
}
