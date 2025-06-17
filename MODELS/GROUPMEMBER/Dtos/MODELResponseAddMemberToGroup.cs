namespace MODELS.GROUPMEMBER.Dtos
{
    public class MODELResponseAddMemberToGroup
    {
        /// <summary>
        /// Id thành viên mới được thêm vào nhóm.
        /// </summary>
        public List<Guid> NewMemberIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Id người dùng được gửi lời mời tham gia nhóm.
        /// </summary>
        public List<Guid> InvitedUserIds { get; set; } = new List<Guid>();
    }
}
