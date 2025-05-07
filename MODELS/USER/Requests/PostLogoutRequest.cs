namespace MODELS.USER.Requests
{
    public class PostLogoutRequest
    {
        public Guid UserId { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
