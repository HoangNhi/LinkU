namespace BE.Services.Redis
{
    public interface IREDISService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task<bool> RemoveAsync(string key);
    }
}
