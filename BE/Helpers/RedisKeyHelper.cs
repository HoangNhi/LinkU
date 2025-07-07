namespace BE.Helpers
{
    public static class RedisKeyHelper
    {
        public static string UserProfile(Guid userId) => $"user:profile:{userId}";
        public static string MessageById(Guid messageId) => $"message:{messageId}";
        public static string MediaFilesByMessageId(Guid messageId) => $"mediafile:messageid:{messageId}";
        public static string ReactionTypeById(Guid reactionTypeId) => $"reactiontype:{reactionTypeId}";
        public static string GroupMemberByGroupId(Guid groupId) => $"group:member:{groupId}";
    }
}
