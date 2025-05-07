namespace MODELS.FRIENDREQUEST.Dtos
{
    public class MODELFriendStatus : MODELFriendRequest
    {
        public bool IsFriend { get; set; }
        public bool IsSentRequest { get; set; }
        public bool IsMyRequest { get; set; }
    }
}
