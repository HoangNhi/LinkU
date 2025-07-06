namespace BE.Helpers
{
    public static class RedisKeyHelper
    {
        public static string UserProfile(Guid userId) => $"user:profile:{userId}";
        public static string MessageHandled(Guid messageId) => $"message:handled:{messageId}";
    }
}
