using MODELS.BASE;
using MODELS.COMMON;

namespace FE.Services
{
    public interface ICONSUMEAPIService
    {
        ApiResponse ExcuteAPI(string action, object? model, HttpAction method);
        ApiResponse ExcuteAPIWithoutToken(string action, object? model, HttpAction method);
        string GetBEUrl();
    }
}
