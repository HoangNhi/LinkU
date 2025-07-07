namespace BE.Helpers
{
    public static class RedisKeyHelper
    {
        public static string UserProfile(Guid userId) => $"user:profile:{userId}";
        public static string MessageHandled(Guid messageId) => $"message:handled:{messageId}";
        public static string MediaFilesByMessageId(Guid messageId) => $"mediafile:messageid:{messageId}";
        public static string ReactionTypeById(Guid reactionTypeId) => $"reactiontype:{reactionTypeId}";
    }
}
