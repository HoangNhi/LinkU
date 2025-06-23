namespace MODELS.MESSAGE.Requests
{
    public class WSRequestAppendMessage
    {
        public Guid SenderId { get; set; }
        public Guid TargetId { get; set; }
        public int ConversationType { get; set; }
        public string HtmlMessage { get; set; } = "";
    }
}
